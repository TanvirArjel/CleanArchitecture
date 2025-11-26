using CleanHr.Application.Services;
using CleanHr.Domain;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed class LoginResponseDto
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }
}

public sealed class LoginUserCommand(string emailOrUserName, string password) : IRequest<Result<LoginResponseDto>>
{
    public string EmailOrUserName { get; } = emailOrUserName.ThrowIfNullOrEmpty(nameof(emailOrUserName));

    public string Password { get; } = password.ThrowIfNullOrEmpty(nameof(password));
}

internal class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<LoginResponseDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository _repository;
    private readonly JwtTokenManager _jwtTokenManager;

    public LoginUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        IRepository repository,
        JwtTokenManager jwtTokenManager)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _jwtTokenManager = jwtTokenManager ?? throw new ArgumentNullException(nameof(jwtTokenManager));
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        ApplicationUser applicationUser = await _userManager.FindByEmailAsync(request.EmailOrUserName);

        applicationUser ??= await _userManager.FindByNameAsync(request.EmailOrUserName);

        if (applicationUser == null)
        {
            return Result<LoginResponseDto>.Failure("EmailOrUserName", "The email or username does not exist.");
        }

        // Check if user is disabled
        if (applicationUser.IsDisabled)
        {
            return Result<LoginResponseDto>.Failure(string.Empty, "The account has been disabled.");
        }

        // Check if email is confirmed (if required)
        if (!await _userManager.IsEmailConfirmedAsync(applicationUser))
        {
            return Result<LoginResponseDto>.Failure("EmailOrUserName", "The email is not confirmed yet.");
        }

        // Check if account is locked out
        if (await _userManager.IsLockedOutAsync(applicationUser))
        {
            return Result<LoginResponseDto>.Failure(string.Empty, "The account is locked.");
        }

        // Verify password
        bool isPasswordValid = await _userManager.CheckPasswordAsync(applicationUser, request.Password);

        if (!isPasswordValid)
        {
            // Increment access failed count for lockout protection
            await _userManager.AccessFailedAsync(applicationUser);
            return Result<LoginResponseDto>.Failure("Password", "Password is incorrect.");
        }

        // Reset access failed count on successful login
        await _userManager.ResetAccessFailedCountAsync(applicationUser);

        // Record login using domain method
        applicationUser.RecordLogin();
        _repository.Update(applicationUser);
        await _repository.SaveChangesAsync(cancellationToken);

        // Generate JWT token and refresh token
        (string accessToken, string refreshToken) = await _jwtTokenManager.GetTokensAsync(applicationUser);

        LoginResponseDto response = new()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return Result<LoginResponseDto>.Success(response);
    }
}
