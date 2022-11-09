/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
#if UNITY_EDITOR
    // v1 No.168
    [ClassAPI("03.SingletonKit", "PersistentMonoSingleton<T>", 5, "PersistentMonoSingleton<T>")]
    [APIDescriptionCN("当场景里包含两个 PersistentMonoSingleton，保留先创建的")]
    [APIDescriptionEN("when a scenario contains two PersistentMonoSingleton, retain the one that was created first")]
    [APIExampleCode(@"
public class GameManager : PersistentMonoSingleton<GameManager>
{
 
}
 
IEnumerator Start()
{
    var gameManager = GameManager.Instance;
 
    var newGameManager = new GameObject().AddComponent<GameManager>();
 
    yield return new WaitForEndOfFrame();
 
    Debug.Log(FindObjectOfTypes<GameManager>().Length);
    // 1
    Debug.Log(gameManager == null);
    // false
    Debug.Log(newGameManager == null);
    // true
}
")]
#endif
    public abstract class PersistentMonoSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T mInstance;
        protected bool mEnabled;

        public static T Instance
        {
            get
            {
                if (mInstance != null) return mInstance;
                mInstance = FindObjectOfType<T>();
                if (mInstance != null) return mInstance;
                var obj = new GameObject();
                mInstance = obj.AddComponent<T>();
                return mInstance;
            }
        }

        protected virtual void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (mInstance == null)
            {
                mInstance = this as T;
                DontDestroyOnLoad(transform.gameObject);
                mEnabled = true;
            }
            else
            {
                if (this != mInstance)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}