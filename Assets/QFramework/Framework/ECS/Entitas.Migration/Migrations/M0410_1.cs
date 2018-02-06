using System.Collections.Generic;
using System.Linq;

namespace QFramework.Migration
{

    public class M0410_1 : IMigration
    {

        public string version
        {
            get { return "0.41.0-1"; }
        }

        public string workingDirectory
        {
            get { return "where source code files are located"; }
        }

        public string description
        {
            get { return "Updating namespaces"; }
        }

        public MigrationFile[] Migrate(string path)
        {
            var files = MigrationUtils.GetFiles(path);

            var migratedFiles = new List<MigrationFile>();

            migratedFiles.AddRange(updateNamespace(files, "QFramework.CodeGenerator.Api",
                "QFramework.CodeGeneration.Attributes"));
            migratedFiles.AddRange(updateNamespace(files, "QFramework.Unity.VisualDebugging",
                "QFramework.VisualDebugging.Unity"));
            migratedFiles.AddRange(updateNamespace(files, "QFramework.Unity.Blueprints", "QFramework.Blueprints.Unity"));

            return migratedFiles.ToArray();
        }

        MigrationFile[] updateNamespace(MigrationFile[] files, string oldNamespace, string newNamespace)
        {
            var filesToMigrate = files.Where(f => f.fileContent.Contains(oldNamespace)).ToArray();
            foreach (var file in filesToMigrate)
            {
                file.fileContent = file.fileContent.Replace(oldNamespace, newNamespace);
            }

            return filesToMigrate;
        }
    }
}