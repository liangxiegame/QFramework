namespace QFramework.GraphDesigner
{
    public class ConfigProxy : GraphItemAttribute
    {
        public SectionVisibility Visibility { get; set; }

        public ConfigProxy(SectionVisibility visibility)
        {
            Visibility = visibility;
        }
    }
}