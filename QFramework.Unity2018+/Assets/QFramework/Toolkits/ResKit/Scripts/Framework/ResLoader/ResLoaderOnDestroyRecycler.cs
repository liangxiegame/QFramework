using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class ResLoaderOnDestroyRecycler : MonoBehaviour
    {
        private HashSet<ResLoader> mResLoaders = new HashSet<ResLoader>();

        public void AddResLoader(ResLoader loader)
        {
            mResLoaders.Add(loader);
        }
        
        private void OnDestroy()
        {
            foreach (var resLoader in mResLoaders)
            {
                resLoader.Recycle2Cache();
            }
            
            mResLoaders.Clear();
            mResLoaders = null;
        }
    }

    public static class ResLoaderOnDestroyReleaserExtension
    {
        public static ResLoader RecycleWhenGameObjectDestroyed(this ResLoader self,Component component)
        {
            var recycler = component.GetOrAddComponent<ResLoaderOnDestroyRecycler>();
            recycler.AddResLoader(self);
            return self;
        }
        
        public static ResLoader RecycleWhenGameObjectDestroyed(this ResLoader self,GameObject gameObj)
        {
            var recycler = gameObj.GetOrAddComponent<ResLoaderOnDestroyRecycler>();
            recycler.AddResLoader(self);
            return self;
        }
    }
}