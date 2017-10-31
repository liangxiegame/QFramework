using System;
using System.Collections.Generic;
using System.IO;



namespace QFramework.CodeGeneration.Plugins {

    public class CleanTargetDirectoryPostProcessor : ICodeGenFilePostProcessor, IConfigurable {

        public string Name { get { return "Clean target directory"; } }
        public int Priority { get { return 0; } }
        public bool IsEnabledByDefault { get { return true; } }
        public bool RunInDryMode { get { return false; } }

        public Dictionary<string,string> DefaultProperties { get { return _targetDirectoryConfig.DefaultProperties; } }

        readonly TargetDirectoryConfig _targetDirectoryConfig = new TargetDirectoryConfig();

        public void Configure(Properties properties) {
            _targetDirectoryConfig.Configure(properties);
        }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            cleanDir();
            return files;
        }

        void cleanDir() {
            if (Directory.Exists(_targetDirectoryConfig.targetDirectory)) {
                var files = new DirectoryInfo(_targetDirectoryConfig.targetDirectory).GetFiles("*.cs", SearchOption.AllDirectories);
                foreach (var file in files) {
                    try {
                        File.Delete(file.FullName);
                    } catch {
                        Console.WriteLine("Could not delete file " + file);
                    }
                }
            } else {
                Directory.CreateDirectory(_targetDirectoryConfig.targetDirectory);
            }
        }
    }
}
