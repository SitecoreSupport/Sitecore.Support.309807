using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Sitecore.Caching;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Pipelines.Response.RenderPlaceholder;
using Sitecore.Mvc.Presentation;
using Sitecore.StringExtensions;

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

      public virtual void Set(string key, Renderer renderer)
      {
        SetObject(key, renderer);
      }

      public virtual Renderer Get(string key)
      {
        return GetObject(key) as Renderer;
      }
    }
    protected override IEnumerable<Rendering> GetRenderings(string placeholderName, RenderPlaceholderArgs args)
    {
      var renderings = base.GetRenderings(placeholderName, args);
      if (Context.Site.Name != "shell" && Context.PageMode.IsNormal)
      {
        foreach (var rendering in renderings)
        {
          string text = rendering.Properties["RenderingXml"];
          if (!string.IsNullOrEmpty(text))
          {
            var renderingReference = new RenderingReference(XElement.Parse(text).ToXmlNode(), Context.Language, args.PageContext.Database);
            if (renderingReference.Settings != null)
            {
              if (!rendering.Caching.Cacheable)
              {
                continue;
              }
              var rulesList = renderingReference.Settings.Rules;
              if (rulesList != null && rulesList.Count > 0)
              {
                continue;
              }

              if (!string.IsNullOrEmpty(renderingReference.Settings.MultiVariateTest))
              {
                continue;
              }

              if (!string.IsNullOrEmpty(renderingReference.Settings.PersonalizationTest) &&
                  !ID.IsNullOrEmpty(ID.Parse(renderingReference.Settings.PersonalizationTest)))
              {
                continue;
              }
            }
          }
          string key = this.GenerateCacheKey(rendering);
          var renderer = RenderersCache.Get(key);
          if (renderer == null)
          {
            renderer = rendering.Renderer;
            RenderersCache.Set(key, renderer);
          }

          rendering.Renderer = renderer;
        }
      }

      return renderings;
    }
    private string GenerateCacheKey(Rendering rendering)
    {

      return "{0}|{1}".FormatWith(rendering.UniqueId, rendering.RenderingItem?.ID);
    }
  }
}