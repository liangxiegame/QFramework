using System.Collections.Generic;


namespace QFramework {

    public class IgnoreNamespacesConfig : AbstractConfigurableConfig {

        const string IGNORE_NAMESPACES_KEY = "QFramework.IgnoreNamespaces";

        public override Dictionary<string, string> DefaultProperties {
            get {
                return new Dictionary<string, string> {
                    { IGNORE_NAMESPACES_KEY, "false" }
                };
            }
        }

        public bool ignoreNamespaces {
            get { return Properties[IGNORE_NAMESPACES_KEY] == "true" ; }
        }
    }
}
