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
    // v1 No.166
    [ClassAPI("03.SingletonKit", "SingletonProperty<T>", 3,"SingletonProperty<T>")]
    [APIDescriptionCN("通过属性实现的 Singleton")]
    [APIDescriptionEN("Singleton implemented through properties")]
    [APIExampleCode(@"
public class GameDataManager : ISingleton
{
    public static GameDataManager Instance
    {
        get { return SingletonProperty<GameDataManager>.Instance; }
    }

    private GameDataManager() {}
		
    private static int mIndex = 0;

    public void OnSingletonInit()
    {
        mIndex++;
    }

    public void Dispose()
    {
        SingletonProperty<GameDataManager>.Dispose();
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
    public static class SingletonProperty<T> where T : class, ISingleton
    {
        /// <summary>
        /// 静态实例
        /// </summary>
        private static T mInstance;

        /// <summary>
        /// 标签锁
        /// </summary>
        private static readonly object mLock = new object();

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
        public static void Dispose()
        {
            mInstance = null;
        }
    }
}