using System;
using UnityEngine;

namespace Invert.Common.UI
{
    public class UFStyle
    {
        private bool _fullWidth = true;
        private TextAnchor _textAnchor = TextAnchor.MiddleCenter;
        private GUIStyle _iconStyle;
        private GUIStyle _backgroundStyle;
        private static GUIStyle _subLabelStyle;

        public UFStyle()
        {
            Enabled = true;
        }

        public UFStyle(string label, GUIStyle backgroundStyle, GUIStyle indicatorStyle = null, GUIStyle optionsStyle = null, Action onShowOptions = null, bool showArrow = true, TextAnchor textAnchor = TextAnchor.MiddleRight)
        {
            Label = label;
            _backgroundStyle = backgroundStyle;
            MarkerStyle = indicatorStyle;
            IconStyle = optionsStyle;
            OnShowOptions = onShowOptions;
            ShowArrow = showArrow;
            _textAnchor = textAnchor;
            Enabled = true;
        }

        public GUIStyle BackgroundStyle
        {
            get { return _backgroundStyle ?? ElementDesignerStyles.EventButtonStyle; }
            set { _backgroundStyle = value; }
        }

        public string SubLabel { get; set; }
        public static GUIStyle MouseDownStyle { get; set; }
        public GUIStyle MarkerStyle { get; set; }

        public GUIStyle IconStyle
        {
            get { return _iconStyle ; }
            set { _iconStyle = value; }
        }

        public Action OnShowOptions { get; set; }
        public string Label { get; set; }

        public bool FullWidth
        {
            get { return _fullWidth; }
            set { _fullWidth = value; }
        }

        public bool IsWindow { get; set; }
        public TextAnchor TextAnchor
        {
            get { return _textAnchor; }
            set { _textAnchor = value; }
        }

        public static GUIStyle SubLabelStyle
        {
            get { return _subLabelStyle ?? ElementDesignerStyles.SubLabelStyle; }
            set { _subLabelStyle = value; }
        }

        public bool ShowArrow { get; set; }
        public bool Enabled { get; set; }
        public string Group { get; set; }
        public object Tag { get; set; }

        public bool IsMouseDown(Rect rect)
        {
            return false;
        }
    }
}