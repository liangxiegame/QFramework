public class AdapterRegister
{
    public static void RegisterCrossBindingAdaptor(ILRuntime.Runtime.Enviorment.AppDomain domain)
    {
        //这几条是手写的
        domain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
        domain.RegisterCrossBindingAdaptor(new CoroutineAdapter());

        //以下是自动生成的


        domain.RegisterCrossBindingAdaptor(new ScriptableObjectAdapter());

        domain.RegisterCrossBindingAdaptor(new ExceptionAdapter());

        domain.RegisterCrossBindingAdaptor(new IEnumerableAdapter());

        domain.RegisterCrossBindingAdaptor(new IAsyncStateMachineAdapter());
    }
}