using System.Web;
using Sitecore.Caching;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.SecurityModel.License;
using Sitecore.StringExtensions;
using Sitecore.XA.Foundation.Theming.Bundler;
using Sitecore.XA.Foundation.Theming.EventHandlers;

namespace Sitecore.Support.XA.Foundation.Theming.Bundler
{
  public class AssetLinksGeneratorFixed : Sitecore.XA.Foundation.Theming.Bundler.AssetLinksGenerator
  {
    private static AssetsHashCodeCache AssetsCache = new AssetsHashCodeCache("SXA[AssetsHashCodeCache]", StringUtil.ParseSizeString(Settings.GetSetting("XA.Foundation.Presentation.AssetsHashCodeCacheMaxSize", "5MB")));
    public class AssetsHashCodeCache : CustomCache
    {
      public AssetsHashCodeCache(string name, long maxSize)
          : base(name, maxSize)
      {
      }

      public virtual void Set(ID itemId, ID deviceId, int hash)
      {
        SetObject("{0}_{1}".FormatWith(itemId, deviceId), hash);
      }

      public virtual int? Get(ID itemId, ID deviceId)
      {
        return GetObject("{0}_{1}".FormatWith(itemId, deviceId)) as int?;
      }
    }
    public override AssetLinks GenerateAssetLinks(IThemesProvider themesProvider)
    {
      if (!License.HasModule("Sitecore.SXA"))
      {
        HttpContext.Current.Response.Redirect($"{Settings.NoLicenseUrl}?license=Sitecore.SXA");
        return null;
      }

      AssetLinks assetLinks = GetAssetsFromCache();
      if (assetLinks == null)
      {
        assetLinks = base.GenerateAssetLinks(themesProvider);
      }

      return assetLinks;
    }

    private AssetLinks GetAssetsFromCache()
    {
      if (Sitecore.Context.Site.Name == "shell" || !Sitecore.Context.PageMode.IsNormal)
      {
        return null;
      }

      var hash = AssetsCache.Get(Context.Item.ID, Context.Device.ID);
      if (hash == null)
      {
        return null;
      }
      string text = GenerateCacheKey((int)hash);
      AssetLinks assetLinks = HttpContext.Current.Cache[text] as AssetLinks;
      return assetLinks;
    }
    protected override string GenerateCacheKey(int hash)
    {
      if (AssetsCache.Get(Context.Item.ID, Context.Device.ID) == null)
      {
        AssetsCache.Set(Context.Item.ID, Context.Device.ID, hash);
      }

      return base.GenerateCacheKey(hash);
    }

    public new static AssetLinks GenerateLinks(IThemesProvider themesProvider)
    {
      if (AssetContentRefresher.IsPublishing() || IsAddingRendering())
      {
        return new AssetLinks();
      }
      return new AssetLinksGeneratorFixed().GenerateAssetLinks(themesProvider);
    }
  }
}