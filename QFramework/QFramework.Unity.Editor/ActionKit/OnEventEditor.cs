/****************************************************************************
 * Copyright (c) 2020.10 liangxie
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
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QFramework
{
    [UnityEditor.CustomEditor(typeof(ActionKitEvent), true)]
    public class ActionKitEventEditor : EasyInspectorEditor
    {
        private ActionKitEvent mOnStart = null;

        private void OnEnable()
        {
            mOnStart = target as ActionKitEvent;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            for (var i = 0; i < mOnStart.ActionsDatas.Count; i++)
            {
                GUILayout.BeginVertical("box");

                var type = ActionTypeDB.GetTypeByFullName(mOnStart.ActionsDatas[i].ActionName);
                var obj = JsonUtility.FromJson(mOnStart.ActionsDatas[i].AcitonData, type);

                GUILayout.BeginHorizontal("box");
                GUILayout.Label(type.Name);

                GUILayout.FlexibleSpace();

                if (i != 0 && GUILayout.Button("Up", GUILayout.Width(40)))
                {
                    var previousActionData = mOnStart.ActionsDatas[i - 1];
                    mOnStart.ActionsDatas[i - 1] = mOnStart.ActionsDatas[i];
                    mOnStart.ActionsDatas[i] = previousActionData;

                    Save();
                    break;
                }

                if (i != mOnStart.ActionsDatas.Count - 1 && GUILayout.Button("Down", GUILayout.Width(40)))
                {
                    var nextActionData = mOnStart.ActionsDatas[i + 1];
                    mOnStart.ActionsDatas[i + 1] = mOnStart.ActionsDatas[i];
                    mOnStart.ActionsDatas[i] = nextActionData;


                    Save();
                    break;
                }

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    mOnStart.ActionsDatas.RemoveAt(i);

                    UnityEditor.EditorUtility.SetDirty(target);
                    UnityEditor.SceneManagement.EditorSceneManager
                        .MarkSceneDirty(SceneManager.GetActiveScene());

                    GUIUtility.ExitGUI();
                    break;
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginVertical();

                foreach (var fieldInfo in type.GetFields())
                {
                    if (fieldInfo.GetCustomAttributes(false).Cast<SerializeField>().Any())
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(fieldInfo.Name);

                        if (fieldInfo.FieldType.IsEnum)
                        {
                            var oldValue = (Enum) fieldInfo.GetValue(obj);

                            var value = UnityEditor.EditorGUILayout.EnumPopup(oldValue);

                            if (!Equals(oldValue, value))
                            {
                                fieldInfo.SetValue(obj, value);

                                UnityEditor.EditorUtility.SetDirty(target);
                                UnityEditor.SceneManagement.EditorSceneManager
                                    .MarkSceneDirty(SceneManager.GetActiveScene());
                            }
                        }
                        else if (fieldInfo.FieldType == typeof(string) || fieldInfo.FieldType == typeof(float) ||
                                 fieldInfo.FieldType == typeof(int))
                        {
                            var oldValue = fieldInfo.GetValue(obj).ToString();
                            var value = GUILayout.TextField(oldValue);

                            if (oldValue != value)
                            {
                                if (fieldInfo.FieldType == typeof(string))
                                {
                                    fieldInfo.SetValue(obj, value);
                                }
                                else if (fieldInfo.FieldType == typeof(float))
                                {
                                    fieldInfo.SetValue(obj, float.Parse(value));
                                }
                                else if (fieldInfo.FieldType == typeof(int))
                                {
                                    fieldInfo.SetValue(obj, int.Parse(value));
                                }

                                mOnStart.ActionsDatas[i].AcitonData = JsonUtility.ToJson(obj);

                                EditorUtility.SetDirty(target);
                                UnityEditor.SceneManagement.EditorSceneManager
                                    .MarkSceneDirty(SceneManager.GetActiveScene());
                            }
                        }
                        else if (fieldInfo.FieldType == typeof(bool))
                        {
                            var oldValue = bool.Parse(fieldInfo.GetValue(obj).ToString());
                            var value = GUILayout.Toggle(oldValue, "");

                            if (oldValue != value)
                            {
                                fieldInfo.SetValue(obj, value);

                                mOnStart.ActionsDatas[i].AcitonData = JsonUtility.ToJson(obj);

                                EditorUtility.SetDirty(target);
                                UnityEditor.SceneManagement.EditorSceneManager
                                    .MarkSceneDirty(SceneManager.GetActiveScene());
                            }
                        }


                        GUILayout.EndHorizontal();
                    }
                }

                GUILayout.EndVertical();

                GUILayout.EndVertical();
            }

            if (GUILayout.Button("+"))
            {
                ActionBrowser.Open((type) =>
                {
                    mOnStart.ActionsDatas.Add(new ActionData()
                    {
                        ActionName = type.FullName,
                        AcitonData = JsonUtility.ToJson(Activator.CreateInstance(type))
                    });

                    UnityEditor.EditorUtility.SetDirty(target);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                });
            }
        }
    }
}