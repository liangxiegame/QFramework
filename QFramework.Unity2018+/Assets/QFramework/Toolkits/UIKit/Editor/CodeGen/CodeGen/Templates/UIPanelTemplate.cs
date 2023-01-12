/****************************************************************************
 * Copyright (c) 2016 ~ 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * 
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
                .EmptyLine();

            var subRoot = new RootCode()
                .Class(name + "Data", "UIPanelData", false, false, classScope => { })
                .EmptyLine()
                .Class(name, "UIPanel", true, false, classScope =>
                {
                    classScope.CustomScope("protected override void OnInit(IUIData uiData = null)", false,
                        function =>
                        {
                            function.Custom(string.Format("mData = uiData as {0} ?? new {0}();", (name + "Data")));
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

            if (!string.IsNullOrWhiteSpace(scriptNamespace))
            {
                rootCode.NewNamespace(scriptNamespace, subRoot);
            }
            else
            {
                rootCode.CustomCodeScopeode(subRoot);
            }

            rootCode.Gen(codeWriter);
            codeWriter.Dispose();
        }
    }
}