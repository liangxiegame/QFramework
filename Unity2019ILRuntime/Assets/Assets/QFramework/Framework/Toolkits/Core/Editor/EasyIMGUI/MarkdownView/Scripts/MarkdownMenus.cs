////////////////////////////////////////////////////////////////////////////////

using System.IO;
using UnityEditor;
using UnityEngine;

namespace MG.MDV
{
    public class Menus
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
