using System;
using Sitecore.Caching;
using Sitecore.Configuration;
using Sitecore.Mvc.Presentation;
using Sitecore.Pipelines;
using Sitecore.XA.Foundation.Presentation.Pipelines.GetRenderingCaching;
using Sitecore.XA.Foundation.Presentation.Services;

namespace Sitecore.Support.XA.Foundation.Presentation.Services
{
  public class PipelineBasedRenderingCachingService : IRenderingCachingService
  {
    private static RenderingCachingOptionsCache RenderingCacheOptions = new RenderingCachingOptionsCache("SXA[RenderingCachingOptions]", StringUtil.ParseSizeString(Settings.GetSetting("SXA.Presentation.RenderingCachingOptionsCacheMaxSize", "5MB")));
    public class RenderingCachingOptionsCache : CustomCache
    {
      public RenderingCachingOptionsCache(string name, long maxSize)
        : base(name, maxSize)
      {
      }

      public virtual void Set(Guid renderingId, RenderingCachingDefinition value)
      {
        SetObject(renderingId.ToString(), value);
      }

      public virtual RenderingCachingDefinition Get(Guid renderingId)
      {
        return GetObject(renderingId.ToString()) as RenderingCachingDefinition;
      }
    }

    public RenderingCachingDefinition GetCachingDefinition(Rendering rendering)
    {
      var entry = RenderingCacheOptions.Get(rendering.UniqueId);
      if (entry != null)
      {
        return entry;
      }

      GetRenderingCachingArgs getRenderingCachingArgs = new GetRenderingCachingArgs
      {
        Rendering = rendering
      };
      CorePipeline.Run("getRenderingCaching", getRenderingCachingArgs);
      RenderingCachingDefinition cachingDefinition = new RenderingCachingDefinition(getRenderingCachingArgs.Rendering);
      RenderingCacheOptions.Set(rendering.UniqueId, cachingDefinition);

      return cachingDefinition;
    }
  }
}