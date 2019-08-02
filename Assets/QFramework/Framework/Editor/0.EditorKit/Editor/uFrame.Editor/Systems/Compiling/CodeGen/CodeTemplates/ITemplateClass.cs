namespace QFramework.GraphDesigner.Pro
{
    public interface ITemplateClass<TData>
    {
        TemplateContext<TData> Context { get; set; }
       
    }
}