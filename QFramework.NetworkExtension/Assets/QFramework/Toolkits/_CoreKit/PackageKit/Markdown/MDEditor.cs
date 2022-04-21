/****************************************************************************
 * Copyright (c) 2019 Gwaredd Mountain UNDER MIT License
 * Copyright (c) 2022 liangxiegame UNDER MIT License
 *
 * https://github.com/gwaredd/UnityMarkdownViewer
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    [CustomEditor(typeof(TextAsset))]
    public class MDEditor : Editor
    {
        public GUISkin SkinDark => Resources.Load<GUISkin>("Skin/MarkdownViewerSkin");
        public GUISkin SkinLight => Resources.Load<GUISkin>("Skin/MarkdownSkinQS");

        MDViewer mViewer;

        private static List<string> mExtensions = new List<string> { ".md", ".markdown" };

        protected void OnEnable()
        {
            var content = (target as TextAsset).text;
            var path = AssetDatabase.GetAssetPath(target);

            var ext = Path.GetExtension(path).ToLower();

            if (mExtensions.Contains(ext))
            {
                mViewer = new MDViewer(MDPreferences.DarkSkin ? SkinDark : SkinLight, path, content);
                EditorApplication.update += UpdateRequests;
            }
        }

        protected void OnDisable()
        {
            if (mViewer != null)
            {
                EditorApplication.update -= UpdateRequests;
                mViewer = null;
            }
        }

        void UpdateRequests()
        {
            if (mViewer != null && mViewer.Update())
            {
                Repaint();
            }
        }


        //------------------------------------------------------------------------------

        public override bool UseDefaultMargins()
        {
            return false;
        }

        protected override void OnHeaderGUI()
        {
#if UNITY_2019_2_OR_NEWER && !UNITY_2020_1_OR_NEWER
            // TODO: workaround for bug in 2019.2
            // https://forum.unity.com/threads/oninspectorgui-not-being-called-on-defaultasset-in-2019-2-0f1.724328/
            DrawEditor();
#endif
        }

        public override void OnInspectorGUI()
        {
#if !UNITY_2019_2_OR_NEWER || UNITY_2020_1_OR_NEWER
            DrawEditor();
#endif
        }


        //------------------------------------------------------------------------------

        private Editor mDefaultEditor;

        void DrawEditor()
        {
            if (mViewer != null)
            {
                mViewer.Draw();
            }
            else
            {
                DrawDefaultEditor();
            }
        }

        void DrawDefaultEditor()
        {
            if (mDefaultEditor == null)
            {
                mDefaultEditor = CreateEditor(target, Type.GetType("UnityEditor.TextAssetInspector, UnityEditor"));
            }

            if (mDefaultEditor != null)
            {
                mDefaultEditor.OnInspectorGUI();
            }
        }
    }
}
#endif