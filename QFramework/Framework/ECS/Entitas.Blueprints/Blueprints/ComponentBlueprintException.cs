using QFramework;

namespace Entitas.Blueprints
{
    public class ComponentBlueprintException : ExceptionWithHint
    {
        public ComponentBlueprintException(string message, string hint)
            : base(message, hint)
        {
        }
    }
}