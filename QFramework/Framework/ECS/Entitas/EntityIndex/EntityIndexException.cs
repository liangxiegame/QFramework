using QFramework;

namespace Entitas {

    public class EntityIndexException : ExceptionWithHint {

        public EntityIndexException(string message, string hint)
            : base(message, hint) {
        }
    }
}
