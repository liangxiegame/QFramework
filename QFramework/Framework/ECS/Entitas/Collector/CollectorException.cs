using QFramework;

namespace Entitas {

    public class CollectorException : ExceptionWithHint {

        public CollectorException(string message, string hint)
            : base(message, hint) {
        }
    }
}
