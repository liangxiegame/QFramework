using UnityEngine;

namespace QFramework
{
    public class UIKitConfig
    {
        public virtual UIRoot Root
        {
            get { return UIRoot.Instance; }
        }

        public virtual IPanel LoadPanel(PanelSearchKeys panelSearchKeys)
        {
            var panelLoader = new DefaultPanelLoader();

            panelLoader.LoadPanelPrefab(panelSearchKeys);

            var panelPrefab = panelLoader.LoadPanelPrefab(panelSearchKeys);

            var obj = Object.Instantiate(panelPrefab);

            var retScript = obj.GetComponent<UIPanel>();

            retScript.As<IPanel>().Loader = panelLoader;

            Debug.Log(retScript.As<IPanel>());

            return retScript;
        }

        /// <summary>
        /// 如果想要定制自己的加载器，自定义 IPanelLoader 以及
        /// </summary>
        public interface IPanelLoader
        {
            GameObject LoadPanelPrefab(PanelSearchKeys panelSearchKeys);

            void Unload();
        }

        /// <summary>
        /// Default
        /// </summary>
        public class DefaultPanelLoader : IPanelLoader
        {
            ResLoader mResLoader = ResLoader.Allocate();

            public GameObject LoadPanelPrefab(PanelSearchKeys panelSearchKeys)
            {
                
                if (panelSearchKeys.PanelType.IsNotNull() && panelSearchKeys.GameObjName.IsNullOrEmpty())
                {
                    return mResLoader.LoadSync<GameObject>(panelSearchKeys.PanelType.Name);
                }
                
                if (panelSearchKeys.AssetBundleName.IsNotNullAndEmpty())
                {
                    return mResLoader.LoadSync<GameObject>(panelSearchKeys.AssetBundleName, panelSearchKeys.GameObjName);
                }

                return mResLoader.LoadSync<GameObject>(panelSearchKeys.GameObjName);
            }

            public void Unload()
            {
                mResLoader.Recycle2Cache();
                mResLoader = null;
            }
        }

        public virtual void SetDefaultSizeOfPanel(IPanel panel)
        {
            var panelRectTrans = panel.Transform.As<RectTransform>();

            panelRectTrans.offsetMin = Vector2.zero;
            panelRectTrans.offsetMax = Vector2.zero;
            panelRectTrans.anchoredPosition3D = Vector3.zero;
            panelRectTrans.anchorMin = Vector2.zero;
            panelRectTrans.anchorMax = Vector2.one;

            panelRectTrans.LocalScaleIdentity();
        }
    }
}