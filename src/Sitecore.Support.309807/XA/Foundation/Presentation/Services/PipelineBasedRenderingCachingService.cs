using System;
using Sitecore.Caching;
using Sitecore.Configuration;
using Sitecore.Mvc.Presentation;
using Sitecore.Pipelines;
using Sitecore.StringExtensions;
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

      public virtual void Set(string key, RenderingCachingDefinition value)
      {
        SetObject(key, value);
      }

      public virtual RenderingCachingDefinition Get(string key)
      {
        return GetObject(key) as RenderingCachingDefinition;
      }
    }

    public RenderingCachingDefinition GetCachingDefinition(Rendering rendering)
    {
      string key = this.GenerateCacheKey(rendering);
      var entry = RenderingCacheOptions.Get(key);
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
      RenderingCacheOptions.Set(key, cachingDefinition);

      return cachingDefinition;
    }
    private string GenerateCacheKey(Rendering rendering)
    {
      return "{0}|{1}|{2}".FormatWith(rendering.Placeholder, rendering.UniqueId, rendering.RenderingItem?.ID);
    }
  }
}