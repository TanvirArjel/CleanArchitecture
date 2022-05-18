using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Identity.Application.Infrastrucures;
using Identity.Domain.Entities;
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
using TanvirArjel.ArgumentChecker;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace Identity.Application.Services;

[ScopedService]
public class ViewRenderService
{
    private readonly IRazorViewEngine _razorViewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;
    private readonly UserManager<User> _userManager;
    private readonly IExceptionLogger _exceptionLogger;

    public ViewRenderService(
        IRazorViewEngine razorViewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider,
        UserManager<User> userManager,
        IExceptionLogger exceptionLogger)
    {
        _razorViewEngine = razorViewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
        _userManager = userManager;
        _exceptionLogger = exceptionLogger;
    }

    public async Task<string> RenderViewToStringAsync(string viewNameOrPath, object model)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(viewNameOrPath))
            {
                throw new ArgumentNullException(nameof(viewNameOrPath));
            }

            DefaultHttpContext httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            ActionContext actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

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

            using StringWriter stringWriter = new StringWriter();
            ViewDataDictionary viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            TempDataDictionary tempData = new TempDataDictionary(actionContext.HttpContext, _tempDataProvider);

            ViewContext viewContext = new ViewContext(actionContext, view, viewDictionary, tempData, stringWriter, new HtmlHelperOptions());

            await view.RenderAsync(viewContext);

            return stringWriter.ToString();
        }
        catch (Exception exception)
        {
            var methodParameterObj = new { ViewName = viewNameOrPath, Model = model };
            await _exceptionLogger.LogAsync(exception, methodParameterObj);
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
            var methodParameterObj = new { ViewName = viewNameOrPath, Model = model, UserId = userId };
            await _exceptionLogger.LogAsync(exception, methodParameterObj);
            throw;
        }
    }
}
