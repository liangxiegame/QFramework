namespace QF.GraphDesigner
{
    public class EditDatabaseCommand : Command
    {
        
        public uFrameDatabaseConfig Configuration { get; set; }

        [InspectorProperty("A namespace to be used for all the generated classes.")]
        public string Namespace { get; set; }

        [InspectorProperty("A path for the generated code output.")]
        public string CodePath { get; set; }


    }
}