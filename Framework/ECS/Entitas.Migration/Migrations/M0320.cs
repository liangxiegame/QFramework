using System.Linq;
using System.Text.RegularExpressions;

namespace QFramework.Migration
{

    public class M0320 : IMigration
    {

        public string version
        {
            get { return "0.32.0"; }
        }

        public string workingDirectory
        {
            get { return "project root"; }
        }

        public string description
        {
            get { return "Updates QFramework.properties to use renamed keys and updates calls to pool.CreateSystem<T>()"; }
        }

        public MigrationFile[] Migrate(string path)
        {
            var properties = MigrationUtils.GetFiles(path, "QFramework.properties");

            for (int i = 0; i < properties.Length; i++)
            {
                var file = properties[i];

                //QFramework.Unity.VisualDebugging.DefaultInstanceCreatorFolderPath = Assets/Editor/DefaultInstanceCreator/
                //QFramework.Unity.VisualDebugging.TypeDrawerFolderPath = Assets/Editor/TypeDrawer/

                file.fileContent = file.fileContent.Replace("QFramework.Unity.CodeGenerator.GeneratedFolderPath",
                    "QFramework.CodeGenerator.GeneratedFolderPath");
                file.fileContent =
                    file.fileContent.Replace("QFramework.Unity.CodeGenerator.Pools", "QFramework.CodeGenerator.Pools");
                file.fileContent = file.fileContent.Replace("QFramework.Unity.CodeGenerator.EnabledCodeGenerators",
                    "QFramework.CodeGenerator.EnabledCodeGenerators");
            }

            const string pattern = @".CreateSystem<(?<system>\w*)>\(\s*\)";

            var sources = MigrationUtils.GetFiles(path)
                .Where(file => Regex.IsMatch(file.fileContent, pattern))
                .ToArray();

            for (int i = 0; i < sources.Length; i++)
            {
                var file = sources[i];

                file.fileContent = Regex.Replace(
                    file.fileContent,
                    pattern,
                    match => ".CreateSystem(new " + match.Groups["system"].Value + "())"
                );
            }

            return properties.Concat(sources).ToArray();
        }
    }
}
