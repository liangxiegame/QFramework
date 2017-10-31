

namespace QFramework
{
    public class EntityIsNotEnabledException : ExceptionWithHint
    {
        public EntityIsNotEnabledException(string message)
            : base(message + "\nEntity is not enabled!",
                "The entity has already been destroyed. " +
                "You cannot modify destroyed entities.")
        {
        }
    }
}