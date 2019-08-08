namespace QF.GraphDesigner
{
    public class CreateDatabaseCommand : Command
    {
        private string _ns;
        private string _codePath = "Assets/Code";

        [InspectorProperty("A unique name for the database.")]
        public string Name { get; set; }

        [InspectorProperty("A namespace to be used for all the generated classes.")]
        public string Namespace
        {
            get
            {
                if (string.IsNullOrEmpty(_ns))
                    return Name;
                return _ns;
            }
            set { _ns = value; }
        }

        [InspectorProperty("A path for the generated code output. This is relative to the assets folder.")]
        public string CodePath
        {
            get { return _codePath; }
            set { _codePath = value; }
        }
    }
}