using UnityEngine;

namespace QF.GraphDesigner
{
    public interface IGraphEditorSettings
    {
        bool UseGrid { get; set; }
        bool ShowHelp { get; set; }
        bool ShowGraphDebug { get; set; }
        Color BackgroundColor { get; set; }
        Color TabTextColor { get; set; }
        Color SectionTitleColor { get; set; }
        Color SectionItemColor { get; set; }
        Color SectionItemTypeColor { get; set; }
        Color GridLinesColor { get; set; }
        Color GridLinesColorSecondary { get; set; }
    }
}
