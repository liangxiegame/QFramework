/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;

namespace QFramework
{
#if UNITY_EDITOR
    // v1 No.173
    [ClassAPI("06.PoolKit", "SafeObjectPool<T>", 3, "SafeObjectPool<T>")]
    [APIDescriptionCN("更安全的对象池，带有一定的约束。")]
    [APIDescriptionEN("More secure object pooling, with certain constraints.")]
    [APIExampleCode(@"
class Bullet :IPoolable,IPoolType
{
    public void OnRecycled()
    {
        Debug.Log(""回收了"");
    }
 
    public  bool IsRecycled { get; set; }
 
    public static Bullet Allocate()
    {
        return SafeObjectPool<Bullet>.Instance.Allocate();
    }
             
    public void Recycle2Cache()
    {
        SafeObjectPool<Bullet>.Instance.Recycle(this);
    }
}
 
SafeObjectPool<Bullet>.Instance.Init(50,25);
             
var bullet = Bullet.Allocate();
 
Debug.Log(SafeObjectPool<Bullet>.Instance.CurCount);
             
bullet.Recycle2Cache();
 
Debug.Log(SafeObjectPool<Bullet>.Instance.CurCount);
 
// can config object factory
// 可以配置对象工厂
SafeObjectPool<Bullet>.Instance.SetFactoryMethod(() =>
{
    // bullet can be mono behaviour
    return new Bullet();
});
             
SafeObjectPool<Bullet>.Instance.SetObjectFactory(new DefaultObjectFactory<Bullet>());
 
// can set
// 可以设置
// NonPublicObjectFactory: 可以通过调用私有构造来创建对象,can call private constructor to create object
// CustomObjectFactory: 自定义创建对象的方式,can create object by Func<T>
// DefaultObjectFactory: 通过 new 创建对象, can create object by new 
")]
#endif
    public class SafeObjectPool<T> : Pool<T>, ISingleton where T : IPoolable, new()
    {
        #region Singleton

        void ISingleton.OnSingletonInit()
        {
        }

        protected SafeObjectPool()
        {
            mFactory = new DefaultObjectFactory<T>();
        }

        public static SafeObjectPool<T> Instance => SingletonProperty<SafeObjectPool<T>>.Instance;

        public void Dispose()
        {
            SingletonProperty<SafeObjectPool<T>>.Dispose();
        }

        #endregion

        /// <summary>
        /// Init the specified maxCount and initCount.
        /// </summary>
        /// <param name="maxCount">Max Cache count.</param>
        /// <param name="initCount">Init Cache count.</param>
        /// <param name="objectFactory">Object Factory.</param>
        public void Init(int maxCount, int initCount)
        {
            MaxCacheCount = maxCount;

            if (maxCount > 0)
            {
                initCount = Math.Min(maxCount, initCount);
            }

            if (CurCount < initCount)
            {
                for (var i = CurCount; i < initCount; ++i)
                {
                    Recycle(mFactory.Create());
                }
            }
        }

        /// <summary>
        /// Gets or sets the max cache count.
        /// </summary>
        /// <value>The max cache count.</value>
        public int MaxCacheCount
        {
            get { return mMaxCount; }
            set
            {
                mMaxCount = value;

                if (mCacheStack != null)
                {
                    if (mMaxCount > 0)
                    {
                        if (mMaxCount < mCacheStack.Count)
                        {
                            int removeCount = mCacheStack.Count - mMaxCount;
                            while (removeCount > 0)
                            {
                                mCacheStack.Pop();
                                --removeCount;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Allocate T instance.
        /// </summary>
        public override T Allocate()
        {
            var result = base.Allocate();
            result.IsRecycled = false;
            return result;
        }

        /// <summary>
        /// Recycle the T instance
        /// </summary>
        /// <param name="t">T.</param>
        public override bool Recycle(T t)
        {
            if (t == null || t.IsRecycled)
            {
                return false;
            }

            if (mMaxCount > 0)
            {
                if (mCacheStack.Count >= mMaxCount)
                {
                    t.OnRecycled();
                    return false;
                }
            }

            t.IsRecycled = true;
            t.OnRecycled();
            mCacheStack.Push(t);

            return true;
        }
    }
}