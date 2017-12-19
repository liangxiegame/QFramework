using System.Collections.Generic;



namespace QFramework
{

    public class CodeGeneratorConfig : AbstractConfigurableConfig
    {

        const string SEARCH_PATHS_KEY = "QFramework.SearchPaths";
        const string PLUGINS_PATHS_KEY = "QFramework.Plugins";

        const string DATA_PROVIDERS_KEY = "QFramework.DataProviders";
        const string CODE_GENERATORS_KEY = "QFramework.CodeGenerators";
        const string POST_PROCESSORS_KEY = "QFramework.PostProcessors";

        public override Dictionary<string, string> DefaultProperties
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {
                        SEARCH_PATHS_KEY,
                        "Assets/Libraries/QFramework, Assets/Libraries/QFramework/Editor, /Applications/Unity/Unity.app/Contents/Managed, /Applications/Unity/Unity.app/Contents/Mono/lib/mono/unity, /Applications/Unity/Unity.app/Contents/UnityExtensions/Unity/GUISystem"
                    },
                    {
                        PLUGINS_PATHS_KEY,
                        "QFramework, QFramework.VisualDebugging.CodeGeneration.Plugins"
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