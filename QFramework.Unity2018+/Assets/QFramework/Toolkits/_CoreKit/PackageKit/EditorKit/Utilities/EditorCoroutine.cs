using System;

#if UNITY_EDITOR
namespace QFramework
{
    using System.Collections;
    using UnityEditor;
    
    // https://gist.github.com/benblo/10732554
    public class EditorCoroutine
    {

        public static EditorCoroutine Start(IEnumerator routine, Action onFinish = null)
        {
            EditorCoroutine coroutine = new EditorCoroutine(routine,onFinish);
            coroutine.Start();
            return coroutine;
        }

        readonly IEnumerator mRoutine;
        public Action OnFinish { get; }

        EditorCoroutine(IEnumerator routine, Action onFinish)
        {
            OnFinish = onFinish;
            mRoutine = routine;
        }

        void Start()
        {
            //Debug.Log("start");
            EditorApplication.update += Update;
        }

        public void Stop()
        {
            //Debug.Log("stop");
            EditorApplication.update -= Update;
        }

        void Update()
        {
            /* NOTE: no need to try/catch MoveNext,
             * if an IEnumerator throws its next iteration returns false.
             * Also, Unity probably catches when calling EditorApplication.update.
             */

            //Debug.Log("update");
            if (!mRoutine.MoveNext())
            {
                Stop();
                OnFinish?.Invoke();
            }
        }
    }
}
#endif