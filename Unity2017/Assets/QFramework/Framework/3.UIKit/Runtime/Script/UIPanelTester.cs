/****************************************************************************
 * Copyright (c) 2018.5 ~ 8 liangxie
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
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    [System.Serializable]
    public class UIPanelTesterInfo
    {
        /// <summary>
        /// 页面的名字
        /// </summary>
        public string PanelName;

        /// <summary>
        /// 层级名字
        /// </summary>
        public UILevel Level;
    }

    public class UIPanelTester : MonoBehaviour
    {
        /// <summary>
        /// 页面的名字
        /// </summary>
        public string PanelName;

        /// <summary>
        /// 层级名字
        /// </summary>
        public UILevel Level;

        [SerializeField] private List<UIPanelTesterInfo> mOtherPanels;

        private void Awake()
        {
            ResMgr.Init();
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.2f);
			
            UIKit.OpenPanel(PanelName, Level);

            mOtherPanels.ForEach(panelTesterInfo => { UIKit.OpenPanel(panelTesterInfo.PanelName, panelTesterInfo.Level); });
        }
    }
}