using System;
using System.Collections.Generic;
using Sitecore.Caching;
using Sitecore.Configuration;
using Sitecore.Mvc.Pipelines.Response.RenderPlaceholder;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Support.XA.Foundation.SitecoreExtensions.Pipelines.RenderPlaceholder
{
  public class PerformRendering : Sitecore.XA.Foundation.SitecoreExtensions.Pipelines.RenderPlaceholder.PerformRendering
  {
    private static RenderingRendererCache RenderersCache = new RenderingRendererCache("MVC[RenderingRendererCache]", StringUtil.ParseSizeString(Settings.GetSetting("MVC.Presentation.RenderingRendererCacheMaxSize", "5MB")));
    public class RenderingRendererCache : CustomCache
    {
      public RenderingRendererCache(string name, long maxSize)
        : base(name, maxSize)
      {
      }

      public virtual void Set(Guid renderingUniqueId, Renderer renderer)
      {
        SetObject(renderingUniqueId.ToString(), renderer);
      }

      public virtual Renderer Get(Guid renderingUniqueId)
      {
        return GetObject(renderingUniqueId.ToString()) as Renderer;
      }
    }
    protected override IEnumerable<Rendering> GetRenderings(string placeholderName, RenderPlaceholderArgs args)
    {
      var renderings = base.GetRenderings(placeholderName, args);
      if (Context.Site.Name != "shell" && Context.PageMode.IsNormal)
      {
        foreach (var rendering in renderings)
        {
          var renderer = RenderersCache.Get(rendering.UniqueId);
          if (renderer == null)
          {
            renderer = rendering.Renderer;
            RenderersCache.Set(rendering.UniqueId, renderer);
          }

          rendering.Renderer = renderer;
        }
      }

      return renderings;
    }
  }
}