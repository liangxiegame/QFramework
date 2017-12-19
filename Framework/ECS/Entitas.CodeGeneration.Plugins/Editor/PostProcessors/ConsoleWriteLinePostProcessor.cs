using System;


namespace QFramework {

    public class ConsoleWriteLinePostProcessor : ICodeGenFilePostProcessor {

        public string Name { get { return "Console.WriteLine generated files"; } }
        public int Priority { get { return 200; } }
        public bool IsEnabledByDefault { get { return false; } }
        public bool RunInDryMode { get { return true; } }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            foreach (var file in files) {
                Console.WriteLine(file.FileName + " - " + file.GeneratorName);
            }

            return files;
        }
    }
}
