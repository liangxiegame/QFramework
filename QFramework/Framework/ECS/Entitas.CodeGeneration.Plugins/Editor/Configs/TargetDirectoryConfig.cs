using System.Collections.Generic;
using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class TargetDirectoryConfig : AbstractConfigurableConfig {

        const string TARGET_DIRECTORY_KEY = "Entitas.CodeGeneration.Plugins.TargetDirectory";

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
