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
using System.IO;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    internal class MDHandlerNavigate
    {
        public MDHistory  MDHistory;
        public string   CurrentPath;

        public Action<float>        ScrollTo;
        public Func<string,MDBlock>   FindBlock;

        //------------------------------------------------------------------------------

        public void SelectPage( string url )
        {
            if( string.IsNullOrEmpty( url ) )
            {
                return;
            }

            // internal link

            if( url.StartsWith( "#" ) )
            {
                var block = FindBlock( url.ToLower() );

                if( block != null )
                {
                    ScrollTo( block.Rect.y );
                }
                else
                {
                    Debug.LogWarning( string.Format( "Unable to find section header {0}", url ) );
                }

                return;
            }

            // relative or absolute link ...

            var newPath = string.Empty;

            if( url.StartsWith( "/" ) )
            {
                newPath = url.Substring( 1 );
            }
            else
            {
                newPath = MDUtils.PathCombine( Path.GetDirectoryName( CurrentPath ), url );
            }

            // load file

            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>( newPath );

            if( asset != null )
            {
                MDHistory.Add( newPath );
                Selection.activeObject = asset;
            }
            else
            {
                Debug.LogError( string.Format( "Could not find asset {0}", newPath ) );
            }
        }
    }
}
#endif