// /****************************************************************************
//  * Copyright (c) 2018 Karsion(拖鞋)
//  * Date: 2018-06-07 18:28
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
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Animation))]
    internal class AnimationInspector : UnityEditor.Editor
    {
        public Animation targetAnimation;
        private AnimationClip[] animationClips = null;

        private void OnEnable()
        {
            targetAnimation = target as Animation;
            animationClips = AnimationUtility.GetAnimationClips(targetAnimation.gameObject);
            Array.Sort(animationClips, (ac1, ac2) => string.Compare(ac1.name, ac2.name, StringComparison.Ordinal));
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            Separator();
            for (int i = 0; i < animationClips.Length; i++)
            {
                if (animationClips[i] == null)
                {
                    continue;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(animationClips[i], typeof(AnimationClip), false);
                string strClipName = animationClips[i].name;
                if (GUILayout.Button(new GUIContent("Get Name", "Copy the clip's name to the clipboard"), EditorStyles.miniButton))
                {
                    TextEditor te = new TextEditor();
                    te.text = strClipName;
                    te.OnFocus();
                    te.Copy();
                }

                GUILayout.Space(24);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (Application.isPlaying)
                {
                    if (targetAnimation.IsPlaying(strClipName))
                    {
                        if (GUILayout.Button("Stop", EditorStyles.miniButton))
                        {
                            targetAnimation.Stop(strClipName);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("> ClampForever", EditorStyles.miniButton))
                        {
                            targetAnimation[strClipName].wrapMode = WrapMode.ClampForever;
                            DoAnimation(strClipName);
                        }

                        if (GUILayout.Button("> Once", EditorStyles.miniButton))
                        {
                            targetAnimation[strClipName].wrapMode = WrapMode.Once;
                            DoAnimation(strClipName);
                        }

                        if (GUILayout.Button("> Loop", EditorStyles.miniButton))
                        {
                            targetAnimation[strClipName].wrapMode = WrapMode.Loop;
                            DoAnimation(strClipName);
                        }
                    }
                }

                GUILayout.Space(24);
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(-4);
                EditorGUILayout.BeginHorizontal();
                AnimationState aniState = targetAnimation[strClipName];
                if (!aniState)
                {
                    EditorGUILayout.HelpBox("This clip is not initialized.! Please run the game", MessageType.Warning);
                    GUILayout.Space(24);
                    EditorGUILayout.EndHorizontal();
                    continue;
                }

                if (aniState.normalizedTime > 0)
                {
                    EditorGUI.ProgressBar(GUILayoutUtility.GetRect(Screen.width - 100, 16), aniState.time/animationClips[i].length, aniState.time.ToString("F3") + " / " + animationClips[i].length.ToString("F3"));
                    Repaint();
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Time:  " + animationClips[i].length.ToString("F3"), GUILayout.Width(100));
                    GUILayout.Label("Frame:  " + (animationClips[i].length*animationClips[i].frameRate).ToString("F2"));
                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.Space(24);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(6);
            }
        }

        private void DoAnimation(string strClipName)
        {
            if (targetAnimation.isPlaying)
            {
                targetAnimation.CrossFade(strClipName);
            }
            else
            {
                targetAnimation.Play(strClipName);
            }
        }

        private static void Separator()
        {
            GUI.color = new Color(1, 1, 1, 0.25f);
            GUILayout.Box("", "HorizontalSlider", GUILayout.Height(16));
            GUI.color = Color.white;
        }
    }
}