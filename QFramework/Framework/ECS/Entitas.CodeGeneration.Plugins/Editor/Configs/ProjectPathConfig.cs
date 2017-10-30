using System.Collections.Generic;
using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ProjectPathConfig : AbstractConfigurableConfig {

        const string PROJECT_PATH_KEY = "Entitas.CodeGeneration.Plugins.ProjectPath";

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
