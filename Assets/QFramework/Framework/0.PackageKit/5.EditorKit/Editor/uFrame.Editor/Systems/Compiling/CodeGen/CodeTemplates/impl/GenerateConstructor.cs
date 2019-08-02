namespace QF.GraphDesigner
{
    public class GenerateConstructor : GenerateMember
    {
        public string[] BaseCallArgs { get; set; }

        public GenerateConstructor(TemplateLocation location, params string[] baseCallArgs)
            : base(location)
        {
            BaseCallArgs = baseCallArgs;
        }

        public GenerateConstructor(params string[] baseCallArgs)
        {
            BaseCallArgs = baseCallArgs;
        }
    }
}