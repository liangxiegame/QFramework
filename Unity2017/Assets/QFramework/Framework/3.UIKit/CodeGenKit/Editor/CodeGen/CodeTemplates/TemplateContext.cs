using System.CodeDom;

namespace QFramework.CodeGen.Pro
{
    public class TemplateContext<TData>
    {
        public TData Data { get; set; }
        public object CurrentItem { get; set; }

        public TAs Item<TAs>()
        {
            return (TAs)CurrentItem;
        }
        public CodeMemberMethod CurrentMethod { get; set; }
        public CodeMemberProperty CurrentProperty { get; set; }

    }
}