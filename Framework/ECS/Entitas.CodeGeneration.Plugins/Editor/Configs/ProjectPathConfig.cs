using System.Collections.Generic;


namespace QFramework {

    public class ProjectPathConfig : AbstractConfigurableConfig {

        const string PROJECT_PATH_KEY = "QFramework.ProjectPath";

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
