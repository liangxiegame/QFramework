

namespace QFramework {

    public class ContextEntityIndexDoesNotExistException : ExceptionWithHint {

        public ContextEntityIndexDoesNotExistException(IContext context, string name)
            : base("Cannot get EntityIndex '" + name + "' from context '" +
                context + "'!", "No EntityIndex with this name has been added.") {
        }
    }
}
