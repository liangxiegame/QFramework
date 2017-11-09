using System.Collections.Generic;


namespace QFramework {

    public class TargetDirectoryConfig : AbstractConfigurableConfig {

        const string TARGET_DIRECTORY_KEY = "QFramework.TargetDirectory";

        public override Dictionary<string, string> DefaultProperties {
            get {
                return new Dictionary<string, string> {
                    { TARGET_DIRECTORY_KEY, "Assets/Sources" }
                };
            }
        }

        public string targetDirectory {
            get { return Properties[TARGET_DIRECTORY_KEY].ToSafeDirectory(); }
        }
    }
}
