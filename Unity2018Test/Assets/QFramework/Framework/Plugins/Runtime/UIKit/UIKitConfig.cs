/****************************************************************************
 * Copyright (c) 2017 ~ 2020.1 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

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