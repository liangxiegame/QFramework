using System.Collections.Generic;
using System.Linq;
using QFramework;

namespace Entitas.CodeGeneration.Plugins {

    public class MergeFilesPostProcessor : ICodeGenFilePostProcessor {

        public string Name { get { return "Merge files"; } }
        public int Priority { get { return 90; } }
        public bool IsEnabledByDefault { get { return true; } }
        public bool RunInDryMode { get { return true; } }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            var pathToFile = new Dictionary<string, CodeGenFile>();
            for (int i = 0; i < files.Length; i++) {
                var file = files[i];
                if (!pathToFile.ContainsKey(file.FileName)) {
                    pathToFile.Add(file.FileName, file);
                } else {
                    pathToFile[file.FileName].FileContent += "\n" + file.FileContent;
                    pathToFile[file.FileName].GeneratorName += ", " + file.GeneratorName;
                    files[i] = null;
                }
            }

            return files
                .Where(file => file != null)
                .ToArray();
        }
    }
}
