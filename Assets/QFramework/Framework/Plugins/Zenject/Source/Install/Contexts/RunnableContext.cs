using ModestTree;
using UnityEngine;

#if !NOT_UNITY3D

namespace Zenject
{
    public abstract class RunnableContext : Context
    {
        [Tooltip("When false, wait until run method is explicitly called. Otherwise run on initialize")]
        [SerializeField]
        bool _autoRun = true;

        static bool _staticAutoRun = true;

        public bool Initialized { get; private set; }

        protected void Initialize()
        {
            if (_staticAutoRun && _autoRun)
            {
                Run();
            }
            else
            {
                // True should always be default
                _staticAutoRun = true;
            }
        }

        public void Run()
        {
            Assert.That(!Initialized,
                "The context already has been initialized!");

            RunInternal();

            Initialized = true;
        }

        protected abstract void RunInternal();

        public static T CreateComponent<T>(GameObject gameObject) where T : RunnableContext
        {
            _staticAutoRun = false;

            var result = gameObject.AddComponent<T>();
            Assert.That(_staticAutoRun); // Should be reset
            return result;
        }
    }
}

#endif
