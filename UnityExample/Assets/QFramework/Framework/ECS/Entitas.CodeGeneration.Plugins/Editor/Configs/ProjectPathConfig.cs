using System.Collections.Generic;


namespace QFramework.CodeGeneration.Plugins {

    public class ProjectPathConfig : AbstractConfigurableConfig {

        const string PROJECT_PATH_KEY = "QFramework.CodeGeneration.Plugins.ProjectPath";

        public override Dictionary<string, string> DefaultProperties {
            get {
                return new Dictionary<string, string> {
                    { PROJECT_PATH_KEY, "Assembly-CSharp.csproj" }
                };
            }
        }

        public string projectPath {
            get { return Properties[PROJECT_PATH_KEY]; }
        }
    }
}
