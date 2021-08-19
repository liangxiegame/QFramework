using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace Unidux.Example.SimpleHttp
{
    public static class SimpleHttpDuck
    {
        public class RequestAction
        {
            public string Url;
        }

        [Serializable]
        public class ResponseAction
        {
            public string Url;
            public int StatusCode;
            public string Body;
        }

        public static class ActionCreator
        {
            public static RequestAction Get(string url)
            {
                return new RequestAction()
                {
                    Url = url
                };
            }
        }

        public class Middleware
        {
            public GameObject InjectGameObject { get; set; }

            public Middleware(GameObject gameObject)
            {
                this.InjectGameObject = gameObject;
            }

            public System.Func<System.Func<object, object>, System.Func<object, object>> Process(IStoreObject store)
            {
                return (System.Func<object, object> next) => (object action) =>
                {
                    if (action is RequestAction && this.InjectGameObject != null)
                    {
                        var entity = (RequestAction) action;

                        Observable.FromCoroutine<ResponseAction>(observer =>
                                Request(
                                    entity.Url,
                                    data =>
                                    {
                                        observer.OnNext(data);
                                        observer.OnCompleted();
                                    },
                                    error => observer.OnError(new Exception("Network error"))
                                )
                            )
                            .Subscribe(data => next(data), error => next(error))
                            .AddTo(this.InjectGameObject);
                    }

                    return null;
                };
            }

            private IEnumerator Request(string url, Action<ResponseAction> success, Action<string> error)
            {
                UnityWebRequest getRequest = UnityWebRequest.Get(url);

                yield return getRequest.SendWebRequest();

                if (getRequest.isNetworkError)
                {
                    error.Invoke(getRequest.error);
                }
                else
                {
                    var entity = new ResponseAction
                    {
                        Url = url,
                        Body = getRequest.downloadHandler.text,
                        StatusCode = (int) getRequest.responseCode,
                    };
                    success(entity);
                }
            }
        }

        public class Reducer : ReducerBase<State, ResponseAction>
        {
            public override State Reduce(State state, ResponseAction action)
            {
                state.Url = action.Url;
                state.StatusCode = action.StatusCode;
                state.Body = action.Body;
                state.SetStateChanged();
                return state;
            }
        }
    }
}