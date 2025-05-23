using PizzaShop.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
namespace PizzaShop.Service.Implementations;
public class RazorViewToStringRenderer : IRazorViewToStringRenderer
{
    private readonly IRazorViewEngine _razorViewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;

    public RazorViewToStringRenderer(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
    {
        _razorViewEngine = razorViewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
    }

    public async Task<string> RenderViewToStringAsync(string viewName, object model)
    {
   
        var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

        var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);

        if (!viewResult.Success)
        {
            var errorMessage = $"View '{viewName}' not found. Error: {viewResult.SearchedLocations}";
            Console.WriteLine(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }
        var view = viewResult.View;

       
        using var writer = new StringWriter();

        var viewContext = new ViewContext(
            actionContext,
            view,
            new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model },
            new TempDataDictionary(httpContext, _tempDataProvider),
            writer,
            new HtmlHelperOptions()
        );

        await view.RenderAsync(viewContext);

      
        return writer.ToString();
        // return null;
    }
}