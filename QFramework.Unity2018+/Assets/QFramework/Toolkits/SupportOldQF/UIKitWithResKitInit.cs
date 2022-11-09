using System;
using UnityEngine;

namespace QFramework
{
    public class UIKitWithResKitInit
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            UIKit.Config.PanelLoaderPool = new ResKitPanelLoaderPool();
        }
    }

    public class ResKitPanelLoaderPool : AbstractPanelLoaderPool
    {
        public class ResKitPanelLoader : IPanelLoader
        {
            private ResLoader mResLoader;

            public GameObject LoadPanelPrefab(PanelSearchKeys panelSearchKeys)
            {
                if (mResLoader == null)
                {
                    mResLoader = ResLoader.Allocate();
                }

                if (panelSearchKeys.PanelType.IsNotNull() && panelSearchKeys.GameObjName.IsNullOrEmpty())
                {
                    return mResLoader.LoadSync<GameObject>(panelSearchKeys.PanelType.Name);
                }

                if (panelSearchKeys.AssetBundleName.IsNotNullAndEmpty())
                {
                    return mResLoader.LoadSync<GameObject>(panelSearchKeys.AssetBundleName,
                        panelSearchKeys.GameObjName);
                }

                return mResLoader.LoadSync<GameObject>(panelSearchKeys.GameObjName);
            }

            public void LoadPanelPrefabAsync(PanelSearchKeys panelSearchKeys, Action<GameObject> onLoad)
            {
                if (mResLoader == null)
                {
                    mResLoader = ResLoader.Allocate();
                }

                if (panelSearchKeys.PanelType.IsNotNull() && panelSearchKeys.GameObjName.IsNullOrEmpty())
                {
                    mResLoader.Add2Load<GameObject>(panelSearchKeys.PanelType.Name, (success, res) =>
                    {
                        if (success)
                        {
                            onLoad(res.Asset as GameObject);
                        }
                    });
                    mResLoader.LoadAsync();
                    return;
                }

                if (panelSearchKeys.AssetBundleName.IsNotNullAndEmpty())
                {
                    mResLoader.Add2Load<GameObject>(panelSearchKeys.AssetBundleName, panelSearchKeys.GameObjName,
                        (success, res) =>
                        {
                            if (success)
                            {
                                onLoad(res.Asset as GameObject);
                            }
                        });
                    mResLoader.LoadAsync();
                    return;
                }

                mResLoader.Add2Load<GameObject>(panelSearchKeys.GameObjName, (success, res) =>
                {
                    if (success)
                    {
                        onLoad(res.Asset as GameObject);
                    }
                });
                mResLoader.LoadAsync();
            }

            public void Unload()
            {
                mResLoader?.Recycle2Cache();
                mResLoader = null;
            }
        }

        protected override IPanelLoader CreatePanelLoader()
        {
            return new ResKitPanelLoader();
        }
    }
}