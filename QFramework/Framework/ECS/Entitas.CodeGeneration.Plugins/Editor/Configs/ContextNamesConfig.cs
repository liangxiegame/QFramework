using System.Collections.Generic;
using Entitas.Utils;
using QFramework;
namespace Entitas.CodeGeneration.Plugins {

    public class ContextNamesConfig : AbstractConfigurableConfig {

        const string CONTEXTS_KEY = "Entitas.CodeGeneration.Plugins.Contexts";

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
