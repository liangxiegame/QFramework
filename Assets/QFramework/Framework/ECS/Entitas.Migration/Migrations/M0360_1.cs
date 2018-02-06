namespace QFramework.Migration
{

    public class M0360_1 : IMigration
    {

        public string version
        {
            get { return "0.36.0-1"; }
        }

        public string workingDirectory
        {
            get { return "project root"; }
        }

        public string description
        {
            get { return "Updates QFramework.properties to use renamed keys"; }
        }

        public MigrationFile[] Migrate(string path)
        {
            var properties = MigrationUtils.GetFiles(path, "QFramework.properties");

            for (int i = 0; i < properties.Length; i++)
            {
                var file = properties[i];

                // QFramework.CodeGenerator.Pools = Input,Core,Score

                file.fileContent =
                    file.fileContent.Replace("QFramework.CodeGenerator.Pools", "QFramework.CodeGenerator.Contexts");
            }

            return properties;
        }
    }
}