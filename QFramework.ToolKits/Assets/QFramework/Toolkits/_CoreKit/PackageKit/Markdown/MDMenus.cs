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
using System.IO;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class MDMenus
    {
        static string GetFilePath( string filename )
        {
            var path = AssetDatabase.GetAssetPath( Selection.activeObject );

            if( string.IsNullOrEmpty( path ) )
            {
                path = "Assets";
            }
            else if( AssetDatabase.IsValidFolder( path ) == false )
            {
                path = Path.GetDirectoryName( path );
            }

            return AssetDatabase.GenerateUniqueAssetPath( path + "/" + filename );
        }

        [MenuItem( "Assets/Create/Markdown" )]
        static void CreateMarkdown()
        {
            var filepath = GetFilePath( "NewMarkdown.md" );
            var writer   = File.CreateText( filepath );

            var template = EditorGUIUtility.Load( "MarkdownTemplate.md" ) as TextAsset;

            if( template != null )
            {
                writer.Write( template.text );
            }
            else
            {
                writer.Write( "# Markdown\n" );
            }

            writer.Close();

            AssetDatabase.ImportAsset( filepath );

            Selection.activeObject = AssetDatabase.LoadAssetAtPath<TextAsset>( filepath );
        }
    }
}
#endif