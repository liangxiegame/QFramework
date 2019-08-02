using System.Collections.Generic;
using Invert.Common;
using UnityEngine;

public static class uFrameHelpStyles
{

    private static Dictionary<string, Font> FontsCache = new Dictionary<string, Font>();

    public static GUIStyle WithAllStates(this GUIStyle style, string textureName, Color textColor)
    {
        var state = new GUIStyleState() { background = !string.IsNullOrEmpty(textureName) ? ElementDesignerStyles.GetSkinTexture(textureName) : null, textColor = textColor };
        style.normal = style.active = style.hover = style.focused = style.onNormal = style.onActive = style.onHover = style.onFocused = state;
        return style;
    }

    public static GUIStyle WithHoveredState(this GUIStyle style, string textureName, Color textColor)
    {
        var state = new GUIStyleState() { background = !string.IsNullOrEmpty(textureName) ? ElementDesignerStyles.GetSkinTexture(textureName) : null, textColor = textColor };
        style.hover = style.onHover = state;
        return style;
    }

    public static GUIStyle WithAllStates(this GUIStyle style, Texture2D texture, Color textColor)
    {
        var state = new GUIStyleState() { background = texture, textColor = textColor };
        style.normal = style.active = style.hover = style.focused = style.onNormal = style.onActive = style.onHover = style.onFocused = state;
        return style;
    }

    public static GUIStyle WithAllStates(this GUIStyle style, Color textColor)
    {
        var state = new GUIStyleState() { textColor = textColor };
        style.normal = style.active = style.hover = style.focused = style.onNormal = style.onActive = style.onHover = style.onFocused = state;
        return style;
    }

    public static GUIStyle ForAllStates(this GUIStyle style, Texture2D texture)
    {
        style.normal.background = style.active.background = style.hover.background = style.focused.background = style.onNormal.background
            = style.onActive.background = style.onHover.background = style.onFocused.background = texture;
        return style;
    }
   
    public static GUIStyle ForNormalState(this GUIStyle style, Texture2D texture)
    {
        style.normal.background =  texture;
        return style;
    }


    public static GUIStyle WithFont(this GUIStyle style, string fontName, int fontSize)
    {
        //        var key = fontName + fontSize;
        //        if (!FontsCache.ContainsKey(key))
        //        {
        //            FontsCache.Add(key,Font.CreateDynamicFontFromOSFont(fontName,fontSize));
        //        }
        //        style.font = FontsCache[key];


        if (!FontsCache.ContainsKey(fontName))
        {
            FontsCache.Add(fontName, Resources.Load<Font>("fonts/" + fontName));
        }

        style.font = FontsCache[fontName];
        style.fontSize = fontSize;
        return style;
    }
    
    public static GUIStyle WithFont(this GUIStyle style, string fontName)
    {
        //        var key = fontName + fontSize;
        //        if (!FontsCache.ContainsKey(key))
        //        {
        //            FontsCache.Add(key,Font.CreateDynamicFontFromOSFont(fontName,fontSize));
        //        }
        //        style.font = FontsCache[key];


        if (!FontsCache.ContainsKey(fontName))
        {
            FontsCache.Add(fontName, Resources.Load<Font>("fonts/" + fontName));
        }

        style.font = FontsCache[fontName];
        return style;
    }

}