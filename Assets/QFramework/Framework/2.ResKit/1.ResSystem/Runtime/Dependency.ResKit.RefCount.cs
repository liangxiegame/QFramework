using System.Collections.Generic;
using UnityEngine;

namespace Dependency.ResKit.RefCount
{
    public interface IRefCounter
    {
        int RefCount { get; }

        void Retain(object refOwner = null);
        void Release(object refOwner = null);
    }

    public class SimpleRC : IRefCounter
    {
        public SimpleRC()
        {
            RefCount = 0;
        }

        public int RefCount { get; private set; }

        public void Retain(object refOwner = null)
        {
            ++RefCount;
        }

        public void Release(object refOwner = null)
        {
            --RefCount;
            if (RefCount == 0)
            {
                OnZeroRef();
            }
        }

        protected virtual void OnZeroRef()
        {
        }
    }
    
    public sealed class SafeARC : IRefCounter
    {
        public int RefCount
        {
            get { return mOwners.Count; }
        }

        public HashSet<object> Owners
        {
            get { return mOwners; }
        }

        readonly HashSet<object> mOwners = new HashSet<object>();

        public void Retain(object refOwner)
        {
            if (!Owners.Add(refOwner))
            {
                Debug.LogError("ObjectIsAlreadyRetainedByOwnerException");
            }
        }

        public void Release(object refOwner)
        {
            if (!Owners.Remove(refOwner))
            {
                Debug.LogError("ObjectIsNotRetainedByOwnerExceptionWithHint");
            }
        }
    }

    public sealed class UnsafeARC : SimpleRC
    {
        
    }
}