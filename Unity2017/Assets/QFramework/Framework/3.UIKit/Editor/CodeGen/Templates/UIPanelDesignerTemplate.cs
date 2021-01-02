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

using System;
using System.IO;

namespace QFramework
{
    public class UIPanelDesignerTemplate
    {
        public static void Write(string name, string scriptsFolder, string scriptNamespace, PanelCodeInfo panelCodeInfo,
            UIKitSettingData uiKitSettingData)
        {
            var scriptFile = scriptsFolder + "/{0}.Designer.cs".FillFormat(name);

            var writer = File.CreateText(scriptFile);

            var root = new RootCode()
                .Using("System")
                .Using("UnityEngine")
                .Using("UnityEngine.UI")
                .Using("QFramework")
                .EmptyLine()
                .Namespace(scriptNamespace.IsTrimNullOrEmpty()
                    ? uiKitSettingData.Namespace
                    : scriptNamespace, ns =>
                {
                    ns.Custom("// Generate Id:{0}".FillFormat(Guid.NewGuid().ToString()));
                    ns.Class(name, null, true, false, (classScope) =>
                    {
                        classScope.Custom("public const string Name = \"" + name + "\";");
                        classScope.EmptyLine();

                        foreach (var bindInfo in panelCodeInfo.BindInfos)
                        {
                            if (bindInfo.BindScript.Comment.IsNotNullAndEmpty())
                            {
                                classScope.Custom("/// <summary>");
                                classScope.Custom("/// " + bindInfo.BindScript.Comment);
                                classScope.Custom("/// </summary>");
                            }

                            classScope.Custom("[SerializeField]");
                            classScope.Custom("public " + bindInfo.BindScript.ComponentName + " " + bindInfo.Name +
                                              ";");
                        }

                        classScope.EmptyLine();
                        classScope.Custom("private " + name + "Data mPrivateData = null;");

                        classScope.EmptyLine();

                        classScope.CustomScope("protected override void ClearUIComponents()", false, (function) =>
                        {
                            foreach (var bindInfo in panelCodeInfo.BindInfos)
                            {
                                function.Custom(bindInfo.Name + " = null;");
                            }

                            function.EmptyLine();
                            function.Custom("mData = null;");
                        });

                        classScope.EmptyLine();

                        classScope.CustomScope("public " + name + "Data Data", false,
                            (property) =>
                            {
                                property.CustomScope("get", false, (getter) => { getter.Custom("return mData;"); });
                            });

                        classScope.EmptyLine();


                        classScope.CustomScope(name + "Data mData", false, (property) =>
                        {
                            property.CustomScope("get", false,
                                (getter) =>
                                {
                                    getter.Custom("return mPrivateData ?? (mPrivateData = new " + name + "Data());");
                                });

                            property.CustomScope("set", false, (setter) =>
                            {
                                setter.Custom("mUIData = value;");
                                setter.Custom("mPrivateData = value;");
                            });
                        });
                    });
                });

            var codeWriter = new FileCodeWriter(writer);
            root.Gen(codeWriter);
            codeWriter.Dispose();
        }
    }
}