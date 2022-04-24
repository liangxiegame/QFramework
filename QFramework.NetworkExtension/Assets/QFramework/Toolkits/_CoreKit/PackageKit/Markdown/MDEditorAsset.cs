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
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    [CustomEditor( typeof( MDAsset ) )]
    public class MDEditorAsset : Editor
    {
        public GUISkin SkinDark => Resources.Load<GUISkin>("Skin/MarkdownViewerSkin");
        public GUISkin SkinLight => Resources.Load<GUISkin>("Skin/MarkdownSkinQS");

        MDViewer mViewer;

        protected void OnEnable()
        {
            var content = ( target as MDAsset ).text;
            var path    = AssetDatabase.GetAssetPath( target );

            mViewer = new MDViewer( MDPreferences.DarkSkin ? SkinDark : SkinLight, path, content );
            EditorApplication.update += UpdateRequests;
        }

        protected void OnDisable()
        {
            EditorApplication.update -= UpdateRequests;
            mViewer = null;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }

        protected override void OnHeaderGUI()
        {
            //base.OnHeaderGUI(); 
        }

        public override void OnInspectorGUI()
        {
            mViewer.Draw();
        }


        void UpdateRequests()
        {
            if( mViewer.Update() )
            {
                Repaint();
            }
        }
    }
}
#endif