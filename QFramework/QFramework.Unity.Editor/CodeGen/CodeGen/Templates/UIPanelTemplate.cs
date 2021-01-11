/****************************************************************************
 * Copyright (c) 2019.1 ~ 2021.1 liangxie
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

namespace QFramework
{
    using System.IO;

    public class UIPanelTemplate
    {
        public static void Write(string name, string srcFilePath, string scriptNamespace,
            UIKitSettingData uiKitSettingData)
        {
            var scriptFile = srcFilePath;

            if (File.Exists(scriptFile))
            {
                return;
            }

            var writer = File.CreateText(scriptFile);

            var codeWriter = new FileCodeWriter(writer);


            var rootCode = new RootCode()
                .Using("UnityEngine")
                .Using("UnityEngine.UI")
                .Using("QFramework")
                .EmptyLine()
                .Namespace(scriptNamespace, nsScope =>
                {
                    nsScope.Class(name + "Data", "UIPanelData", false, false, classScope => { });

                    nsScope.Class(name, "UIPanel", true, false, classScope =>
                    {
                        classScope.CustomScope("protected override void ProcessMsg(int eventId, QMsg msg)", false,
                            (function) => { function.Custom("throw new System.NotImplementedException();"); });

                        classScope.EmptyLine();
                        classScope.CustomScope("protected override void OnInit(IUIData uiData = null)", false,
                            function =>
                            {
                                function.Custom("mData = uiData as {0} ?? new {0}();".FillFormat(name + "Data"));
                                function.Custom("// please add init code here");
                            });

                        classScope.EmptyLine();
                        classScope.CustomScope("protected override void OnOpen(IUIData uiData = null)", false,
                            function => { });

                        classScope.EmptyLine();
                        classScope.CustomScope("protected override void OnShow()", false,
                            function => { });
                        classScope.EmptyLine();
                        classScope.CustomScope("protected override void OnHide()", false,
                            function => { });

                        classScope.EmptyLine();
                        classScope.CustomScope("protected override void OnClose()", false,
                            function => { });
                    });
                });

            rootCode.Gen(codeWriter);
            codeWriter.Dispose();
        }
    }
}