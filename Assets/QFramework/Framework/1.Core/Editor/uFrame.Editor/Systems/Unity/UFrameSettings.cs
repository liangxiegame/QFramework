using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QF.GraphDesigner;
using UnityEditor;
using UnityEngine;

namespace QF.GraphDesigner.Unity
{


    public class UFrameSettings : IGraphEditorSettings
    {
        private Color? _gridLinesColor;
        private Color? _backgroundColor;
        private Color? _gridLinesColorSecondary;
        private Color? _tabTextColor;
        private Color? _sectionItemColor;
        private Color? _sectionTitleColor;

        public void SetColorPref(string name, Color value)
        {
            EditorPrefs.SetFloat(name + "R", value.r);
            EditorPrefs.SetFloat(name + "G", value.g);
            EditorPrefs.SetFloat(name + "B", value.b);
            EditorPrefs.SetFloat(name + "A", value.a);
        }

        public Color GetColorPref(string name, Color def)
        {
            var r =EditorPrefs.GetFloat(name + "R", def.r);
            var g =EditorPrefs.GetFloat(name + "G", def.g);
            var b =EditorPrefs.GetFloat(name + "B", def.b);
            var a = EditorPrefs.GetFloat(name + "A", def.a);
            return new Color(r,g,b,a);
        }

        
        public virtual Color TabTextColor
        {
            get
            {
                if (_tabTextColor == null)
                {
                    return (_tabTextColor = GetColorPref("_tabTextColor", new Color(0.8f, 0.8f, 0.8f))).Value;
                }
                return _tabTextColor.Value;
            }
            set
            {
                _tabTextColor = value;
                SetColorPref("_tabTextColor", value);
            }
        }

        public Color SectionTitleColor
        {
            get
            {
                if (_sectionTitleColor == null)
                {
                    return (_sectionTitleColor = GetColorPref("_sectionTitleColor", new Color(0.78f, 0.78f, 0.78f))).Value;
                }
                return _sectionTitleColor.Value;
            }
            set
            {
                _sectionTitleColor = value;
                SetColorPref("_sectionTitleColor", value);
            }
        }

        public virtual Color SectionItemColor
        {
            get
            {
                if (_sectionItemColor == null)
                {
                    return (_sectionItemColor = GetColorPref("_sectionItemColor", new Color(0.65f, 0.65f, 0.65f))).Value;
                }
                return _sectionItemColor.Value;
            }
            set
            {
                _sectionItemColor = value;
                SetColorPref("_sectionItemColor", value);
            }
        }

        public Color SectionItemTypeColor { get; set; }

        public virtual Color GridLinesColor
        {
            get
            {
                if (_gridLinesColor == null)
                {
                    return (_gridLinesColor = GetColorPref("_gridLinesColor", new Color(0.1f, 0.1f, 0.1f))).Value;
                }
                return _gridLinesColor.Value;
            }
            set
            {
                _gridLinesColor = value;
                SetColorPref("_gridLinesColor", value);
            }
        }
        public virtual Color GridLinesColorSecondary
        {
            get
            {
                if (_gridLinesColorSecondary == null)
                {
                    return (_gridLinesColorSecondary = GetColorPref("_gridLinesColorSecondary", new Color(0.08f, 0.08f, 0.08f))).Value;
                }
                return _gridLinesColorSecondary.Value;
            }
            set
            {
                _gridLinesColorSecondary = value;
                SetColorPref("_gridLinesColorSecondary", value);
            }
        }
        public virtual Color BackgroundColor
        {
            get
            {
                if (_backgroundColor == null)
                {
                    return (_backgroundColor = GetColorPref("_backgroundColor", new Color(0.13f, 0.13f, 0.13f))).Value;
                }
                return _backgroundColor.Value;
            }
            set
            {
                _backgroundColor = value;
                SetColorPref("_backgroundColor", value);
            }
        }
        
        public virtual bool UseGrid
        {
            get { return Convert.ToBoolean(PlayerPrefs.GetInt("UseGrid", Convert.ToInt32(true))); }
            set
            {
                PlayerPrefs.SetInt("UseGrid",Convert.ToInt32(value));
            }
        }
        public virtual bool ShowHelp
        {
            get { return Convert.ToBoolean(PlayerPrefs.GetInt("ShowHelp", Convert.ToInt32(true))); }
            set
            {
                PlayerPrefs.SetInt("ShowHelp", Convert.ToInt32(value));
            }
        }

        public virtual bool ShowGraphDebug
        {
            get { return Convert.ToBoolean(PlayerPrefs.GetInt("ShowGraphDebug", Convert.ToInt32(false))); }
            set
            {
                PlayerPrefs.SetInt("ShowGraphDebug", Convert.ToInt32(value));
            }
        }
     
    }


}
