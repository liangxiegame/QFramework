using System.Collections.Generic;


namespace QFramework.CodeGeneration.Plugins {

    public class ContextNamesConfig : AbstractConfigurableConfig {

        const string CONTEXTS_KEY = "QFramework.CodeGeneration.Plugins.Contexts";

        public override Dictionary<string, string> DefaultProperties {
            get {
                return new Dictionary<string, string> {
                    { CONTEXTS_KEY, "Game, Input" }
                };
            }
        }

        public string[] contextNames {
            get { return Properties[CONTEXTS_KEY].ArrayFromCSV(); }
        }
    }
}
