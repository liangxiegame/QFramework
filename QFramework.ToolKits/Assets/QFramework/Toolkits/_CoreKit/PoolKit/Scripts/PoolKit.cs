using System;
using System.Collections.Generic;
using System.Reflection;

namespace QFramework
{
    #region API

    /// <summary>
    /// Unity 游戏框架搭建 (十九) 简易对象池：http://qframework.io/post/24/ 的例子
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleObjectPool<T> : Pool<T>
    {
        readonly Action<T> mResetMethod;

        public SimpleObjectPool(Func<T> factoryMethod, Action<T> resetMethod = null, int initCount = 0)
        {
            mFactory = new CustomObjectFactory<T>(factoryMethod);
            mResetMethod = resetMethod;

            for (int i = 0; i < initCount; i++)
            {
                mCacheStack.Push(mFactory.Create());
            }
        }

        public override bool Recycle(T obj)
        {
            if (mResetMethod != null)
            {
                mResetMethod.Invoke(obj);
            }

            mCacheStack.Push(obj);
            return true;
        }
    }

    /// <summary>
    /// Object pool.
    /// </summary>
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

        public static SafeObjectPool<T> Instance
        {
            get { return SingletonProperty<SafeObjectPool<T>>.Instance; }
        }

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
                    Recycle(new T());
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

    /// <summary>
    /// Object pool 4 class who no public constructor
    /// such as SingletonClass.QEventSystem
    /// </summary>
    public class NonPublicObjectPool<T> : Pool<T>, ISingleton where T : class, IPoolable
    {
        #region Singleton

        public void OnSingletonInit()
        {
        }

        public static NonPublicObjectPool<T> Instance
        {
            get { return SingletonProperty<NonPublicObjectPool<T>>.Instance; }
        }

        protected NonPublicObjectPool()
        {
            mFactory = new NonPublicObjectFactory<T>();
        }

        public void Dispose()
        {
            SingletonProperty<NonPublicObjectPool<T>>.Dispose();
        }

        #endregion

        /// <summary>
        /// Init the specified maxCount and initCount.
        /// </summary>
        /// <param name="maxCount">Max Cache count.</param>
        /// <param name="initCount">Init Cache count.</param>
        public void Init(int maxCount, int initCount)
        {
            if (maxCount > 0)
            {
                initCount = Math.Min(maxCount, initCount);
            }

            if (CurCount >= initCount) return;

            for (var i = CurCount; i < initCount; ++i)
            {
                Recycle(mFactory.Create());
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

                if (mCacheStack == null) return;
                if (mMaxCount <= 0) return;
                if (mMaxCount >= mCacheStack.Count) return;
                var removeCount = mMaxCount - mCacheStack.Count;
                while (removeCount > 0)
                {
                    mCacheStack.Pop();
                    --removeCount;
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

    public abstract class AbstractPool<T> where T : AbstractPool<T>, new()
    {
        private static Stack<T> mPool = new Stack<T>(10);

        protected bool mInPool = false;

        public static T Allocate()
        {
            var node = mPool.Count == 0 ? new T() : mPool.Pop();
            node.mInPool = false;
            return node;
        }

        public void Recycle2Cache()
        {
            OnRecycle();
            mInPool = true;
            mPool.Push(this as T);
        }

        protected abstract void OnRecycle();
    }

    #endregion

    #region interfaces

    /// <summary>
    /// 对象池接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPool<T>
    {
        /// <summary>
        /// 分配对象
        /// </summary>
        /// <returns></returns>
        T Allocate();

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool Recycle(T obj);
    }

    /// <summary>
    /// I pool able.
    /// </summary>
    public interface IPoolable
    {
        void OnRecycled();
        bool IsRecycled { get; set; }
    }

    /// <summary>
    /// I cache type.
    /// </summary>
    public interface IPoolType
    {
        void Recycle2Cache();
    }

    /// <summary>
    /// 对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Pool<T> : IPool<T>
    {
        #region ICountObserverable

        /// <summary>
        /// Gets the current count.
        /// </summary>
        /// <value>The current count.</value>
        public int CurCount
        {
            get { return mCacheStack.Count; }
        }

        #endregion

        protected IObjectFactory<T> mFactory;

        /// <summary>
        /// 存储相关数据的栈
        /// </summary>
        protected readonly Stack<T> mCacheStack = new Stack<T>();

        /// <summary>
        /// default is 5
        /// </summary>
        protected int mMaxCount = 12;

        public virtual T Allocate()
        {
            return mCacheStack.Count == 0
                ? mFactory.Create()
                : mCacheStack.Pop();
        }

        public abstract bool Recycle(T obj);
    }

    #endregion

    #region DataStructurePool

    /// <summary>
    /// 字典对象池：用于存储相关对象
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DictionaryPool<TKey, TValue>
    {
        /// <summary>
        /// 栈对象：存储多个字典
        /// </summary>
        static Stack<Dictionary<TKey, TValue>> mListStack = new Stack<Dictionary<TKey, TValue>>(8);

        /// <summary>
        /// 出栈：从栈中获取某个字典数据
        /// </summary>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> Get()
        {
            if (mListStack.Count == 0)
            {
                return new Dictionary<TKey, TValue>(8);
            }

            return mListStack.Pop();
        }

        /// <summary>
        /// 入栈：将字典数据存储到栈中 
        /// </summary>
        /// <param name="toRelease"></param>
        public static void Release(Dictionary<TKey, TValue> toRelease)
        {
            toRelease.Clear();
            mListStack.Push(toRelease);
        }
    }

    /// <summary>
    /// 对象池字典 拓展方法类
    /// </summary>
    public static class DictionaryPoolExtensions
    {
        /// <summary>
        /// 对字典拓展 自身入栈 的方法
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="toRelease"></param>
        public static void Release2Pool<TKey, TValue>(this Dictionary<TKey, TValue> toRelease)
        {
            DictionaryPool<TKey, TValue>.Release(toRelease);
        }
    }

    /// <summary>
    /// 链表对象池：存储相关对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ListPool<T>
    {
        /// <summary>
        /// 栈对象：存储多个List
        /// </summary>
        static Stack<List<T>> mListStack = new Stack<List<T>>(8);

        /// <summary>
        /// 出栈：获取某个List对象
        /// </summary>
        /// <returns></returns>
        public static List<T> Get()
        {
            if (mListStack.Count == 0)
            {
                return new List<T>(8);
            }

            return mListStack.Pop();
        }

        /// <summary>
        /// 入栈：将List对象添加到栈中
        /// </summary>
        /// <param name="toRelease"></param>
        public static void Release(List<T> toRelease)
        {
            toRelease.Clear();
            mListStack.Push(toRelease);
        }
    }

    /// <summary>
    /// 链表对象池 拓展方法类
    /// </summary>
    public static class ListPoolExtensions
    {
        /// <summary>
        /// 给List拓展 自身入栈 的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toRelease"></param>
        public static void Release2Pool<T>(this List<T> toRelease)
        {
            ListPool<T>.Release(toRelease);
        }
    }

    #endregion

    #region Factories

    /// <summary>
    /// 对象工厂
    /// </summary>
    public class ObjectFactory
    {
        /// <summary>
        /// 动态创建类的实例：创建有参的构造函数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="constructorArgs"></param>
        /// <returns></returns>
        public static object Create(Type type, params object[] constructorArgs)
        {
            return Activator.CreateInstance(type, constructorArgs);
        }

        /// <summary>
        /// 动态创建类的实例：泛型扩展
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="constructorArgs"></param>
        /// <returns></returns>
        public static T Create<T>(params object[] constructorArgs)
        {
            return (T)Create(typeof(T), constructorArgs);
        }

        /// <summary>
        /// 动态创建类的实例：创建无参/私有的构造函数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object CreateNonPublicConstructorObject(Type type)
        {
            // 获取私有构造函数
            var constructorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

            // 获取无参构造函数
            var ctor = Array.Find(constructorInfos, c => c.GetParameters().Length == 0);

            if (ctor == null)
            {
                throw new Exception("Non-Public Constructor() not found! in " + type);
            }

            return ctor.Invoke(null);
        }

        /// <summary>
        /// 动态创建类的实例：创建无参/私有的构造函数  泛型扩展
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateNonPublicConstructorObject<T>()
        {
            return (T)CreateNonPublicConstructorObject(typeof(T));
        }

        /// <summary>
        /// 创建带有初始化回调的 对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="onObjectCreate"></param>
        /// <param name="constructorArgs"></param>
        /// <returns></returns>
        public static object CreateWithInitialAction(Type type, Action<object> onObjectCreate,
            params object[] constructorArgs)
        {
            var obj = Create(type, constructorArgs);
            onObjectCreate(obj);
            return obj;
        }

        /// <summary>
        /// 创建带有初始化回调的 对象：泛型扩展
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="onObjectCreate"></param>
        /// <param name="constructorArgs"></param>
        /// <returns></returns>
        public static T CreateWithInitialAction<T>(Action<T> onObjectCreate,
            params object[] constructorArgs)
        {
            var obj = Create<T>(constructorArgs);
            onObjectCreate(obj);
            return obj;
        }
    }

    /// <summary>
    /// 自定义对象工厂：相关对象是 自己定义 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomObjectFactory<T> : IObjectFactory<T>
    {
        public CustomObjectFactory(Func<T> factoryMethod)
        {
            mFactoryMethod = factoryMethod;
        }

        protected Func<T> mFactoryMethod;

        public T Create()
        {
            return mFactoryMethod();
        }
    }

    /// <summary>
    /// 默认对象工厂：相关对象是通过New 出来的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultObjectFactory<T> : IObjectFactory<T> where T : new()
    {
        public T Create()
        {
            return new T();
        }
    }

    /// <summary>
    /// 对象工厂接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectFactory<T>
    {
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <returns></returns>
        T Create();
    }

    /// <summary>
    /// 没有公共构造函数的对象工厂：相关对象只能通过反射获得
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NonPublicObjectFactory<T> : IObjectFactory<T> where T : class
    {
        public T Create()
        {
            var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);

            if (ctor == null)
            {
                throw new Exception("Non-Public Constructor() not found! in " + typeof(T) + "\n 在没有找到非 public 的构造方法");
            }

            return ctor.Invoke(null) as T;
        }
    }

    #endregion
}