using System.Collections.Generic;

namespace QFramework
{
    public abstract class ILPool<T> where T : ILPool<T>, new()
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
}