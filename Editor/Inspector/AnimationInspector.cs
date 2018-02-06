/****************************************************************************
 * Copyright (c) 2017 Karsion
****************************************************************************/
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Animation))]
public class ExtendedAnimation : Editor
{
    private AnimationClip[] animationClips = null;
    public Animation targetAnimation;

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
            if (GUILayout.Button("Copy Name", EditorStyles.miniButton))
            {
                TextEditor te = new TextEditor();
                te.text = strClipName;
                te.OnFocus();
                te.Copy();
            }

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
                    if (GUILayout.Button("Play", EditorStyles.miniButton))
                    {
                        targetAnimation.Play(strClipName);
                    }
                }
            }

            EditorGUILayout.EndHorizontal();

            AnimationState aniState = targetAnimation[strClipName];
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

            //EditorGUILayout.Space();
        }
    }

    private static void Separator()
    {
        GUI.color = new Color(1, 1, 1, 0.25f);
        GUILayout.Box("", "HorizontalSlider", GUILayout.Height(16));
        GUI.color = Color.white;
    }
}