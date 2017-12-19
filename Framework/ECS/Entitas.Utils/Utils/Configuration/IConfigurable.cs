using System.Collections.Generic;

namespace QFramework
{
    public interface IConfigurable
    {
        Dictionary<string, string> DefaultProperties { get; }

        void Configure(Properties properties);
    }
}