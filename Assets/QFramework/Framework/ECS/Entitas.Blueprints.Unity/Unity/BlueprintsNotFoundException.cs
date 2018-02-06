


namespace QFramework.Blueprints.Unity 
{
    public class BlueprintsNotFoundException : ExceptionWithHint 
    {
        public BlueprintsNotFoundException(string blueprintName)
            : base("'" + blueprintName + "' does not exist!", "Did you update the Blueprints ScriptableObject?") {
        }
    }
}
