using UniRx;
using UnityEngine;

namespace Unidux.Example.Middlewares
{
    public static class Middlewares
    {
        public static System.Func<System.Func<object, object>, System.Func<object, object>> Thunk(IStoreObject store)
        {
            return (System.Func<object, object> next) => (object action) =>
            {
                return Observable.ReturnUnit().Select(_ =>
                {
                    next(action);
                    return Unit.Default;
                });
            };
        }

        public static System.Func<System.Func<object, object>, System.Func<object, object>> Logger(IStoreObject store)
        {
            return (System.Func<object, object> next) => (object action) =>
            {
                Debug.Log("previous state: " + store.ObjectState);
                Debug.Log("action: " + action);
                var result = next(action);
                Debug.Log("next state: " + store.ObjectState);
                return result;
            };
        }

        public static System.Func<System.Func<object, object>, System.Func<object, object>> CrashReport(
            IStoreObject store)
        {
            return (System.Func<object, object> next) => (object action) =>
            {
                try
                {
                    return next(action);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("CrashReport: " + e.Message);
                    throw e;
                }
            };
        }
    }
}