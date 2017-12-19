

namespace QFramework {

    public class ContextInfoException : ExceptionWithHint
    {
        public ContextInfoException(IContext context, ContextInfo contextInfo)
            : base("Invalid ContextInfo for '" + context + "'!\nExpected " +
                context.TotalComponents + " componentName(s) but got " +
                contextInfo.ComponentNames.Length + ":",
                string.Join("\n", contextInfo.ComponentNames)) {
        }
    }
}
