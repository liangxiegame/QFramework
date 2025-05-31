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
    // v1 No.169
    [ClassAPI("03.SingletonKit", "ReplaceableMonoSingleton<T>", 5, "ReplaceableMonoSingleton<T>")]
    [APIDescriptionCN("当场景里包含两个 ReplaceableMonoSingleton，保留最后创建的")]
    [APIDescriptionEN("When the scene contains two ReplaceableMonoSingleton, keep the last one created")]
    [APIExampleCode(@"
public class GameManager : ReplaceableMonoSingleton<GameManager>
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
    // true
    Debug.Log(newGameManager == null);
    // false
}
")]
#endif
    public class ReplaceableMonoSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T mInstance;
        
        public float InitializationTime;
        
        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = FindObjectOfType<T>();
                    if (mInstance == null)
                    {
                        var obj = new GameObject
                        {
                            hideFlags = HideFlags.HideAndDontSave
                        };
                        mInstance = obj.AddComponent<T>();
                    }
                }

                return mInstance;
            }
        }
        
        protected virtual void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            InitializationTime = Time.time;

            DontDestroyOnLoad(this.gameObject);

            var check = FindObjectsOfType<T>();
            foreach (var searched in check)
            {
                if (searched == this) continue;
                if (searched.GetComponent<ReplaceableMonoSingleton<T>>().InitializationTime < InitializationTime)
                {
                    Destroy(searched.gameObject);
                }
            }

            if (mInstance == null)
            {
                mInstance = this as T;
            }
        }
    }
}