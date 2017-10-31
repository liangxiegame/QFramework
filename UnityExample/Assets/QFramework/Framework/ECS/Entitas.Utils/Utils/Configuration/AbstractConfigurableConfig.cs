using System.Collections.Generic;

namespace QFramework
{
    public abstract class AbstractConfigurableConfig : IConfigurable
    {
        public abstract Dictionary<string, string> DefaultProperties { get; }

        public Properties Properties
        {
            get { return mProperties; }
        }

        Properties mProperties;

        public virtual void Configure(Properties properties)
        {
            mProperties = properties;
        }

        public override string ToString()
        {
            return Properties.ToString();
        }
    }
}