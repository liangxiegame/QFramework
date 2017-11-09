using System.Collections.Generic;
using System.IO;



namespace QFramework
{

    public class WriteToDiskPostProcessor : ICodeGenFilePostProcessor, IConfigurable
    {

        public string Name
        {
            get { return "Write to disk"; }
        }

        public int Priority
        {
            get { return 100; }
        }

        public bool IsEnabledByDefault
        {
            get { return true; }
        }

        public bool RunInDryMode
        {
            get { return false; }
        }

        public Dictionary<string, string> DefaultProperties
        {
            get { return _targetDirectoryConfig.DefaultProperties; }
        }

        readonly TargetDirectoryConfig _targetDirectoryConfig = new TargetDirectoryConfig();

        public void Configure(Properties properties)
        {
            _targetDirectoryConfig.Configure(properties);
        }

        public CodeGenFile[] PostProcess(CodeGenFile[] files)
        {
            foreach (var file in files)
            {
                var fileName = _targetDirectoryConfig.targetDirectory + Path.DirectorySeparatorChar + file.FileName;
                var targetDir = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }
                File.WriteAllText(fileName, file.FileContent);
            }

            return files;
        }
    }
}