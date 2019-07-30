using Sitecore.Data.Items;
using Sitecore.Layouts;
using Sitecore.Mvc.Presentation;
using Sitecore.XA.Foundation.Presentation;
using Sitecore.XA.Foundation.Presentation.Extensions;
using Sitecore.XA.Foundation.Presentation.Pipelines.GetRenderingCaching;
using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;

namespace Sitecore.Support.XA.Foundation.Presentation.Pipelines.GetRenderingCaching
{
  public class GetSiteLevelCaсhing
  {
    private readonly IPresentationContext _presentationContext;

    public GetSiteLevelCaсhing(IPresentationContext presentationContext)
    {
      _presentationContext = presentationContext;
    }

    public void Process(GetRenderingCachingArgs args)
    {
      Item item = _presentationContext.PresentationItem?.FirstChildInheritingFrom(Templates.CacheSettingsFolder.ID);
      if (item != null)
      {
        string value = args.Rendering.RenderingItem?.ID.ToString() ?? args.Rendering.RenderingItemPath;
        foreach (Item child in item.Children)
        {
          if (child.InheritsFrom(Templates.CacheSettings.ID) && child[Templates.CacheSettings.Fields.Renderings].Contains(value))
          {
            PopulateCacheOptions(args.Rendering, child);
            args.Rendering["InheritedCaching"] = "1";
            args.CachingDefinition = args.Rendering.Caching;
            break;
          }
        }
      }
    }

    protected virtual void PopulateCacheOptions(Rendering rendering, Item cacheSettingsItem)
    {
      if (cacheSettingsItem.InheritsFrom(Templates.Caching.ID))
      {
        RenderingCaching renderingCaching = RenderingCaching.Parse(cacheSettingsItem);
        rendering.SetCachingOptions(renderingCaching);
      }
    }
  }
}