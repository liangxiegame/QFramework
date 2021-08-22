using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using QFramework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace MG.MDV
{
    public class HandlerImages
    {
        public string  CurrentPath;

        Texture                     mPlaceholder      = null;
        List<ImageRequest>          mActiveRequests   = new List<ImageRequest>();
        Dictionary<string,Texture>  mTextureCache     = new Dictionary<string, Texture>();
        List<AnimatedTexture>       mAnimatedTextures = new List<AnimatedTexture>();

        class AnimatedTexture
        {
            public string           URL          = string.Empty;
            public int              CurrentFrame = 0;
            public double           FrameTime    = 0.0f;
            public List<Texture2D>  Textures     = new List<Texture2D>();
            public List<float>      Times        = new List<float>();

            public AnimatedTexture( string url )
            {
                URL       = url;
                FrameTime = EditorApplication.timeSinceStartup;
            }

            public void Add( Texture2D tex, float delay )
            {
                Textures.Add( tex );
                Times.Add( delay );
            }

            public bool Update()
            {
                var span = EditorApplication.timeSinceStartup - FrameTime;

                if( span < Times[ CurrentFrame ] )
                {
                    return false;
                }

                FrameTime = EditorApplication.timeSinceStartup;
                CurrentFrame = ( CurrentFrame + 1 ) % Textures.Count;

                return true;
            }
        }

        class ImageRequest
        {
            public string           URL; // original url
            public UnityWebRequest  Request;
            public bool             IsGif;

            public ImageRequest( string url )
            {
                URL = url;

                if( url.EndsWith( ".gif", System.StringComparison.OrdinalIgnoreCase ) )
                {
                    IsGif   = true;
                    Request = UnityWebRequest.Get( url );
                }
                else
                {
                    IsGif   = false;
                    Request = UnityWebRequestTexture.GetTexture( url );
                }

                Request.SendWebRequest();
            }

            public AnimatedTexture GetAnimatedTexture()
            {
                var decoder = new MG.GIF.Decoder( Request.downloadHandler.data );
                var img     = decoder.NextImage();
                var anim    = new AnimatedTexture( URL );

                while( img != null )
                {
                    anim.Add( img.CreateTexture(), img.Delay / 1000.0f );
                    img = decoder.NextImage();
                }

                return anim;
            }

            public Texture GetTexture()
            {
                var handler = Request.downloadHandler as DownloadHandlerTexture;
                return handler != null ? handler.texture : null;
            }
        }


        //------------------------------------------------------------------------------

        private string RemapURL( string url )
        {
            if( Regex.IsMatch( url, @"^\w+:", RegexOptions.Singleline ) )
            {
                return url;
            }

            var projectDir = Path.GetDirectoryName( Application.dataPath );

            if( url.StartsWith( "/" ) )
            {
                return string.Format( "file:///{0}{1}", projectDir, url );
            }

            var assetDir = Path.GetDirectoryName( CurrentPath );
            return "file:///" + Utils.PathNormalise( string.Format( "{0}/{1}/{2}", projectDir, assetDir, url ) );
        }

        //------------------------------------------------------------------------------

        public Texture FetchImage( string url )
        {
            url = RemapURL( url );

            Texture tex;

            if( mTextureCache.TryGetValue( url, out tex ) )
            {
                return tex;
            }

            if( mPlaceholder == null )
            {
                var style = GUI.skin.GetStyle( "btnPlaceholder" );
                mPlaceholder = style != null ? style.normal.background : null;
            }

            mActiveRequests.Add( new ImageRequest( url ) );
            mTextureCache[ url ] = mPlaceholder;

            return mPlaceholder;
        }

        //------------------------------------------------------------------------------

        public bool UpdateRequests()
        {
            var req = mActiveRequests.Find( r => r.Request.isDone );

            if( req == null )
            {
                return false;
            }

#if UNITY_2020_2_OR_NEWER
            if( req.Request.result == UnityWebRequest.Result.ProtocolError )
#else
            if( req.Request.isHttpError )
#endif
            {
                Log.E( string.Format( "HTTP Error: {0} - {1} {2}", req.URL, req.Request.responseCode, req.Request.error ) );
                mTextureCache[ req.URL ] = null;
            }
#if UNITY_2020_2_OR_NEWER
            else if( req.Request.result == UnityWebRequest.Result.ConnectionError )
#else
            else if( req.Request.isNetworkError )
#endif
            {
               Log.E( string.Format( "Network Error: {0} - {1}", req.URL, req.Request.error ) );
                mTextureCache[ req.URL ] = null;
            }
            else if( req.IsGif )
            {
                var anim = req.GetAnimatedTexture();

                if( anim != null && anim.Textures.Count > 0 )
                {
                    mTextureCache[ req.URL ] = anim.Textures[ 0 ];

                    if( anim.Textures.Count > 1 )
                    {
                        mAnimatedTextures.Add( anim );
                    }
                }
            }
            else
            {
                mTextureCache[ req.URL ] = req.GetTexture();
            }

            mActiveRequests.Remove( req );
            return true;
        }


        //------------------------------------------------------------------------------

        public bool UpdateAnimations()
        {
            var update = false;

            foreach( var anim in mAnimatedTextures )
            {
                if( anim.Update() )
                {
                    mTextureCache[ anim.URL ] = anim.Textures[ anim.CurrentFrame ];
                    update = true;
                }
            }

            return update;
        }


        //------------------------------------------------------------------------------

        public bool Update()
        {
            return UpdateRequests() || UpdateAnimations();
        }
    }
}
