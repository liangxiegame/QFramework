using System;
using System.IO;
using QFramework;
using UnityEditor;
using UnityEngine;

namespace MG.MDV
{
    public class HandlerNavigate
    {
        public History  History;
        public string   CurrentPath;

        public Action<float>        ScrollTo;
        public Func<string,Block>   FindBlock;

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
                    Log.W( string.Format( "Unable to find section header {0}", url ) );
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
                newPath = Utils.PathCombine( Path.GetDirectoryName( CurrentPath ), url );
            }

            // load file

            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>( newPath );

            if( asset != null )
            {
                History.Add( newPath );
                Selection.activeObject = asset;
            }
            else
            {
                Log.E( string.Format( "Could not find asset {0}", newPath ) );
            }
        }
    }
}
