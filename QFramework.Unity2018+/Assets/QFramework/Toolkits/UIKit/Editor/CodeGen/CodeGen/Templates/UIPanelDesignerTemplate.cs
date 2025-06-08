/****************************************************************************
* Copyright (c) 2016 ~ 2025 liangxiegame UNDER MIT LINCENSE
* 
* https://qframework.cn
* https://github.com/liangxiegame/QFramework
* https://gitee.com/liangxiegame/QFramework
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
            var scriptFile = string.Format(scriptsFolder + "/{0}.Designer.cs",name);

            var writer = File.CreateText(scriptFile);

            var root = new RootCode()
                .Using("System")
                .Using("UnityEngine")
                .Using("UnityEngine.UI")
                .Using("QFramework")
                .EmptyLine()
                .Namespace(string.IsNullOrWhiteSpace(scriptNamespace)
                    ? uiKitSettingData.Namespace
                    : scriptNamespace, ns =>
                {
                    ns.Custom(string.Format("// Generate Id:{0}",Guid.NewGuid().ToString()));
                    ns.Class(name, null, true, false, (classScope) =>
                    {
                        classScope.Custom("public const string Name = \"" + name + "\";");
                        classScope.EmptyLine();

                        foreach (var bindInfo in panelCodeInfo.BindInfos)
                        {
                            if (!string.IsNullOrEmpty(bindInfo.BindScript.Comment))
                            {
                                classScope.Custom("/// <summary>");
                                classScope.Custom("/// " + bindInfo.BindScript.Comment);
                                classScope.Custom("/// </summary>");
                            }

                            classScope.Custom("[SerializeField]");
                            classScope.Custom("public " + bindInfo.BindScript.TypeName + " " + bindInfo.TypeName +
                                              ";");
                        }

                        classScope.EmptyLine();
                        classScope.Custom("private " + name + "Data mPrivateData = null;");

                        classScope.EmptyLine();

                        classScope.CustomScope("protected override void ClearUIComponents()", false, (function) =>
                        {
                            foreach (var bindInfo in panelCodeInfo.BindInfos)
                            {
                                function.Custom(bindInfo.TypeName + " = null;");
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