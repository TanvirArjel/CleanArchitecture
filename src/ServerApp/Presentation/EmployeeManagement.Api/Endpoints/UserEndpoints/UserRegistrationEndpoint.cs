using System.ComponentModel.DataAnnotations;
using EmployeeManagement.Application.Infrastrucures;
using EmployeeManagement.Domain.Aggregates.IdentityAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EmployeeManagement.Api.Endpoints.UserEndpoints;

[ApiVersion("1.0")]
public class UserRegistrationEndpoint : UserEndpointBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IExceptionLogger _exceptionLogger;

    public UserRegistrationEndpoint(UserManager<ApplicationUser> userManager, IExceptionLogger exceptionLogger)
    {
        _userManager = userManager;
        _exceptionLogger = exceptionLogger;
    }

    [HttpPost("registration")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Create or register new user by posting the required data.")]
    public async Task<ActionResult> Post(RegistrationModel model)
    {
        try
        {
            ApplicationUser applicationUser = new ApplicationUser
            {
                FullName = model.FirstName + " " + model.LastName,
                UserName = model.Email,
                Email = model.Email
            };

            IdentityResult identityResult = await _userManager.CreateAsync(applicationUser, model.Password);

            if (identityResult.Succeeded == false)
            {
                foreach (IdentityError item in identityResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }

                return BadRequest(ModelState);
            }

            //// await _applicationUserService.SendEmailVerificationCodeAsync(applicationUser.Email);
            return Ok();
        }
        catch (Exception exception)
        {
            model.Password = null;
            model.ConfirmPassword = null;
            await _exceptionLogger.LogAsync(exception, model);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}

public class RegistrationModel
{
    [Required]
    [MaxLength(30, ErrorMessage = "The {1} can't be more than {1} characters long.")]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(30, ErrorMessage = "The {1} can't be more than {1} characters long.")]
    public string LastName { get; set; }

    [Required]
    [MinLength(4, ErrorMessage = "The {0} must be at least {1} characters long.")]
    [MaxLength(50, ErrorMessage = "The {1} can't be more than {1} characters long.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; }

    [Required]
    [MinLength(6, ErrorMessage = "The {0} must be at least {1} characters long.")]
    [MaxLength(20, ErrorMessage = "The {1} can't be more than {1} characters long.")]
    public string Password { get; set; }

    [Required]
    [Compare(nameof(Password), ErrorMessage = "Confirm password does match with password.")]
    public string ConfirmPassword { get; set; }
}
