using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System;
using System.Linq;
using System.Reflection;

namespace UI.Xml
{
    public enum RectAlignment
    {
        UpperCenter,
        MiddleCenter,
        LowerCenter,

        UpperLeft,
        MiddleLeft,
        LowerLeft,

        UpperRight,
        MiddleRight,
        LowerRight        
    }

    public enum ButtonIconAlignment
    {
        Left,
        Right
    }

    public enum HideAnimation
    {
        None,
        SlideOut_Top,
        SlideOut_Bottom,
        SlideOut_Left,
        SlideOut_Right,
        FadeOut,
        Shrink,
        Shrink_Vertical,
        Shrink_Horizontal
    }

    public enum ShowAnimation
    {
        None,
        SlideIn_Top,
        SlideIn_Bottom,
        SlideIn_Left,
        SlideIn_Right,
        FadeIn,
        Grow,
        Grow_Vertical,
        Grow_Horizontal
    }

    public enum SlideDirection
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public enum ParseXmlResult
    {
        Changed,
        Unchanged,
        Failed
    }

    public static class EnumExtensions
    {
        public static bool IsSlideAnimation(this HideAnimation hideAnimation)
        {
            switch (hideAnimation)
            {
                case HideAnimation.SlideOut_Bottom:
                case HideAnimation.SlideOut_Top:
                case HideAnimation.SlideOut_Left:
                case HideAnimation.SlideOut_Right:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsSlideAnimation(this ShowAnimation showAnimation)
        {
            switch (showAnimation)
            {
                case ShowAnimation.SlideIn_Bottom:
                case ShowAnimation.SlideIn_Top:
                case ShowAnimation.SlideIn_Left:
                case ShowAnimation.SlideIn_Right:
                    return true;
                default:
                    return false;
            }
        }

        public static SlideDirection ToSlideDirection(this HideAnimation hideAnimation)
        {
            switch (hideAnimation)
            {
                case HideAnimation.SlideOut_Bottom: return SlideDirection.Bottom;
                case HideAnimation.SlideOut_Top: return SlideDirection.Top;
                case HideAnimation.SlideOut_Left: return SlideDirection.Left;
                case HideAnimation.SlideOut_Right: return SlideDirection.Right;
                default: return SlideDirection.Top;
            }
        }

        public static SlideDirection ToSlideDirection(this ShowAnimation showAnimation)
        {
            switch (showAnimation)
            {
                case ShowAnimation.SlideIn_Bottom: return SlideDirection.Bottom;
                case ShowAnimation.SlideIn_Top: return SlideDirection.Top;
                case ShowAnimation.SlideIn_Left: return SlideDirection.Left;
                case ShowAnimation.SlideIn_Right: return SlideDirection.Right;
                default: return SlideDirection.Top;
            }
        }
    }
}
