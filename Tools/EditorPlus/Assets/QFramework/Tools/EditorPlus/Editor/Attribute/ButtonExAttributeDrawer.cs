// /****************************************************************************
//  * Copyright (c) 2021 Karsion(拖鞋)
//  * Date: 2021-09-01 09:56
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

namespace QFramework
{
    internal class ButtonExAttributeDrawer
	{
		private readonly SerializedObject serializedObject;
		private readonly List<MethodInfo> buttonMethods;
		private readonly List<ButtonExAttribute> buttonAttributes;

		public ButtonExAttributeDrawer(SerializedObject serializedObject)
		{
			this.serializedObject = serializedObject;
			MethodInfo[] methodInfos = serializedObject.targetObject.GetType().GetMethods(BindingFlags.Static |
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			if (methodInfos.Length <= 0) return;

			buttonMethods = new List<MethodInfo>();
			buttonAttributes = new List<ButtonExAttribute>();
			for (int i = 0; i < methodInfos.Length; i++)
			{
				MethodInfo buttonMethod = methodInfos[i];
				Type tButtonExAttribute = typeof(ButtonExAttribute);
				if (Attribute.IsDefined(buttonMethod, tButtonExAttribute, true))
				{
					buttonMethods.Add(buttonMethod);
					ButtonExAttribute[] exAttributes =
						buttonMethod.GetCustomAttributes(tButtonExAttribute, true) as ButtonExAttribute[];
					buttonAttributes.Add(exAttributes[0]);
				}
			}
		}

		public void OnInspectorGUI()
		{
			if (buttonMethods.Count == 0) return;

			for (int index = 0; index < buttonAttributes.Count; index++)
			{
				MethodInfo methodInfo = buttonMethods[index];
				ButtonExAttribute buttonExAttribute = buttonAttributes[index];
				bool disabled = false;
				if (buttonExAttribute.showIfRunTime != ShowIfRunTime.All)
				{
					if (buttonExAttribute.showIfRunTime == ShowIfRunTime.Playing != Application.isPlaying)
						disabled = true;
				}

				using (new EditorGUI.DisabledScope(disabled))
				{
					GUILayout.BeginHorizontal();
					string name = methodInfo.Name;
					if (GUILayout.Button(name))
					{
						for (int i = 0; i < serializedObject.targetObjects.Length; i++)
						{
							methodInfo.Invoke(serializedObject.targetObjects[i], null);
						}

						//methodInfo.Invoke(target, null);
					}

					for (int i = 0; i < buttonExAttribute.funcNames.Length; i++)
					{
						string funcName = buttonExAttribute.funcNames[i];
						if (GUILayout.Button(funcName))
						{
							for (int j = 0; j < serializedObject.targetObjects.Length; j++)
							{
								ButtonAttributeDrawer.CalledFunc(serializedObject.targetObjects[j], funcName);
							}
						}
					}

					GUILayout.EndHorizontal();
				}
			}
		}
	}
}