////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace MG.MDV
{
    public struct Style
    {
        public static readonly Style Default = new Style();

        const int FlagBold      = 0x0100;
        const int FlagItalic    = 0x0200;
        const int FlagFixed     = 0x0400;
        const int FlagLink      = 0x0800;
        const int FlagBlock     = 0x1000;

        const int MaskSize      = 0x000F;
        const int MaskWeight    = 0x0300;

        int mStyle;

        public static bool operator==( Style a, Style b )
        {
            return a.mStyle == b.mStyle;
        }

        public static bool operator!=( Style a, Style b )
        {
            return a.mStyle != b.mStyle;
        }

        public override bool Equals( object a )
        {
            return a is Style ? ( (Style) ( a ) ).mStyle == mStyle : false;
        }

        public override int GetHashCode()
        {
            return mStyle.GetHashCode();
        }


        public void Clear()
        {
            mStyle = 0x0000;
        }

        public bool Bold
        {
            get { return ( mStyle & FlagBold ) != 0x0000; }
            set { if( value ) mStyle |= FlagBold; else mStyle &= ~FlagBold; }
        }

        public bool Italic
        {
            get { return ( mStyle & FlagItalic ) != 0x0000; }
            set { if( value ) mStyle |= FlagItalic; else mStyle &= ~FlagItalic; }
        }

        public bool Fixed
        {
            get { return ( mStyle & FlagFixed ) != 0x0000; }
            set { if( value ) mStyle |= FlagFixed; else mStyle &= ~FlagFixed; }
        }

        public bool Link
        {
            get { return ( mStyle & FlagLink ) != 0x0000; }
            set { if( value ) mStyle |= FlagLink; else mStyle &= ~FlagLink; }
        }

        public bool Block
        {
            get { return ( mStyle & FlagBlock ) != 0x0000; }
            set { if( value ) mStyle |= FlagBlock; else mStyle &= ~FlagBlock; }
        }

        public int Size
        {
            get { return mStyle & MaskSize; }
            set { mStyle = ( mStyle & ~MaskSize ) | UnityEngine.Mathf.Clamp( value, 0, 6 ); }
        }

        public FontStyle GetFontStyle()
        {
            switch( mStyle & MaskWeight )
            {
                case FlagBold:              return FontStyle.Bold;
                case FlagItalic:            return FontStyle.Italic;
                case FlagBold | FlagItalic: return FontStyle.BoldAndItalic;
                default:                    return FontStyle.Normal;
            }
        }
    }
}

