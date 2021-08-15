/****************************************************************************
 * Copyright 2021.3 liangxie
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
using UnityEditor;
using UnityEngine.UI;

namespace QFramework
{
    public class UIEditorWindow : EditorWindow
    {
        // [MenuItem("QFramework/CreateUIRoot", priority = 600)]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(UIEditorWindow));
        }


        void OnEnable()
        {
            GameObject uiManager = GameObject.Find("UIRoot");

            if (uiManager)
            {
                uiManager.GetComponent<UIManager>();
            }
        }

        void OnGUI()
        {
            titleContent.text = "CreateUIRoot";

            EditorGUILayout.BeginVertical();

            UIManagerGUI();

            EditorGUILayout.EndVertical();
        }

        bool isFoldUImanager = false;
        public Vector2 m_referenceResolution = new Vector2(960, 640);
        public CanvasScaler.ScreenMatchMode m_MatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

        public bool m_isOnlyUICamera = false;
        public bool m_isVertical = false;

        void UIManagerGUI()
        {
            EditorGUI.indentLevel = 0;
            isFoldUImanager = EditorGUILayout.Foldout(isFoldUImanager, "UIRoot:");
            if (isFoldUImanager)
            {
                EditorGUI.indentLevel = 1;
                m_referenceResolution = EditorGUILayout.Vector2Field("参考分辨率", m_referenceResolution);
                m_isOnlyUICamera = EditorGUILayout.Toggle("只有一个UI摄像机", m_isOnlyUICamera);
                m_isVertical = EditorGUILayout.Toggle("是否竖屏", m_isVertical);

                if (GUILayout.Button("创建UIRoot"))
                {
                    UICreateService.CreatUIManager(m_referenceResolution, m_MatchMode, m_isOnlyUICamera, m_isVertical);
                }

            }
        }
    }
}