using System;
using System.Linq;
using Unidux;
using UniRx;
using UnityEngine;

namespace QF.DVA
{
    public class DvaAction
    {
        public string Type;

        public object Payload;
    }

    public abstract class DvaModelEditor<TSelf, TState> : ReducerBase<TState, DvaAction>, IStoreAccessor
        where TState : DvaState, new() where TSelf : DvaModelEditor<TSelf, TState>, new()
    {
        public DvaModelEditor()
        {
            if (ModelDB.DB.ContainsKey(Namespace))
            {
                ModelDB.DB[Namespace] = this;
            }
            else
            {
                ModelDB.DB.Add(Namespace,this);
            }
            
            OnInit();
        }

        protected virtual void OnInit()
        {
            
        }

        protected abstract string Namespace { get; }
        
        protected abstract TState InitialState { get; }
        
        
        private static DvaModelEditor<TSelf, TState> mSingleton = null;

        private static DvaModelEditor<TSelf, TState> mInstance
        {
            get { return mSingleton = mSingleton ?? new TSelf(); }
        }

        private Store<TState> mStore;


        public static TState State
        {
            get { return Store.State; }
        }

        public static Subject<TState> Subject
        {
            get { return Store.Subject; }
        }


        public static Store<TState> Store
        {
            get { return mInstance.mStore = mInstance.mStore ?? new Store<TState>(mInstance.InitialState, mInstance); }
        }


        public IStoreObject StoreObject
        {
            get { return Store; }
        }


        public static object Dispatch(string path,object payload = null)
        {
            return Store.Dispatch(new DvaAction()
            {
                Type = path,
                Payload = payload
            });
        }

        public static void Update()
        {
            Store.Update();
        }

        protected virtual void OnDispose()
        {
            
        }

        public static void Dispose()
        {
            
            if (mSingleton != null)
            {
                mSingleton.OnDispose();
                ModelDB.DB.Remove(mSingleton.Namespace);
            }

            mSingleton = null;
        }
        
    }

    public abstract class DvaModel<TSelf, TState> :  DvaModelEditor<TSelf, TState> where TState : DvaState, new() where TSelf : DvaModel<TSelf, TState>, new()
    {
        private IDisposable mUpdate;

        protected override void OnInit()
        {
            mUpdate = Observable.EveryUpdate().Subscribe(_ => Store.Update());
        }

        protected override void OnDispose()
        {
            mUpdate.Dispose();
        }
    }
}