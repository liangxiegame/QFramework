namespace QF.GraphDesigner
{
    public class GraphConfiguration : IGraphConfiguration
    {
        public GraphConfiguration(string codeOutputPath, string ns)
        {
            CodeOutputPath = codeOutputPath;
            Namespace = ns;
            IsCurrent = true;
        }

        public string CodeOutputPath { get; set; }
        public string Namespace { get; set; }
        public bool IsCurrent { get; set; }
        public string FullPath { get; set; }
        public string Title { get; set; }
        public string Group { get;  set; }
        public string SearchTag { get;  set; }
        public string Description { get; set; }
    }
}