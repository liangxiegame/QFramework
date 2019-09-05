using QF.GraphDesigner;

namespace QFramework
{
    public class UIGraph :IGraphConfiguration
    {
        public string Title { get; private set; }
        public string Group { get; private set; }
        public string SearchTag { get; private set; }
        public string Description { get; set; }
        public string CodeOutputPath { get; private set; }
        public string Namespace { get; set; }
        public bool IsCurrent { get; set; }
        public string FullPath { get; private set; }
    }
}