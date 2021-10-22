namespace QFramework
{
    public class ILKitBehaviour : ILComponentBehaviour
    {
        private void Awake()
        {
            if (ScriptKit.Script == null)
            {
                ScriptKit.Script = new ILRuntimeScript();

                ScriptKit.LoadScript(() =>
                {
                    ScriptKit.CallStaticMethod(Namespace + "." + ScriptName, "Start", this);
                });
            }
            else
            {
                ScriptKit.CallStaticMethod(Namespace + "." + ScriptName, "Start", this);
            }
        }

        void OnApplicationQuit()
        {
#if UNITY_EDITOR
            if (ScriptKit.Script != null)
            {
                ScriptKit.Dispose();
            }
#endif
        }


    }
}