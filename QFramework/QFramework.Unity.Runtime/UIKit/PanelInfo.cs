/****************************************************************************
 * Copyright (c) 2018.7 ~ 2021.3 liangxie
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

using System;

namespace QFramework
{
    public class PanelInfo : IPoolType, IPoolable
    {
        public IUIData UIData;

        public UILevel Level = UILevel.Common;

        public string AssetBundleName;

        public string GameObjName;

        public Type PanelType;

        public static PanelInfo Allocate(string gameObjName, UILevel level, IUIData uiData, Type panelType,
            string assetBundleName)
        {
            var panelInfo = SafeObjectPool<PanelInfo>.Instance.Allocate();

            panelInfo.GameObjName = gameObjName;
            panelInfo.Level = level;
            panelInfo.UIData = uiData;
            panelInfo.PanelType = panelType;
            panelInfo.AssetBundleName = assetBundleName;
            return panelInfo;
        }

        public void Recycle2Cache()
        {
            SafeObjectPool<PanelInfo>.Instance.Recycle(this);
        }

        public void OnRecycled()
        {
            UIData = null;
            AssetBundleName = null;
            GameObjName = null;
            PanelType = null;
        }

        public bool IsRecycled { get; set; }
    }
}