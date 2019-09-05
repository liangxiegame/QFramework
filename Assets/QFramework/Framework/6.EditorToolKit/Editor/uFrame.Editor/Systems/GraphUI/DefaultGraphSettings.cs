using UnityEngine;

namespace QF.GraphDesigner
{
    public class DefaultGraphSettings : IGraphEditorSettings
    {
        public Color BackgroundColor { get; set; }
        public Color TabTextColor { get; set; }
        public Color SectionTitleColor { get; set; }
        public Color SectionItemColor { get; set; }
        public Color SectionItemTypeColor { get; set; }

        public Color GridLinesColor { get; set; }

        public Color GridLinesColorSecondary { get; set; }

        public bool ShowGraphDebug { get; set; }

        public bool ShowHelp
        {
            get { return false; }
            set
            {
            }
        }

        public bool UseGrid
        {
            get { return true; }
            set { }
        }

        public DefaultGraphSettings()
        {
            BackgroundColor = new Color(0.176f, 0.176f, 0.176f);
            GridLinesColor = new Color(0.153f, 0.153f, 0.153f);
            GridLinesColorSecondary = new Color(0.075f, 0.075f, 0.075f);
        }
    }
}