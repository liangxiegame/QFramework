using Invert.Data;

namespace QFramework.CodeGen
{
    public class FlagItem : IDataRecord
    {
        private string _parentIdentifier;
        private string _name;
        public IRepository Repository { get; set; }
        public string Identifier { get; set; }
        public bool Changed { get; set; }

        public string ParentIdentifier
        {
            get { return _parentIdentifier; }
            set
            {
                
                this.Changed("ParentIdentifier",ref _parentIdentifier,value);
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                Changed = true;
            }
        }
    }
}