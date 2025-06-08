/****************************************************************************
* Copyright (c) 2016 ~ 2025 liangxiegame UNDER MIT LINCENSE
* 
* https://qframework.cn
* https://github.com/liangxiegame/QFramework
* https://gitee.com/liangxiegame/QFramework
****************************************************************************/

namespace QFramework
{
    using System.IO;

    public class UIPanelTemplate
    {
        public static void Write(string name, string srcFilePath, string scriptNamespace)
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
                        classScope.CustomScope("protected override void OnInit(IUIData uiData = null)", false,
                            function =>
                            {
                                function.Custom(string.Format("mData = uiData as {0} ?? new {0}();",(name + "Data")));
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