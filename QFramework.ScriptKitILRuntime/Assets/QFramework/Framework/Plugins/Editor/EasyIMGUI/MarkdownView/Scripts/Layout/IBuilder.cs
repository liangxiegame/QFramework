////////////////////////////////////////////////////////////////////////////////

namespace MG.MDV
{
    public interface IBuilder
    {
        void Text( string text, Style style, string link, string tooltip );
        void Image( string url, string alt, string tooltip );

        void NewLine();
        void Space();
        void HorizontalLine();

        void Indent();
        void Outdent();
        void Prefix( string text, Style style );

        void StartBlock( bool quote );
        void EndBlock();
    }
}
