using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using CleanHr.Application.Extensions;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace CleanHr.Application.Services;

[ScopedService]
public sealed class ViewRenderService
{
    private readonly IRazorViewEngine _razorViewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly ILogger<ViewRenderService> _logger;

    public ViewRenderService(
        IRazorViewEngine razorViewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider,
        UserManager<ApplicationUser> userManager,
        ILogger<ViewRenderService> logger)
    {
        _razorViewEngine = razorViewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<string> RenderViewToStringAsync(string viewNameOrPath, object model)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(viewNameOrPath))
            {
                throw new ArgumentNullException(nameof(viewNameOrPath));
            }

            DefaultHttpContext httpContext = new() { RequestServices = _serviceProvider };
            ActionContext actionContext = new(httpContext, new RouteData(), new ActionDescriptor());

            ViewEngineResult viewEngineResult = _razorViewEngine.FindView(actionContext, viewNameOrPath, false);

            if (!viewEngineResult.Success)
            {
                viewEngineResult = _razorViewEngine.GetView(executingFilePath: viewNameOrPath, viewPath: viewNameOrPath, isMainPage: false);
            }

            if (viewEngineResult.View == null || !viewEngineResult.Success)
            {
                throw new ArgumentNullException($"Unable to find view '{viewNameOrPath}'");
            }

            IView view = viewEngineResult.View;

            using StringWriter stringWriter = new();
            ViewDataDictionary viewDictionary = new(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            TempDataDictionary tempData = new(actionContext.HttpContext, _tempDataProvider);

            ViewContext viewContext = new(actionContext, view, viewDictionary, tempData, stringWriter, new HtmlHelperOptions());

            await view.RenderAsync(viewContext);

            return stringWriter.ToString();
        }
        catch (Exception exception)
        {
            Dictionary<string, object> fields = new()
            {
                { "ViewNameOrPath", viewNameOrPath },
                { "Model", model }
            };

            _logger.LogWithLevel(LogLevel.Error, exception.Message, fields);
            throw;
        }
    }

    public async Task<string> RenderViewToStringAsync(string viewNameOrPath, object model, Guid userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(viewNameOrPath))
            {
                throw new ArgumentNullException(nameof(viewNameOrPath));
            }

            userId.ThrowIfEmpty(nameof(userId));

            string userLanguageCulture = await _userManager.Users.Where(u => u.Id == userId)
                .Select(u => u.LanguageCulture).FirstOrDefaultAsync();

            if (!string.IsNullOrWhiteSpace(userLanguageCulture))
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(userLanguageCulture);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(userLanguageCulture);
            }

            string viewString = await RenderViewToStringAsync(viewNameOrPath, model);
            return viewString;
        }
        catch (Exception exception)
        {
            Dictionary<string, object> methodParamsDict = new() {
                { "ViewNameOrPath", viewNameOrPath },
                { "Model", model },
                { "UserId", userId }
            };

            _logger.LogWithLevel(LogLevel.Error, exception.Message, methodParamsDict);
            throw;
        }
    }
}
