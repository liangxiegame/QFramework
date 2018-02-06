using System.Collections.Generic;


namespace QFramework {

    public class ContextNamesConfig : AbstractConfigurableConfig {

        const string CONTEXTS_KEY = "QFramework.Contexts";

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
