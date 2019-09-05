using System;
using QF;

namespace QF.GraphDesigner
{
    public interface ICorePlugin
    {
        string Title { get; }
        bool Enabled { get; set; }
        decimal LoadPriority { get; }
        string PackageName { get; }
        bool Required { get; }
        bool Ignore { get; }
        QFrameworkContainer Container { get; set; }
        TimeSpan InitializeTime { get; set; }
        TimeSpan LoadTime { get; set; }
        void Initialize(QFrameworkContainer container);
        void Loaded(QFrameworkContainer container);
        
    }
}