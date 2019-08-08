using QF.GraphDesigner;
using UnityEngine;

namespace QF.GraphDesigner {

    public interface INodeStyleSchema
    {

        // Computed Styles, evaluated based on the look and feel settings
        object CollapsedHeaderStyleObject { get; }
        object ExpandedHeaderStyleObject { get; }
        object TitleStyleObject { get; }
        object SubTitleStyleObject { get; }

        // Look and feel settings
        bool ShowTitle { get; }
        bool ShowSubtitle { get; }
        bool ShowIcon { get; }
        int TitleFontSize { get; }
        int SubTitleFontSize { get; }
        Color TitleColor { get; }
        Color SubTitleColor { get; }
        string TitleFont { get; }
        string SubTitleFont { get; }
        string HeaderImage { get; }
        NodeColor HeaderColor { get; }
        FontStyle TitleFontStyle { get; }
        FontStyle SubTitleFontStyle { get; }
        RectOffset HeaderPadding { get; }

        //Fancy bells and whistles
        INodeStyleSchema RecomputeStyles();
        INodeStyleSchema Clone();
        INodeStyleSchema WithHeaderColor(NodeColor color);
        INodeStyleSchema WithHeaderImage(string image);
        INodeStyleSchema WithTitleFont(string font, int? fontsize, Color? color, FontStyle? style);
        INodeStyleSchema WithSubTitleFont(string font, int? fontsize, Color? color, FontStyle? style);
        INodeStyleSchema WithTitle(bool shown);
        INodeStyleSchema WithSubTitle(bool shown);
        INodeStyleSchema WithIcon(bool shown);
        INodeStyleSchema WithHeaderPadding(RectOffset padding);

        object GetHeaderImage(bool expanded, Color color = default(Color), string iconName = null);
        object GetIconImage(string iconName, Color color = default(Color));
    }

}
