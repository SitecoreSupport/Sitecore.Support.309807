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
        if (value == null)
        {
          SetObject(key, new object()); 
        }
        else
        {
          SetObject(key, value);
        }
      }

      public virtual object Get(string key)
      {
        return GetObject(key);
      }
    }

    public RenderingCachingDefinition GetCachingDefinition(Rendering rendering)
    {
      string key = this.GenerateCacheKey(rendering);
      var entry = RenderingCacheOptions.Get(key);
      if (entry != null)
      {
        return entry as RenderingCachingDefinition;
      }

      GetRenderingCachingArgs getRenderingCachingArgs = new GetRenderingCachingArgs
      {
        Rendering = rendering
      };
      CorePipeline.Run("getRenderingCaching", getRenderingCachingArgs);

      RenderingCacheOptions.Set(key, getRenderingCachingArgs.CachingDefinition);

      return getRenderingCachingArgs.CachingDefinition;
    }

    private string GenerateCacheKey(Rendering rendering)
    {
      return "{0}|{1}|{2}".FormatWith(rendering.Placeholder, rendering.UniqueId, rendering.RenderingItem?.ID);
    }
  }
}