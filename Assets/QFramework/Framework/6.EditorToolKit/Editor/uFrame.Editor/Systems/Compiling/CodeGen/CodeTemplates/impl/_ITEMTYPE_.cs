namespace QF.GraphDesigner
{
    public class _ITEMTYPE_ : _TEMPLATETYPE_
    {
        public override string TheType(TemplateContext context)
        {
            if (context.TypedItem == null)
            {
                return context.Item.Name;
            }
            return context.TypedItem.RelatedTypeName;
        }
    }
}