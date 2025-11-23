using System.Text;
using CleanHr.Application.Extensions;
using CleanHr.Domain.Exceptions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Api.Filters;

internal sealed class ExceptionHandlerFilter : IAsyncExceptionFilter
{
    private readonly ILogger<ExceptionHandlerFilter> _exceptionLogger;

    public ExceptionHandlerFilter(ILogger<ExceptionHandlerFilter> exceptionLogger)
    {
        _exceptionLogger = exceptionLogger ?? throw new ArgumentNullException(nameof(exceptionLogger));
    }

    public async Task OnExceptionAsync(ExceptionContext context)
    {
        context.ThrowIfNull(nameof(context));

        // DomainValidationException should be treated as a validation error.
        if (context.Exception is DomainValidationException)
        {
            context.ModelState.AddModelError(string.Empty, context.Exception.Message);
            context.Result = new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
            return;
        }

        // EntityNotFoundException should be treated as a validation error.
        if (context.Exception is EntityNotFoundException)
        {
            context.ModelState.AddModelError(string.Empty, context.Exception.Message);
            context.Result = new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
            return;
        }

        HttpRequest httpRequest = context.HttpContext.Request;
        string requestPath = httpRequest.GetEncodedUrl();

        string requestBoy = string.Empty;

        try
        {
            httpRequest.Body.Seek(0, SeekOrigin.Begin);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Can't rewind body stream. " + ex.Message);
        }

        using StreamReader streamReader = new(httpRequest.Body, Encoding.UTF8);
        requestBoy = await streamReader.ReadToEndAsync();

        Dictionary<string, object> fields = new()
        {
            { "RequestPath", requestPath },
            { "RequestBody", requestBoy }
        };

        _exceptionLogger.LogException(context.Exception, $"Error occurred while processing request to {requestPath}", fields);

        context.Result = new StatusCodeResult(500);
    }
}
