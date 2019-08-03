/****************************************************************************
 * Copyright (c) 2019.7 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
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

using EGO.Framework;

namespace QF.Editor
{
    public class HeaderView : HorizontalLayout
    {
        public HeaderView()
        {
            HorizontalStyle = "box";
            
            new LabelView(LocaleText.PackageName)
                .Width(150)
                .FontSize(12)
                .FontBold()
                .AddTo(this);

            new LabelView(LocaleText.ServerVersion)
                .Width(80)
                .TextMiddleCenter()
                .FontSize(12)
                .FontBold()
                .AddTo(this);

            new LabelView(LocaleText.LocalVersion)
                .Width(80)
                .TextMiddleCenter()
                .FontSize(12)
                .FontBold()
                .AddTo(this);

            new LabelView(LocaleText.AccessRight)
                .Width(80)
                .TextMiddleCenter()
                .FontSize(12)
                .FontBold()
                .AddTo(this);
            
            new LabelView(LocaleText.Doc)
                .Width(40)
                .TextMiddleCenter()
                .FontSize(12)
                .FontBold()
                .AddTo(this);
            
            new LabelView(LocaleText.Action)
                .Width(100)
                .TextMiddleCenter()
                .FontSize(12)
                .FontBold()
                .AddTo(this);

            new LabelView(LocaleText.ReleaseNote)
                .Width(100)
                .TextMiddleCenter()
                .FontSize(12)
                .FontBold()
                .AddTo(this);
        }

        class LocaleText
        {
        
            public static string PackageName
            {
                get { return Language.IsChinese ? " 模块名" : " Package Name"; }
            }

            public static string ServerVersion
            {
                get { return Language.IsChinese ? "服务器版本" : "Server Version"; }
            }

            public static string LocalVersion
            {
                get { return Language.IsChinese ? "本地版本" : "Local Version"; }
            }

            public static string AccessRight
            {
                get { return Language.IsChinese ? "访问权限" : "Access Right"; }
            }

            public static string Doc
            {
                get { return Language.IsChinese ? "文档" : "Doc"; }
            }

            public static string Action
            {
                get { return Language.IsChinese ? "动作" : "Action"; }
            }

            public static string ReleaseNote
            {
                get { return Language.IsChinese ? "版本说明" : "ReleaseNote Note"; }
            }
            
        }
    }
}