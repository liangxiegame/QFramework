using QFramework;

namespace Entitas {

    public class ContextDoesNotContainEntityException : ExceptionWithHint {

        public ContextDoesNotContainEntityException(string message, string hint)
            : base(message + "\nContext does not contain entity!", hint) {
        }
    }
}
