using System.Collections.Generic;
using UnityEngine;

namespace QF.GraphDesigner
{
    public abstract class BreadcrumbsStyleSchema : IBreadcrumbsStyleSchema
    {

        private static Dictionary<IconCacheItem,object> IconsCache = new Dictionary<IconCacheItem,object>();

        public object GetIcon(string name, Color tint = default(Color))
        {
            var item = new IconCacheItem()
            {
                Name = name,
                TintColor = tint
            };
            if (IconsCache.ContainsKey(item) && ((Equals(IconsCache[item], null)) || IconsCache[item].Equals(null))) IconsCache.Remove(item);
            if (!IconsCache.ContainsKey(item)) IconsCache.Add(item,ConstructIcon(name,tint));
            return IconsCache[item];
        }

        protected abstract object ConstructIcon(string name, Color tint = default(Color));

    }
}