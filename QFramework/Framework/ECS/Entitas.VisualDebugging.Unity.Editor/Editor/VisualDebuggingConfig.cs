using System.Collections.Generic;
using Entitas.Utils;

namespace Entitas.VisualDebugging.Unity.Editor
{
    public class VisualDebuggingConfig : AbstractConfigurableConfig
    {
        const string SYSTEM_WARNING_THRESHOLD_KEY = "Entitas.VisualDebugging.Unity.Editor.SystemWarningThreshold";

        const string DEFAULT_INSTANCE_CREATOR_FOLDER_PATH_KEY =
            "Entitas.VisualDebugging.Unity.Editor.DefaultInstanceCreatorFolderPath";

        const string TYPE_DRAWER_FOLDER_PATH_KEY = "Entitas.VisualDebugging.Unity.Editor.TypeDrawerFolderPath";

        public override Dictionary<string, string> DefaultProperties
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {SYSTEM_WARNING_THRESHOLD_KEY, "5"},
                    {DEFAULT_INSTANCE_CREATOR_FOLDER_PATH_KEY, "Assets/Editor/DefaultInstanceCreator"},
                    {TYPE_DRAWER_FOLDER_PATH_KEY, "Assets/Editor/TypeDrawer"}
                };
            }
        }

        public int systemWarningThreshold
        {
            get { return int.Parse(Properties[SYSTEM_WARNING_THRESHOLD_KEY]); }
            set { Properties[SYSTEM_WARNING_THRESHOLD_KEY] = value.ToString(); }
        }

        public string defaultInstanceCreatorFolderPath
        {
            get { return Properties[DEFAULT_INSTANCE_CREATOR_FOLDER_PATH_KEY]; }
            set { Properties[DEFAULT_INSTANCE_CREATOR_FOLDER_PATH_KEY] = value; }
        }

        public string typeDrawerFolderPath
        {
            get { return Properties[TYPE_DRAWER_FOLDER_PATH_KEY]; }
            set { Properties[TYPE_DRAWER_FOLDER_PATH_KEY] = value; }
        }
    }
}