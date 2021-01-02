using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;

namespace UI.Xml
{
    public static class ConversionExtensions
    {
        private static CultureInfo _CultureInfo;
        static CultureInfo CultureInfo
        {
            get
            {
                if (_CultureInfo == null)
                {
                    _CultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();

                    // This ensures that we use decimal points as a float separator regardless of what culture info is current
                    _CultureInfo.NumberFormat.NumberDecimalSeparator = ".";
                }

                return _CultureInfo;
            }
        }

        public static T ChangeToType<T>(this string str)
        {
            return (T)str.ChangeToType(typeof(T));
        }

        public static object ChangeToType(this string str, Type type)
        {
            if (String.IsNullOrEmpty(str) || str.ToLower() == "none")
            {
                return null;
            }

            // Special cases : Enums & specific types
            if (type.IsEnum)
            {
                return Enum.Parse(type, str, true);
            }
            
            switch (type.Name)
            {
                case "RectOffset":
                    return str.ToRectOffset();

                case "Vector2":
                    return str.ToVector2();

                case "Vector3":
                    return str.ToVector3();
                
                case "Boolean":
                case "bool":
                    return str.ToBoolean();

                case "Color":
                    return str.ToColor();

                case "ColorBlock":
                    return str.ToColorBlock();

                case "Sprite":
                    return str.ToSprite();

                case "Quaternion":
                    return str.ToQuaternion();

                case "Font":
                    return str.ToFont();

                case "AudioClip":
                    return str.ToAudioClip();

#if DATEPICKER_PRESENT
                case "SerializableDate":
                    return new UI.Dates.SerializableDate(DateTime.Parse(str));
#endif
            }

            // Special handling for float lists
            if (typeof(IEnumerable<float>).IsAssignableFrom(type))
            {
                return GetFloatList(str);//.Select(i => (Single)i).ToList();
            }

            // Default behaviour
            return Convert.ChangeType(str, type, CultureInfo);
        }

        public static RectOffset ToRectOffset(this string str)
        {
            var rectArray = GetIntList(str);

            int left = 0, right = 0, top = 0, bottom = 0;

            left = rectArray[0];
            right = rectArray.Count > 1 ? rectArray[1] : left;
            top = rectArray.Count > 2 ? rectArray[2] : right;
            bottom = rectArray.Count > 3 ? rectArray[3] : top;

            return new RectOffset(left, right, top, bottom);
        }

        public static bool ToBoolean(this string str)
        {
            var s = str.ToLower();

            return s == "true" || s == "1";
        }

        public static Vector2 ToVector2(this string str)
        {
            var vectorArray = GetFloatList(str);

            float x = 0, y = 0;

            x = vectorArray[0];
            y = (vectorArray.Count > 1) ? vectorArray[1] : x;

            return new Vector2(x, y);
        }

        public static Vector3 ToVector3(this string str)
        {
            var vectorArray = GetFloatList(str);

            float x = 0, y = 0, z = 0;

            x = vectorArray[0];           
            y = vectorArray.Count > 1 ? vectorArray[1] : x;

            if (vectorArray.Count > 2) z = vectorArray[2];

            return new Vector3(x, y, z);
        }

        public static Color ToColor(this string str)
        {
            str = str.ToLower();

            // TODO

            Regex regex;
            MatchCollection matches;

            // a) HTML code
            if (str.StartsWith("#"))
            {
                return HexStringToColor(str);
            }

            // b) RGB / RGBA
            if (str.StartsWith("rgb"))
            {                
                regex = new Regex(@"(\d+[.]\d+)|(\d+)");               
                matches = regex.Matches(str);

                if (matches.Count >= 3)
                {

                    float r = GetColorValue(matches[0].Value);
                    float g = GetColorValue(matches[1].Value);
                    float b = GetColorValue(matches[2].Value);

                    float a = 1f;
                    if (matches.Count == 4)
                    {
                        a = GetColorValue(matches[3].Value);
                    }

                    return new Color(r, g, b, a);
                }
                else
                {
                    Debug.LogWarning("[XmlLayout] Warning: '" + str + "' is not a valid Color value.");
                }
            }

            // c) By name
            var propertyInfo = typeof(Color).GetProperty(str);
            if (propertyInfo != null && propertyInfo.PropertyType == typeof(Color))
            {
                return (Color)propertyInfo.GetValue(null, null);
            }

            Debug.LogWarning("[XmlLayout] Warning: '" + str + "' is not a valid Color value.");

            // default
            return Color.clear;
        }

        private static float GetColorValue(string match)
        {            
            return float.Parse(match, CultureInfo.InvariantCulture);
        }
        
        public static Color HexStringToColor(string hex)
        {     
            hex = hex.Replace ("0x", "");
            hex = hex.Replace ("#", "");
            
            if (hex.Length < 6) return Color.clear;

            byte a = 255;
            byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
            
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(6,2), System.Globalization.NumberStyles.HexNumber);                
            }

             return ((Color)new Color32(r,g,b,a));
        }

        public static List<int> GetIntList(string str)
        {
            return str.Trim('(', ')')
                      .Split(' ', 'x', ',')
                      .Select(s =>
                        {
                            int result = 0;
                            int.TryParse(s.Trim(), out result);
                            return result;
                        })
                      .ToList();
        }

        public static List<float> GetFloatList(string str)
        {
            return str.Trim('(', ')')
                      .Split(' ', 'x', ',')                      
                      .Select(s =>
                      {
                          float result = 0;
                          float.TryParse(s.Trim(), out result);
                          return result;
                      })
                      .ToList();
        }

        public static List<Color> GetColorList(string str)
        {
            return str.Split(' ', 'x', '|')
                      .Select(s => s.ToColor())                      
                      .ToList();
        }

        public static ColorBlock ToColorBlock(this string str)
        {
            var colorBlock = new ColorBlock();
            colorBlock.normalColor = colorBlock.disabledColor = colorBlock.pressedColor = Color.white;
            colorBlock.disabledColor = new Color(1, 1, 1, 0.5f);
            colorBlock.colorMultiplier = 1;

            var colorList = GetColorList(str);

            colorBlock.normalColor = colorList[0];
            if (colorList.Count > 1) colorBlock.highlightedColor = colorList[1];
            if (colorList.Count > 2) colorBlock.pressedColor = colorList[2];
            if (colorList.Count > 3) colorBlock.disabledColor = colorList[3];

            return colorBlock;
        }

        public static Sprite ToSprite(this string str)
        {
            if (String.IsNullOrEmpty(str) || str.ToLower() == "none")
            {
                return null;
            }

            var sprite = XmlLayoutUtilities.LoadResource<Sprite>(str);

            if (sprite == null)
            {
                Debug.LogError("[XmlLayout] Unable to load sprite '" + str + "'. Please ensure that it is located within a Resources folder.");
            }

            return sprite;
        }

        public static Quaternion ToQuaternion(this string str)
        {
            var floatArray = GetFloatList(str);

            Quaternion result = new Quaternion();

            if (floatArray.Count >= 4)
            {
                return new Quaternion(floatArray[0], floatArray[1], floatArray[2], floatArray[3]);
            }
            else
            {
                var x = floatArray[0];
                var y = floatArray.Count > 1 ? floatArray[1] : 0;
                var z = floatArray.Count > 2 ? floatArray[2] : 0;

                result.eulerAngles = new Vector3(x, y, z);                
            }

            return result;
        }

        public static Font ToFont(this string str)
        {            
            var font = XmlLayoutUtilities.LoadResource<Font>("Fonts/" + str);
            if (font == null) font = XmlLayoutUtilities.LoadResource<Font>(str);

            if (font == null)
            {
                Debug.LogWarning("Font '" + str + "' not found. Please ensure that it is located within a Resources folder. (Reverting to Arial)");

                return Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;                             
            }            

            return font;
        }

        public static RuntimeAnimatorController ToRuntimeAnimatorController(this string str)
        {
            if (str.ToLower() == "none")
            {
                return null;
            }

            var animationController = XmlLayoutUtilities.LoadResource<RuntimeAnimatorController>(str);

            if (animationController == null)
            {
                Debug.Log("Animation Controller '" + str + "' not found. Please ensure that it is located within a Resources folder.");
            }

            return animationController;
        }

        public static AudioClip ToAudioClip(this string str)
        {
            if (str.ToLower() == "none")
            {
                return null;
            }

            var audioClip = XmlLayoutUtilities.LoadResource<AudioClip>(str);

            if (audioClip == null)
            {
                Debug.Log("Audio Clip '" + str + "' not found. Please ensure that it is located within a Resources folder.");
            }

            return audioClip;
        }        
    }
}
