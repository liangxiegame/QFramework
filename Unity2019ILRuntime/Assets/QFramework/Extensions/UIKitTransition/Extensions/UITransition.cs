/****************************************************************************
 * Copyright (c) 2018.5 ~ 9 liangxie
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

namespace QFramework
{
    using System;
    using UnityEngine.UI;

    public static class UITransitionExtension
    {
        public static void DoTransition<TDstPanel>(this UIPanel selfBehaviour, UITransition transition,
            UILevel uiLevel = UILevel.Common,
            IUIData uiData = null,
            string assetBundleName = null,
            string prefabName = null) where TDstPanel : UIPanel
        {
            transition.FromPanel = selfBehaviour;
            transition.InCompleted = () =>
            {
                UIKit.OpenPanel<TDstPanel>(uiLevel, uiData, assetBundleName, prefabName);
            };
            

            UIKit.OpenPanel<UITransitionPanel>(UILevel.Forward, new UITransitionPanelData()
            {
                Transition = transition
            }, prefabName: "Resources/UITransitionPanel");
        }

        /// <summary>
        /// 绑定跳转逻辑
        /// </summary>
        /// <param name="selfPanel"></param>
        /// <param name="btn"></param>
        /// <typeparam name="T"></typeparam>
        public static void BindTransition<TSrcPanel, TDstPanel>(this UnityEngine.UI.Button btn) where TSrcPanel : UIPanel
            where TDstPanel : UIPanel
        {
            btn.onClick.AddListener(() =>
            {
                UIKit.ClosePanel<TSrcPanel>();
                UIKit.OpenPanel<TDstPanel>();
            });
        }
        
        /// <summary>
        /// 绑定跳转逻辑
        /// </summary>
        /// <param name="selfPanel"></param>
        /// <param name="btn"></param>
        /// <typeparam name="T"></typeparam>
        public static System.Action Transition<TDstPanel>(this UIPanel selfBehaviour,IUIData uidata = null) where TDstPanel : UIPanel
        {
            return () =>
            {
                UIKit.ClosePanel(selfBehaviour.name);
                UIKit.OpenPanel<TDstPanel>(uidata);
            };
        }

        // TODO:这里要想办法抽象下
        public static void Do(this System.Action action)
        {
            action();
        }
        
        public static void Start(this System.Action action)
        {
            action();
        }
        
        public static void Begin(this System.Action action)
        {
            action();
        }
    }
}