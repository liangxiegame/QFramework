using System;


namespace QFramework.CodeGeneration.Plugins {

    public class NewLinePostProcessor : ICodeGenFilePostProcessor {

        public string Name { get { return "Convert newlines"; } }
        public int Priority { get { return 95; } }
        public bool IsEnabledByDefault { get { return true; } }
        public bool RunInDryMode { get { return true; } }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            foreach (var file in files) {
                file.FileContent = file.FileContent.Replace("\n", Environment.NewLine);
            }

            return files;
        }
    }
}
