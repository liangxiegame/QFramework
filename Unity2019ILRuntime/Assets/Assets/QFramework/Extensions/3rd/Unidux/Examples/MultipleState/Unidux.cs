using UniRx;
using UnityEngine;

namespace Unidux.Example.MultipleState
{
    public sealed class Unidux : SingletonMonoBehaviour<Unidux>, IStoreAccessor
    {
        public TextAsset InitialStateJson;

        private Store<State> _store;

        public IStoreObject StoreObject
        {
            get { return Store; }
        }

        public static State State
        {
            get { return Store.State; }
        }

        public static Subject<State> Subject
        {
            get { return Store.Subject; }
        }

        private static IReducer[] Reducers
        {
            get {
                // Assign your reducers
                return new IReducer[] {};
            }
        }

        private static State InitialState
        {
            get
            {
                return Instance.InitialStateJson != null
                    ? UniduxSetting.Serializer.Deserialize(
                        System.Text.Encoding.UTF8.GetBytes(Instance.InitialStateJson.text),
                        typeof(State)
                    ) as State
                    : new State();
            }
        }

        public static Store<State> Store
        {
            get { return Instance._store = Instance._store ?? new Store<State>(InitialState, Reducers); }
        }

        public static object Dispatch<TAction>(TAction action)
        {
            return Store.Dispatch(action);
        }
        
        void Start()
        {
            // Assign your middlewares
            // Store.ApplyMiddlewares(SampleMiddleware.Process);
        }

        void Update()
        {
            Store.Update();
        }
    }
}
