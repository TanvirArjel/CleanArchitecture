﻿using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using CleanHr.Application.Infrastructures;
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
using TanvirArjel.ArgumentChecker;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace CleanHr.Application.Services;

[ScopedService]
public sealed class ViewRenderService(
    IRazorViewEngine razorViewEngine,
    ITempDataProvider tempDataProvider,
    IServiceProvider serviceProvider,
    UserManager<ApplicationUser> userManager,
    IExceptionLogger exceptionLogger)
{
    public async Task<string> RenderViewToStringAsync(string viewNameOrPath, object model)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(viewNameOrPath))
            {
                throw new ArgumentNullException(nameof(viewNameOrPath));
            }

            DefaultHttpContext httpContext = new() { RequestServices = serviceProvider };
            ActionContext actionContext = new(httpContext, new RouteData(), new ActionDescriptor());

            ViewEngineResult viewEngineResult = razorViewEngine.FindView(actionContext, viewNameOrPath, false);

            if (!viewEngineResult.Success)
            {
                viewEngineResult = razorViewEngine.GetView(executingFilePath: viewNameOrPath, viewPath: viewNameOrPath, isMainPage: false);
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

            TempDataDictionary tempData = new(actionContext.HttpContext, tempDataProvider);

            ViewContext viewContext = new(actionContext, view, viewDictionary, tempData, stringWriter, new HtmlHelperOptions());

            await view.RenderAsync(viewContext);

            return stringWriter.ToString();
        }
        catch (Exception exception)
        {
            var methodParameterObj = new { ViewName = viewNameOrPath, Model = model };
            await exceptionLogger.LogAsync(exception, methodParameterObj);
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

            string userLanguageCulture = await userManager.Users.Where(u => u.Id == userId)
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
            await exceptionLogger.LogAsync(exception, methodParameterObj);
            throw;
        }
    }
}
