using System.Text;
using CleanHr.Application.Infrastructures;
using CleanHr.Domain.Exceptions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Api.Filters;

public class ExceptionHandlerFilter(IExceptionLogger exceptionLogger) : IAsyncExceptionFilter
{
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

        await exceptionLogger.LogAsync(context.Exception, requestPath, requestBoy);

        context.Result = new StatusCodeResult(500);
    }
}
