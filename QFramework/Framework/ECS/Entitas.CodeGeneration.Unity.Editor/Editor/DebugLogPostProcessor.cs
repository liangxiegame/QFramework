using UnityEngine;
using QFramework;

namespace Entitas.CodeGeneration.Unity.Editor {

    public class DebugLogPostProcessor : ICodeGenFilePostProcessor {

        public string Name { get { return "Debug.Log generated files"; } }
        public bool IsEnabledByDefault { get { return false; } }
        public int Priority { get { return 200; } }
        public bool RunInDryMode { get { return true; } }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            foreach (var file in files) {
                Debug.Log(file.FileName + " - " + file.GeneratorName);
            }

            return files;
        }
    }
}
