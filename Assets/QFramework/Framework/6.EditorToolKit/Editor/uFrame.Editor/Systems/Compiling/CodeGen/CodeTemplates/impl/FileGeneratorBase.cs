using System.IO;

namespace QF.GraphDesigner
{
    public abstract class FileGeneratorBase
    {
        public string SystemPath { get; set; }
        public string AssetPath { get; set; }
        public abstract string CreateOutput();
        public override string ToString()
        {
            return CreateOutput();
        }
        public abstract bool CanGenerate(FileInfo fileInfo);
        
    }
}