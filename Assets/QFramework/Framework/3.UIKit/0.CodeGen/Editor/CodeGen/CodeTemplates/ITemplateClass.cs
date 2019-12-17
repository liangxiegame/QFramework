namespace QFramework.CodeGen.Pro
{
    public interface ITemplateClass<TData>
    {
        TemplateContext<TData> Context { get; set; }
       
    }
}