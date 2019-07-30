using Sitecore.Data;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Presentation;
using Sitecore.XA.Foundation.Presentation.HtmlCaching;
using Sitecore.XA.Foundation.Presentation.Pipelines.GetRenderingCaching;
using Sitecore.XA.Foundation.Presentation.Services;

namespace Sitecore.Support.XA.Foundation.PlaceholderSettings.Pipelines.GetRenderingCaching
{
  public class GetPlaceholderRenderingCaching
  {
    private readonly IPlaceholderCachingResolver _placeholderCachingResolver;

    public GetPlaceholderRenderingCaching(IPlaceholderCachingResolver placeholderCachingResolver)
    {
      _placeholderCachingResolver = placeholderCachingResolver;
    }

    public void Process(GetRenderingCachingArgs args)
    {
      if (args.Rendering?.Item?.Database != null)
      {
        CachingOptions cachingOptionsFromContext = _placeholderCachingResolver.GetCachingOptionsFromContext(PageContext.Current, ID.Parse(args.Rendering.DeviceId), args.Rendering.Placeholder);
        if (cachingOptionsFromContext != null && cachingOptionsFromContext.ResetCachingOptions)
        {
          args.Rendering["Cacheable"] = cachingOptionsFromContext.Cacheable.ToBoolString();
          args.Rendering["Cache_VaryByData"] = cachingOptionsFromContext.VaryByData.ToBoolString();
          args.Rendering["Cache_VaryByDevice"] = cachingOptionsFromContext.VaryByDevice.ToBoolString();
          args.Rendering["Cache_VaryByLogin"] = cachingOptionsFromContext.VaryByLogin.ToBoolString();
          args.Rendering["Cache_VaryByParameters"] = cachingOptionsFromContext.VaryByParm.ToBoolString();
          args.Rendering["Cache_VaryByQueryString"] = cachingOptionsFromContext.VaryByQueryString.ToBoolString();
          args.Rendering["Cache_VaryByUser"] = cachingOptionsFromContext.VaryByUser.ToBoolString();
          args.Rendering["InheritedCaching"] = "1";
          args.CachingDefinition = args.Rendering.Caching;
          args.AbortPipeline();
        }
      }
    }
  }
}