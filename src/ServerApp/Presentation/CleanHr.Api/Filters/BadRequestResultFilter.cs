using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CleanHr.Api.Filters;

internal sealed class BadRequestResultFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context != null && context.Result is BadRequestObjectResult)
        {
            context.Result = new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        // Do nothing
    }
}
