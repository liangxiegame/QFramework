using UnityEngine;

namespace Invert.Common
{
    public static class TexturesExtensions
    {

        public static Texture2D CutTextureBottomBorder(this Texture2D texture, int cutSize)
        {
            var newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false, true);
            var pixels = texture.GetPixels32();
            var markerRow = cutSize;

            Color32[] newPixels = new Color32[texture.width * texture.height];

            for (var row = 0; row < texture.height; row++)
            {
                for (int pix = 0; pix < texture.width; pix++)
                {
                    if (row > markerRow)
                    {
                        newPixels[row * texture.width + pix] = pixels[row * texture.width + pix];
                    }
                    else
                    {
                        newPixels[row * texture.width + pix] = pixels[markerRow * texture.width + pix];
                    }
                }
            }

            newTexture.SetPixels32(newPixels);
            newTexture.Apply();

            return newTexture;
        }

        public static Texture2D Tint(this Texture2D texture, Color color)
        {
            var newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false, true);
            var pixels = texture.GetPixels();

            Color[] newPixels = new Color[texture.width * texture.height];

            for (var row = 0; row < texture.height; row++)
            {
                for (int pix = 0; pix < texture.width; pix++)
                {
                    var pixel = pixels[row * texture.width + pix];
                    var r = 1f - pixel.r;
                    var g = 1f - pixel.g;
                    var b = 1f - pixel.b;
                    var a = 1f - pixel.a;
                    var newPixel = new Color(color.r - r, color.g - g, color.b - b, color.a - a);
                    newPixels[row * texture.width + pix] = newPixel;
                }
            }

            newTexture.SetPixels(newPixels);
            newTexture.Apply();

            return newTexture;
        }



        public static Texture2D Rotate90(this Texture2D texture)
        {
            var newTexture = new Texture2D(texture.height, texture.width, TextureFormat.RGBA32, false, true);
            var pixels = texture.GetPixels();

            Color[] newPixels = new Color[texture.width * texture.height];


            for (var row = 0; row < texture.height; row++)
            {
                for (var pix = 0; pix < texture.width; pix++)
                {
                    var pixel = pixels[row * texture.width + pix];
                    newPixels[(pix*texture.height) + row] = pixel;
                }
            }


            newTexture.SetPixels(newPixels);
            newTexture.Apply();

            return newTexture;
        }
        

        public static Texture2D Rotate90CW(this Texture2D texture)
        {
            var newTexture = new Texture2D(texture.height, texture.width, TextureFormat.RGBA32, false, true);
            var pixels = texture.GetPixels();

            Color[] newPixels = new Color[texture.width * texture.height];


            for (var row = 0; row < texture.height; row++)
            {
                for (var pix = 0; pix < texture.width; pix++)
                {
                    var pixel = pixels[row * texture.width + pix];
                    newPixels[(pix*texture.height) + row] = pixel;
                }
            }


            newTexture.SetPixels(newPixels);
            newTexture.Apply();

            return newTexture;
        }
        
        public static Texture2D Rotate180(this Texture2D texture)
        {
            var newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false, true);
            var pixels = texture.GetPixels();

            Color[] newPixels = new Color[texture.width * texture.height];

            for (var row = 0; row < texture.height; row++)
            {
                for (var pix = 0; pix < texture.width; pix++)
                {
                    var pixel = pixels[row * texture.width + pix];
                    newPixels[((texture.height-row-1) * texture.width) + (texture.width - 1 - pix)] = pixel;
                }
            }


            newTexture.SetPixels(newPixels);
            newTexture.Apply();

            return newTexture;
        }
        
        public static Texture2D Gradient(this Texture2D texture, Color colorSource, Color colorDestination)
        {
            var newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false, true);
            var pixels = texture.GetPixels();

            var progress = 0f;

            Color[] newPixels = new Color[texture.width * texture.height];

            for (var row = 0; row < texture.height; row++)
            {
                progress = (float)row / texture.height;
                Color color = Color.Lerp(colorSource, colorDestination, progress);
                for (int pix = 0; pix < texture.width; pix++)
                {

                    var pixel = pixels[row * texture.width + pix];

                    if (pixel.a == 0)
                    {
                        newPixels[row * texture.width + pix] = pixel; 
                    }
                    else
                    {
                        var r = 1f - pixel.r;
                        var g = 1f - pixel.g;
                        var b = 1f - pixel.b;
                        var a = 1f - pixel.a;
                        var newPixel = new Color(color.r - r, color.g - g, color.b - b, color.a - a);
                        newPixels[row * texture.width + pix] = newPixel; 
                    }
       
                }
            }

            newTexture.SetPixels(newPixels);
            newTexture.Apply();

            return newTexture;
        }


    }
}