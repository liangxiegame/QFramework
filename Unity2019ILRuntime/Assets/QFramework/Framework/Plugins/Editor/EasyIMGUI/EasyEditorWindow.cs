/****************************************************************************
 * Copyright (c) 2018 ~ 2020.10 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
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
using UnityEditor;

namespace QFramework
{

    public abstract class EasyEditorWindow : EditorWindow,IMGUILayoutRoot
    {
        public static T Create<T>(bool utility, string title = null) where T : EasyEditorWindow
        {
            return string.IsNullOrEmpty(title) ? GetWindow<T>(utility) : GetWindow<T>(utility, title);
        }
        public bool Openning { get; set; }
        
        public void Open()
        {
            Openning = true;
            EditorApplication.update += OnUpdate;
            Show();
        }

        public new void Close()
        {
            Openning = false;
            base.Close();
        }
        

        public void RemoveAllChildren()
        {
            this.GetLayout().Clear();
        }

        public abstract void OnClose();


        public abstract void OnUpdate();

        private void OnDestroy()
        {
            EditorApplication.update -= OnUpdate;
            OnClose();
        }

        protected abstract void Init();

        private bool mInited = false;

        public virtual void OnGUI()
        {
            if (!mInited)
            {
                Init();
                mInited = true;
            }
            
            this.GetLayout().DrawGUI();
        }
        
        VerticalLayout IMGUILayoutRoot.Layout { get; set; }
        RenderEndCommandExecutor IMGUILayoutRoot.RenderEndCommandExecutor { get; set; }
    }
    
    [Obsolete("已经弃用，请使用 EasyEditorWindow",true)]
    public abstract class IMGUIEditorWindow : EasyEditorWindow {}
    
     
    
}
