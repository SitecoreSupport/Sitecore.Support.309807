using System;
using System.Collections.Generic;
using Sitecore.Caching;
using Sitecore.Configuration;
using Sitecore.Data;
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

      public virtual void Set(ID renderingItemId, Renderer renderer)
      {
        SetObject(renderingItemId.ToString(), renderer);
      }

      public virtual Renderer Get(ID renderingItemId)
      {
        return GetObject(renderingItemId.ToString()) as Renderer;
      }
    }
    protected override IEnumerable<Rendering> GetRenderings(string placeholderName, RenderPlaceholderArgs args)
    {
      var renderings = base.GetRenderings(placeholderName, args);
      if (Context.Site.Name != "shell" && Context.PageMode.IsNormal)
      {
        foreach (var rendering in renderings)
        {
          var renderer = RenderersCache.Get(rendering.RenderingItem.ID);
          if (renderer == null)
          {
            renderer = rendering.Renderer;
            RenderersCache.Set(rendering.RenderingItem.ID, renderer);
          }

          rendering.Renderer = renderer;
        }
      }

      return renderings;
    }
  }
}