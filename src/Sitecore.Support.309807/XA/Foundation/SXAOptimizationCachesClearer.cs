using System;
using Sitecore.Caching;

namespace Sitecore.Support.XA.Foundation
{
  public class CachesClearer
  {
    public void OnPublishEnd(object sender, EventArgs e)
    {
      var cache1 = CacheManager.FindCacheByName<string>("SXA[AssetsHashCodeCache]");
      if (cache1 != null)
        cache1.Clear();

      var cache2 = CacheManager.FindCacheByName<string>("SXA[IsPageDesignItem]");
      if (cache2 != null)
        cache2.Clear();

      var cache3 = CacheManager.FindCacheByName<string>("MVC[RenderingRendererCache]");
      if (cache3 != null)
        cache3.Clear();

      var cache4 = CacheManager.FindCacheByName<string>("SXA[RenderingCachingOptions]");
      if (cache4 != null)
        cache4.Clear();
    }
  }
}