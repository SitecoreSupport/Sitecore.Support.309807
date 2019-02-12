using Sitecore.Caching;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.XA.Foundation.Presentation;
using Sitecore.XA.Foundation.Presentation.Extensions;
using Sitecore.XA.Foundation.SitecoreExtensions.CustomFields;

namespace Sitecore.Support.XA.Foundation.Presentation
{
  public class PresentationContext : Sitecore.XA.Foundation.Presentation.PresentationContext
  {
    public class IsPageDesignCache : CustomCache
    {
      public IsPageDesignCache(string name, long maxSize)
        : base(name, maxSize)
      {
      }

      public virtual void Set(ID id, bool value)
      {
        SetObject(id.ToString(), value);
      }

      public virtual bool? Get(ID id)
      {
        return GetObject(id.ToString()) as bool?;
      }
    }

    private static IsPageDesignCache IsPageDesignItemCache = new IsPageDesignCache("SXA[IsPageDesignItem]",
      StringUtil.ParseSizeString(Settings.GetSetting("XA.Foundation.Presentation.IsPageDesignItemCacheMaxSize",
        "5MB")));

    protected virtual bool IsPageDesign(Item item)
    {
      var cacheRecord = IsPageDesignItemCache.Get(item.ID);
      if (cacheRecord != null)
      {
        return (bool) cacheRecord;
      }

      bool result = item.IsPageDesign();
      IsPageDesignItemCache.Set(item.ID, result);

      return result;
    }

    protected override Item DoGetPageDesignItem(Item item, Item pageDesignsItem)
    {
      if (item == null)
      {
        return null;
      }

      if (this.IsPageDesign(item))
      {
        return item;
      }

      LookupField lookupField = item.Fields[Templates._Designable.Fields.Design];
      if (lookupField != null)
      {
        Item targetItem = lookupField.TargetItem;
        if (targetItem != null && targetItem.IsPageDesign())
        {
          return targetItem;
        }
      }

      if (pageDesignsItem != null)
      {
        MappingField mappingField = pageDesignsItem.Fields[Templates._DesignTemplateMapping.Fields.TemplatesMapping];
        if (mappingField != null)
        {
          Item item2 = mappingField.Lookup(item.TemplateID);
          if (item2 != null && item2.IsPageDesign())
          {
            return item2;
          }
        }
      }

      return null;
    }
  }
}