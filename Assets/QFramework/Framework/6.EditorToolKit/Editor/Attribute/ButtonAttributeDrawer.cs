// /****************************************************************************
//  * Copyright (c) 2018 Karsion(拖鞋)
//  * 
//  * http://qframework.io
//  * https://github.com/liangxiegame/QFramework
//  * 
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  * 
//  * The above copyright notice and this permission notice shall be included in
//  * all copies or substantial portions of the Software.
//  * 
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  * THE SOFTWARE.
//  ****************************************************************************/

using System.Reflection;
using UnityEditor;
using UnityEngine;
using ButtonAttribute = UnityEngine.ButtonAttribute;

namespace QFramework
{
    [CustomPropertyDrawer(typeof(UnityEngine.ButtonAttribute), true)]
    internal class ButtonAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            UnityEngine.ButtonAttribute attribute = (UnityEngine.ButtonAttribute) this.attribute;
            return attribute.height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            UnityEngine.ButtonAttribute attribute = (UnityEngine.ButtonAttribute) this.attribute;
            if (attribute.funcNames == null || attribute.funcNames.Length == 0)
            {
                position = EditorGUI.IndentedRect(position);
                EditorGUI.HelpBox(position, "[Button] funcNames is Not Set!", MessageType.Warning);
                return;
            }

            //看看应该显示按钮的时机
            bool disabled = false;
            if (attribute.showIfRunTime != ShowIfRunTime.All)
            {
                if (attribute.showIfRunTime == ShowIfRunTime.Playing != Application.isPlaying)
                {
                    disabled = true;
                }
            }

            using (new EditorGUI.DisabledScope(disabled))
            {
                float width = position.width / attribute.funcNames.Length;
                position.width = width;
                for (int i = 0; i < attribute.funcNames.Length; i++)
                {
                    string funcName = attribute.funcNames[i];

                    if (GUI.Button(position, funcName))
                    {
                        CalledFunc(property.serializedObject.targetObject, funcName);
                    }

                    position.x += width;
                }
            }
        }

        private static void CalledFunc(Object target, string strFuncName)
        {
            MethodInfo methodInfo = target.GetType()
                .GetMethod(strFuncName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo == null)
            {
                return;
            }

            Debug.Log(target + ".<b>" + strFuncName + "()</b> Invoke");
            methodInfo.Invoke(target, null);
        }
    }
}