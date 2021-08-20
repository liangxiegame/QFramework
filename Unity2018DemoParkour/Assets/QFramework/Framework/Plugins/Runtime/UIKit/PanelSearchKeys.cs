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

using System;
using UnityEngine;

namespace QFramework
{
    public class PanelSearchKeys : IPoolType, IPoolable
    {
        public Type PanelType;

        public string AssetBundleName;

        public string GameObjName;

        public UILevel Level = UILevel.Common;

        public IUIData UIData;
        
        
        public IPanel Panel;
        
        public PanelOpenType OpenType = PanelOpenType.Single;


        public void OnRecycled()
        {
            PanelType = null;
            AssetBundleName = null;
            GameObjName = null;
            UIData = null;
            Panel = null;
        }

        public bool IsRecycled { get; set; }


        public override string ToString()
        {
            return "PanelSearchKeys PanelType:{0} AssetBundleName:{1} GameObjName:{2} Level:{3} UIData:{4}".FillFormat(PanelType, AssetBundleName, GameObjName, Level,
                UIData);
        }

        public static PanelSearchKeys Allocate()
        {
            return SafeObjectPool<PanelSearchKeys>.Instance.Allocate();
        }

        public void Recycle2Cache()
        {
            SafeObjectPool<PanelSearchKeys>.Instance.Recycle(this);
        }
    }
}