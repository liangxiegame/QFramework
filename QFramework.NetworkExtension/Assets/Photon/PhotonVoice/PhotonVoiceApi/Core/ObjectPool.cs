using System;

namespace Photon.Voice
{
    /// <summary>
    /// Generic Pool to re-use objects of a certain type (TType) that optionally match a certain property or set of properties (TInfo).
    /// </summary>
    /// <typeparam name="TType">Object type.</typeparam>
    /// <typeparam name="TInfo">Type of parameter used to check 2 objects identity (like integral length of array).</typeparam>
    public abstract class ObjectPool<TType, TInfo> : IDisposable
    {
        protected int capacity;
        protected TInfo info;
        private TType[] freeObj = new TType[0];
        protected int pos;
        protected string name;
        private bool inited;
        abstract protected TType createObject(TInfo info);
        abstract protected void destroyObject(TType obj);
        abstract protected bool infosMatch(TInfo i0, TInfo i1);
        internal string LogPrefix { get { return "[ObjectPool] [" + name + "]"; } }

        /// <summary>Create a new ObjectPool instance. Does not call Init().</summary>
        /// <param name="capacity">Capacity (size) of the object pool.</param>
        /// <param name="name">Name of the object pool.</param>
        public ObjectPool(int capacity, string name)
        {
            this.capacity = capacity;
            this.name = name;
        }

        /// <summary>Create a new ObjectPool instance with the given info structure. Calls Init().</summary>
        /// <param name="capacity">Capacity (size) of the object pool.</param>
        /// <param name="name">Name of the object pool.</param>
        /// <param name="info">Info about this Pool's objects.</param>
        public ObjectPool(int capacity, string name, TInfo info)
        {
            this.capacity = capacity;
            this.name = name;
            Init(info);
        }

        /// <summary>(Re-)Initializes this ObjectPool.</summary>
        /// If there are objects available in this Pool, they will be destroyed.
        /// Allocates (Capacity) new Objects.
        /// <param name="info">Info about this Pool's objects.</param>
        public void Init(TInfo info)
        {
            lock (this)
            {
                while (pos > 0)
                {
                    destroyObject(freeObj[--pos]);
                }
                this.info = info;
                this.freeObj = new TType[capacity];
                inited = true;
            }
        }

        /// <summary>The property (info) that objects in this Pool must match.</summary>
        public TInfo Info
        {
            get { return info; }
        }

        /// <summary>Acquire an existing object, or create a new one if none are available.</summary>
        /// <remarks>If it fails to get one from the pool, this will create from the info given in this pool's constructor.</remarks>
        public TType AcquireOrCreate()
        {
            lock (this)
            {
                if (pos > 0)
                {
                    return freeObj[--pos];
                }
                if (!inited)
                {
                    throw new Exception(LogPrefix + " not initialized");
                }
            }
            return createObject(this.info);
        }

        /// <summary>Acquire an existing object (if info matches), or create a new one from the passed info.</summary>
        /// <param name="info">Info structure to match, or create a new object with.</param>
        public TType AcquireOrCreate(TInfo info)
        {
            // TODO: this.info thread safety
            if (!infosMatch(this.info, info))
            {
                Init(info);
            }
            return AcquireOrCreate();
        }

        /// <summary>Returns object to pool.</summary>
        /// <param name="obj">The object to return to the pool.</param>
        /// <param name="objInfo">The info structure about obj.</param>
        /// <remarks>obj is returned to the pool only if objInfo matches this pool's info. Else, it is destroyed.</remarks>
        virtual public bool Release(TType obj, TInfo objInfo)
        {
            // TODO: this.info thread safety
            if (infosMatch(this.info, objInfo))
            {
                lock (this)
                {
                    if (pos < freeObj.Length)
                    {
                        freeObj[pos++] = obj;
                        return true;
                    }
                }
            }

            // destroy if can't reuse
            //UnityEngine.Debug.Log(LogPrefix + " Release(Info) destroy");
            destroyObject(obj);
            // TODO: log warning
            return false;
        }

        /// <summary>Returns object to pool, or destroys it if the pool is full.</summary>
        /// <param name="obj">The object to return to the pool.</param>
        virtual public bool Release(TType obj)
        {
            lock (this)
            {
                if (pos < freeObj.Length)
                {
                    freeObj[pos++] = obj;
                    return true;
                }
            }

            // destroy if can't reuse
            //UnityEngine.Debug.Log(LogPrefix + " Release destroy " + pos);
            destroyObject(obj);
            // TODO: log warning
            return false;
        }

        /// <summary>Free resources assoicated with this ObjectPool</summary>
        public void Dispose()
        {
            lock (this)
            {
                while (pos > 0)
                {
                    destroyObject(freeObj[--pos]);
                }
                freeObj = new TType[0];
            }
        }
    }

    /// <summary>
    /// Pool of Arrays with components of type T, with ObjectPool info being the array's size.
    /// </summary>
    /// <typeparam name="T">Array element type.</typeparam>
    public class PrimitiveArrayPool<T> : ObjectPool<T[], int>
    {
        public PrimitiveArrayPool(int capacity, string name) : base(capacity, name) { }
        public PrimitiveArrayPool(int capacity, string name, int info) : base(capacity, name, info) { }
        protected override T[] createObject(int info)
        {
            //UnityEngine.Debug.Log(LogPrefix + " Create " + pos);
            return new T[info];
        }

        protected override void destroyObject(T[] obj)
        {
            //UnityEngine.Debug.Log(LogPrefix + " Dispose " + pos + " " + obj.GetHashCode());
        }

        protected override bool infosMatch(int i0, int i1)
        {
            return i0 == i1;
        }
    }

    public class ImageBufferNativePool<T> : ObjectPool<T, ImageBufferInfo> where T : ImageBufferNative
    {
        public delegate T Factory(ImageBufferNativePool<T> pool, ImageBufferInfo info);
        Factory factory;
        public ImageBufferNativePool(int capacity, Factory factory, string name) : base(capacity, name)
        {
            this.factory = factory;
        }
        public ImageBufferNativePool(int capacity, Factory factory, string name, ImageBufferInfo info) : base(capacity, name, info)
        {
            this.factory = factory;
        }
        protected override T createObject(ImageBufferInfo info)
        {
            //UnityEngine.Debug.Log(LogPrefix + " Create " + pos);
            return factory(this, info);
        }

        protected override void destroyObject(T obj)
        {
            //UnityEngine.Debug.Log(LogPrefix + " Dispose " + pos + " " + obj.GetHashCode());
            obj.Dispose();
        }

        // only height and stride compared, other parameters do not affect native buffers and can be simple overwritten
        protected override bool infosMatch(ImageBufferInfo i0, ImageBufferInfo i1)
        {
            if (i0.Height != i1.Height)
            {
                return false;
            }
            var s0 = i0.Stride;
            var s1 = i1.Stride;
            if (s0.Length != s1.Length)
            {
                return false;
            }
            switch (i0.Stride.Length)
            {
                // most common case are 1 and 3 planes
                case 1:
                    return s0[0] == s1[0];
                case 2:
                    return s0[0] == s1[0] && s0[1] == s1[1];
                case 3:
                    return s0[0] == s1[0] && s0[1] == s1[1] && s0[2] == s1[2];
                default:
                    for (int i = 0; i < s0.Length; i++)
                    {
                        if (s0[i] != s1[i])
                        {
                            return false;
                        }
                    }
                    return true;
            }
        }
    }
}
