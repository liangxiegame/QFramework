using System.IO;

namespace QF.GraphDesigner
{
    public class DefaultCodePathStrategy : ICodePathStrategy
    {
        public IGraphData Data { get; set; }

        public string AssetPath { get; set; }

        public virtual string BehavioursPath
        {
            get { return Path.Combine(AssetPath, "Behaviours"); }
        }

        public virtual string ScenesPath
        {
            get { return Path.Combine(AssetPath, "Scenes"); }
        }

        public string GetDesignerFilePath(string postFix)
        {
            return Path.Combine("_DesignerFiles", Data.Name + postFix + ".designer.cs");
        }

        public string GetEditableFilePath(IGraphItem item, string name = null)
        {
            return (name ?? Data.Name) + ".cs";
        }
    }
}