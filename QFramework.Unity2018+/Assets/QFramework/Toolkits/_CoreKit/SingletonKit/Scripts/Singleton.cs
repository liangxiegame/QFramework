/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

namespace QFramework
{
#if UNITY_EDITOR
    // v1 No.164
    [ClassAPI("03.SingletonKit", "Singleton<T>", 1,"Singleton<T>")]
    [APIDescriptionCN("C# 单例类")]
    [APIDescriptionEN("Pure C# Singleton Class")]
    [APIExampleCode(@"
public class GameDataManager : Singleton<GameDataManager>
{
    private static int mIndex = 0;

    private Class2Singleton() {}

    public override void OnSingletonInit()
    {
        mIndex++;
    }

    public void Log(string content)
    {
        Debug.Log(""GameDataManager"" + mIndex + "":"" + content);
    }
}

GameDataManager.Instance.Log(""Hello"");
// GameDataManager1:OnSingletonInit:Hello
GameDataManager.Instance.Log(""Hello"");
// GameDataManager1:OnSingletonInit:Hello
GameDataManager.Instance.Dispose();
")]
#endif
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>
    {
        /// <summary>
        /// 静态实例
        /// </summary>
        protected static T mInstance;

        /// <summary>
        /// 标签锁：确保当一个线程位于代码的临界区时，另一个线程不进入临界区。
        /// 如果其他线程试图进入锁定的代码，则它将一直等待（即被阻止），直到该对象被释放
        /// </summary>
        static object mLock = new object();

        /// <summary>
        /// 静态属性
        /// </summary>
        public static T Instance
        {
            get
            {
                lock (mLock)
                {
                    if (mInstance == null)
                    {
                        mInstance = SingletonCreator.CreateSingleton<T>();
                    }
                }

                return mInstance;
            }
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public virtual void Dispose()
        {
            mInstance = null;
        }

        /// <summary>
        /// 单例初始化方法
        /// </summary>
        public virtual void OnSingletonInit()
        {
        }
    }
}