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
    // v1 No.163
    [ClassAPI("03.SingletonKit", "MonoSingleton<T>", 0,"MonoSingleton<T>")]
    [APIDescriptionCN("MonoBehaviour 单例类")]
    [APIDescriptionEN("MonoBehavior Singleton Class")]
    [APIExampleCode(@"
public class GameManager : MonoSingleton<GameManager>
{
    public override void OnSingletonInit()
    {
        Debug.Log(name + "":"" + ""OnSingletonInit"");
    }

    private void Awake()
    {
        Debug.Log(name + "":"" + ""Awake"");
    }

    private void Start()
    {
        Debug.Log(name + "":"" + ""Start"");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
			
        Debug.Log(name + "":"" + ""OnDestroy"");
    }
}

var gameManager = GameManager.Instance;
// GameManager:OnSingletonInit
// GameManager:Awake
// GameManager:Start
// ---------------------
// GameManager:OnDestroy
")]
#endif
    public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T>
    {
        /// <summary>
        /// 静态实例
        /// </summary>
        protected static T mInstance;

        /// <summary>
        /// 静态属性：封装相关实例对象
        /// </summary>
        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = SingletonCreator.CreateMonoSingleton<T>();
                }

                return mInstance;
            }
        }

        /// <summary>
        /// 实现接口的单例初始化
        /// </summary>
        public virtual void OnSingletonInit()
        {
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public virtual void Dispose()
        {
            if (SingletonCreator.IsUnitTestMode)
            {
                var curTrans = transform;
                do
                {
                    var parent = curTrans.parent;
                    DestroyImmediate(curTrans.gameObject);
                    curTrans = parent;
                } while (curTrans != null);

                mInstance = null;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        

        /// <summary>
        /// 应用程序退出：释放当前对象并销毁相关GameObject
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            if (mInstance == null) return;
            Destroy(mInstance.gameObject);
            mInstance = null;
        }

        /// <summary>
        /// 释放当前对象
        /// </summary>
        protected virtual void OnDestroy()
        {
            mInstance = null;
        }
    }
}