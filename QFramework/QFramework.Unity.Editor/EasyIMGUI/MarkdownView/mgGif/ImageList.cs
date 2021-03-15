using System.Collections.Generic;
using UnityEngine;

namespace MG.GIF
{
    public enum Disposal
    {
        None              = 0x00,
        DoNotDispose      = 0x04,
        RestoreBackground = 0x08,
        ReturnToPrevious  = 0x0C
    }

    public class Image
    {
        public Color32[] RawImage;
        public int       Delay; // ms
        public Disposal  DisposalMethod = Disposal.None;

        protected ImageList mGif;

        public Image( ImageList gif )
        {
            mGif = gif;
        }

        public Texture2D CreateTexture()
        {
            var tex = new Texture2D( mGif.Width, mGif.Height, TextureFormat.ARGB32, false );
            tex.filterMode = FilterMode.Point;
            tex.wrapMode   = TextureWrapMode.Clamp;
            tex.SetPixels32( RawImage );
            tex.Apply();

            return tex;
        }
    }

    public class ImageList
    {
        public string   Version;
        public ushort   Width;
        public ushort   Height;
        public int      BitDepth;

        public List<Image> Images = new List<Image>();

        public void Add( Image img )
        {
            Images.Add( img );
        }

        public Image GetImage( int index )
        {
            return index < Images.Count ? Images[index] : null;
        }

        public int NumFrames
        {
            get
            {
                int count = 0;

                foreach( var img in Images )
                {
                    if( img.Delay > 0 )
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public Image GetFrame( int index )
        {
            if( Images.Count == 0 )
            {
                return null;
            }

            foreach( var img in Images )
            {
                if( img.Delay > 0 )
                {
                    if( index == 0 )
                    {
                        return img;
                    }

                    index--;
                }
            }

            return Images[Images.Count - 1];
        }
    }
}
