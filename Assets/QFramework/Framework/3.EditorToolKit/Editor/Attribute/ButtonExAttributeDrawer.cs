// /****************************************************************************
//  * Copyright (c) 2018 Karsion(拖鞋)
//  * Date: 2018-06-07 18:29
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

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFramework
{
    internal class ButtonExAttributeDrawer
    {
        private readonly Object target;
        private readonly List<MethodInfo> buttonMethods;
        private readonly List<ButtonExAttribute> buttonAttributes;

        public ButtonExAttributeDrawer(Object target)
        {
            if (!target)
            {
                return;
            }

            this.target = target;
            MethodInfo[] methodInfos = target.GetType().GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (methodInfos.Length <= 0)
            {
                return;
            }

            buttonMethods = new List<MethodInfo>();
            buttonAttributes = new List<ButtonExAttribute>();
            for (int i = 0; i < methodInfos.Length; i++)
            {
                MethodInfo buttonMethod = methodInfos[i];
                if (Attribute.IsDefined(buttonMethod, typeof(ButtonExAttribute), true))
                {
                    buttonMethods.Add(buttonMethod);
                    ButtonExAttribute[] exAttributes = buttonMethod.GetCustomAttributes(typeof(ButtonExAttribute), true) as ButtonExAttribute[];
                    buttonAttributes.Add(exAttributes[0]);
                }
            }
        }

        public void OnInspectorGUI()
        {
            if (buttonMethods.Count == 0)
            {
                return;
            }

            for (int index = 0; index < buttonMethods.Count; index++)
            {
                MethodInfo methodInfo = buttonMethods[index];
                ButtonExAttribute buttonExAttribute = buttonAttributes[index];
                bool disabled = false;
                if (buttonExAttribute.showIfRunTime != ShowIfRunTime.All)
                {
                    if (buttonExAttribute.showIfRunTime == ShowIfRunTime.Playing != Application.isPlaying)
                    {
                        disabled = true;
                    }
                }

                using (new EditorGUI.DisabledScope(disabled))
                {
                    string name = buttonExAttribute.txtButtonName ?? methodInfo.Name;
                    if (GUILayout.Button(name))
                    {
                        methodInfo.Invoke(target, null);
                    }
                }
            }
        }
    }
}