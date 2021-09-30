using System.Numerics;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Generated;

namespace QFramework
{
    public class ILRuntimeValueTypeBinderHelper
    {
        public static void Register(AppDomain appdomain)
        {
            //暂时注释  Vector2Binder有问题
            return;
            appdomain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());
            appdomain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
            appdomain.RegisterValueTypeBinder(typeof(Vector4), new Vector4Binder());
            appdomain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());
        }
    }
}