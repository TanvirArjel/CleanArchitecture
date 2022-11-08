using System.Text;
using EmployeeManagement.Application.Infrastructures;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TanvirArjel.ArgumentChecker;

namespace EmployeeManagement.Api.Filters;

public class ExceptionHandlerFilter : IAsyncExceptionFilter
{
    private readonly IExceptionLogger _exceptionLogger;

    public ExceptionHandlerFilter(IExceptionLogger exceptionLogger)
    {
        _exceptionLogger = exceptionLogger;
    }

    public async Task OnExceptionAsync(ExceptionContext context)
    {
        context.ThrowIfNull(nameof(context));

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

        using (StreamReader reader = new StreamReader(httpRequest.Body, Encoding.UTF8))
        {
            requestBoy = await reader.ReadToEndAsync();
        }

        await _exceptionLogger.LogAsync(context.Exception, requestPath, requestBoy);

        context.Result = new StatusCodeResult(500);
    }
}
