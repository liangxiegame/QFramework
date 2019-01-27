/****************************************************************************
 * Copyright 2019.1 liangxie
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

using QFramework.Editor;
using UnityEngine;

namespace QFramework
{
    public class UIKitSettingView : GUIView,IPackageKitView
    {
        private FrameworkSettingData mFrameworkSettingData;
		
        public UIKitSettingView()
        {
            mFrameworkSettingData = FrameworkSettingData.Load();
        }

        public IQFrameworkContainer Container { get; set; }
        public int RenderOrder
        {
            get { return 0; }
        }
        
        public bool Ignore { get; private set; }

        public bool Enabled
        {
            get { return true; }
        }

        public void Init(IQFrameworkContainer container)
        {
            
        }

        public override void OnGUI()
        {
            base.OnGUI();
            GUILayout.Label("UI Kit Settings:");
            GUILayout.BeginVertical("box");

            mFrameworkSettingData.Namespace = EditorGUIUtils.GUILabelAndTextField("Namespace", mFrameworkSettingData.Namespace);
            mFrameworkSettingData.UIScriptDir =
                EditorGUIUtils.GUILabelAndTextField("UI Script Generate Dir", mFrameworkSettingData.UIScriptDir);
            mFrameworkSettingData.UIPrefabDir =
                EditorGUIUtils.GUILabelAndTextField("UI Prefab Dir", mFrameworkSettingData.UIPrefabDir);
			
            if (GUILayout.Button("Apply"))
            {
                mFrameworkSettingData.Save();
            }
			
            GUILayout.EndVertical();
        }
    }
}