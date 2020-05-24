/****************************************************************************
 * Copyright (c) 2017 ~ 2019.11 liangxie
 * https://github.com/thefuntastic/unity-object-pool
 * https://github.com/prime31/RecyclerKit
 * https://github.com/prime31
 * https://github.com/ihaiucom/ihaiu.PoolManager
 * https://github.com/ihaiucom?tab=repositories
 * https://github.com/Seneral/UndoPro
 * https://github.com/SpaceMadness/lunar-unity-plugin
 * https://github.com/jjcat/JumpCommand
 * https://github.com/appetizermonster/Unity3D-ActionEngine
 * http://robertpenner.com/easing/
 * http://robotacid.com/documents/code/Easing.cs
 * https://github.com/appetizermonster/Unity3D-Co/blob/master/Assets/Co/Co.cs
 * https://github.com/modesttree/Zenject
 * https://github.com/RyanNielson/awesome-unity
 * https://github.com/appetizermonster/Unity3D-RecompileDisabler
 * https://github.com/Demigiant
 * https://github.com/CatLib
 * https://github.com/ketoo/NoahGameFrame
 * https://github.com/ketoo
 * https://github.com/MFatihMAR/UnityGAME
 * https://github.com/MFatihMAR
 * https://github.com/David-Desmaisons/EasyActor
 * https://github.com/carbers/UnityComponent
 * https://github.com/mr-kelly/KSFramework
 * https://github.com/EllanJiang/GameFramework
 * https://github.com/Ourpalm/ILRuntime
 * https://github.com/Ourpalm/ILRuntimeU3D
 * https://github.com/andoowhy/EgoCS/blob/master/Ego.cs
 * https://github.com/doukasd/Unity-Components
 * https://github.com/GreatArcStudios/unitypausemenu
 * https://github.com/Thundernerd/Unity3D-ComponentAttribute
 * https://github.com/ChemiKhazi/ReorderableInspector
 * https://github.com/bkeiren/AwesomiumUnity
 * http://www.rivellomultimediaconsulting.com/unity3d-csharp-design-patterns/
 * http://www.rivellomultimediaconsulting.com/unity3d-applicant-tests-11/
 * http://www.rivellomultimediaconsulting.com/
 * http://www.rivellomultimediaconsulting.com/unity3d-mvcs-architectures-strangeioc-2/
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;

namespace QFramework
{
	public interface IPool<T>
    {
        T Allocate();

        bool Recycle(T obj);
    }
    
    public abstract class Pool<T> : IPool<T>,ICountObserveAble
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
    
    /// <summary>
	/// Object pool 4 class who no public constructor
	/// such as SingletonClass.QEventSystem
	/// </summary>
	public class NonPublicObjectPool<T> :Pool<T>,ISingleton where T : class,IPoolable
	{
		#region Singleton
		public void OnSingletonInit(){}
		
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
    
        /// <summary>
    /// I cache type.
    /// </summary>
    public interface IPoolType
    {
        void Recycle2Cache();
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
    /// Count observer able.
    /// </summary>
    public interface ICountObserveAble
    {
        int CurCount { get; }
    }

    /// <summary>
    /// Object pool.
    /// </summary>
    public class SafeObjectPool<T> : Pool<T>, ISingleton where T : IPoolable, new()
    {
        #region Singleton
        void ISingleton.OnSingletonInit() {}

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
    /// Unity 游戏框架搭建 (十九) 简易对象池：http://qframework.io/post/24/ 的例子
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleObjectPool<T> : Pool<T>
    {
	    readonly Action<T> mResetMethod;

	    public SimpleObjectPool(Func<T> factoryMethod, Action<T> resetMethod = null,int initCount = 0)
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

    public interface IObjectFactory<T>
    {
	    T Create();
    }
    
    public class DefaultObjectFactory<T> : IObjectFactory<T> where T : new()
    {
	    public T Create()
	    {
		    return new T();
	    }
    }
    
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
    public class NonPublicObjectFactory<T> : IObjectFactory<T> where T : class
    {
	    public T Create()
	    {
		    var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
		    var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
		    return ctor.Invoke(null) as T;
	    }
    }
}