/****************************************************************************
 * Copyright (c) 2021.3 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
    public struct MarkdownStyle
    {
        public static readonly MarkdownStyle Default = new MarkdownStyle();

        const int FlagBold = 0x0100;
        const int FlagItalic = 0x0200;
        const int FlagFixed = 0x0400;
        const int FlagLink = 0x0800;
        const int FlagBlock = 0x1000;

        const int MaskSize = 0x000F;
        const int MaskWeight = 0x0300;

        int mStyle;

        public static bool operator ==(MarkdownStyle a, MarkdownStyle b)
        {
            return a.mStyle == b.mStyle;
        }

        public static bool operator !=(MarkdownStyle a, MarkdownStyle b)
        {
            return a.mStyle != b.mStyle;
        }

        public override bool Equals(object a)
        {
            return a is MarkdownStyle ? ((MarkdownStyle) (a)).mStyle == mStyle : false;
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
            get { return (mStyle & FlagBold) != 0x0000; }
            set
            {
                if (value) mStyle |= FlagBold;
                else mStyle &= ~FlagBold;
            }
        }

        public bool Italic
        {
            get { return (mStyle & FlagItalic) != 0x0000; }
            set
            {
                if (value) mStyle |= FlagItalic;
                else mStyle &= ~FlagItalic;
            }
        }

        public bool Fixed
        {
            get { return (mStyle & FlagFixed) != 0x0000; }
            set
            {
                if (value) mStyle |= FlagFixed;
                else mStyle &= ~FlagFixed;
            }
        }

        public bool Link
        {
            get { return (mStyle & FlagLink) != 0x0000; }
            set
            {
                if (value) mStyle |= FlagLink;
                else mStyle &= ~FlagLink;
            }
        }

        public bool Block
        {
            get { return (mStyle & FlagBlock) != 0x0000; }
            set
            {
                if (value) mStyle |= FlagBlock;
                else mStyle &= ~FlagBlock;
            }
        }

        public int Size
        {
            get { return mStyle & MaskSize; }
            set { mStyle = (mStyle & ~MaskSize) | UnityEngine.Mathf.Clamp(value, 0, 6); }
        }

        public FontStyle GetFontStyle()
        {
            switch (mStyle & MaskWeight)
            {
                case FlagBold: return FontStyle.Bold;
                case FlagItalic: return FontStyle.Italic;
                case FlagBold | FlagItalic: return FontStyle.BoldAndItalic;
                default: return FontStyle.Normal;
            }
        }
    }
}