using System.Collections.Generic;

namespace Entitas.Utils
{
    public interface IConfigurable
    {
        Dictionary<string, string> DefaultProperties { get; }

        void Configure(Properties properties);
    }
}