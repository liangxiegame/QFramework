/****************************************************************************
 * Copyright (c) 2018 vin129
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
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
namespace QFramework {
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class QUIPanelStack : ISingleton
    {
        public void OnSingletonInit()
        {
            Log.I("QUIPanelStack init");
        }

        private QUIPanelStack(){}

        private static QUIPanelStack mInstance;

        public static QUIPanelStack Instance
        {
            get
            {
                if (null == mInstance)
                {
                    mInstance = QSingletonProperty<QUIPanelStack>.Instance;
                }
               
                return mInstance;
            }
        }

        /// <summary>
        /// 按layer入栈
        /// </summary>
        /// <param name="uilevel"></param>
        /// <param name="ui"></param>
        public void OnCreatUI(int uilevel, IUIBehaviour ui,string uiBehaviourName)
        {
            if (Push(uilevel, ui))
                mAllUIStackSite.Add(uiBehaviourName,uilevel);
        }

        public void RemoveUI(string uiName,IUIBehaviour ui) {
            var uilevel = GetUILevel(uiName);
            if (mAllStack.ContainsKey(uilevel)) {
                mAllStack[uilevel].Remove(ui);
                mAllUIStackSite.Remove(uiName);
            }      
        }

        public int GetUILevel(string uiName){
            return mAllUIStackSite[uiName];
        }

        public void CloseLastUI(int uilevel) {
            IUIBehaviour ui = GetLastUI(uilevel);
            if (ui != null)
                QUIManager.Instance.CloseUI(ui.Transform.gameObject.name);
        }

        public void ClearStack(){
            mAllStack.Clear();
            mAllUIStackSite.Clear();
        }



        private bool Push(int uilevel, IUIBehaviour ui) {
            if (!mAllStack.ContainsKey(uilevel))
                mAllStack.Add(uilevel, new List<IUIBehaviour>());
            if (!mAllStack[uilevel].Contains(ui))
            {
                mAllStack[uilevel].Add(ui);
                return true;
            }
            return false;
        }

        private IUIBehaviour GetLastUI(int uilevel) {
            if (mAllStack.ContainsKey(uilevel))
            {
                var count = mAllStack[uilevel].Count;
                if(count > 0)
                    return mAllStack[uilevel][count - 1] ;
            }
            return null;
        }




        #region property
        Dictionary<int, List<IUIBehaviour>> mAllStack = new Dictionary<int, List<IUIBehaviour>>();
        Dictionary<string, int> mAllUIStackSite = new Dictionary<string, int>();
        #endregion
    }
}
