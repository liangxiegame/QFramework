

namespace QFramework
{
    public class EntityDoesNotHaveComponentException : ExceptionWithHint
    {
        public EntityDoesNotHaveComponentException(int index, string message, string hint)
            : base(message + "\nEntity does not have a component at index " + index + "!", hint)
        {
        }
    }
}