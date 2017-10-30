using System.Collections.Generic;
using Entitas.Utils;
using QFramework;

namespace Entitas.CodeGeneration.CodeGenerator
{

    public class CodeGeneratorConfig : AbstractConfigurableConfig
    {

        const string SEARCH_PATHS_KEY = "Entitas.CodeGeneration.CodeGenerator.SearchPaths";
        const string PLUGINS_PATHS_KEY = "Entitas.CodeGeneration.CodeGenerator.Plugins";

        const string DATA_PROVIDERS_KEY = "Entitas.CodeGeneration.CodeGenerator.DataProviders";
        const string CODE_GENERATORS_KEY = "Entitas.CodeGeneration.CodeGenerator.CodeGenerators";
        const string POST_PROCESSORS_KEY = "Entitas.CodeGeneration.CodeGenerator.PostProcessors";

        public override Dictionary<string, string> DefaultProperties
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {
                        SEARCH_PATHS_KEY,
                        "Assets/Libraries/Entitas, Assets/Libraries/Entitas/Editor, /Applications/Unity/Unity.app/Contents/Managed, /Applications/Unity/Unity.app/Contents/Mono/lib/mono/unity, /Applications/Unity/Unity.app/Contents/UnityExtensions/Unity/GUISystem"
                    },
                    {
                        PLUGINS_PATHS_KEY,
                        "Entitas.CodeGeneration.Plugins, Entitas.VisualDebugging.CodeGeneration.Plugins"
                    },

                    {DATA_PROVIDERS_KEY, string.Empty},
                    {CODE_GENERATORS_KEY, string.Empty},
                    {POST_PROCESSORS_KEY, string.Empty}
                };
            }
        }

        public string[] SearchPaths
        {
            get { return Properties[SEARCH_PATHS_KEY].ArrayFromCSV(); }
            set { Properties[SEARCH_PATHS_KEY] = value.ToCSV(); }
        }

        public string[] Plugins
        {
            get { return Properties[PLUGINS_PATHS_KEY].ArrayFromCSV(); }
            set { Properties[PLUGINS_PATHS_KEY] = value.ToCSV(); }
        }

        public string[] DataProviders
        {
            get { return Properties[DATA_PROVIDERS_KEY].ArrayFromCSV(); }
            set { Properties[DATA_PROVIDERS_KEY] = value.ToCSV(); }
        }

        public string[] CodeGenerators
        {
            get { return Properties[CODE_GENERATORS_KEY].ArrayFromCSV(); }
            set { Properties[CODE_GENERATORS_KEY] = value.ToCSV(); }
        }

        public string[] PostProcessors
        {
            get { return Properties[POST_PROCESSORS_KEY].ArrayFromCSV(); }
            set { Properties[POST_PROCESSORS_KEY] = value.ToCSV(); }
        }
    }
}