using System;
using System.Linq;
using UnityEngine;

namespace QFramework
{
    public abstract class ILComponent : ICanAddILComponent2GameObject ,ICanGetILComponentFromGameObject
    {
        public Transform transform { get; private set; }
        public GameObject gameObject { get; private set; }
        

        protected void Destroy()
        {
            transform = null;
            gameObject = null;
            OnDestroy();
        }

        protected abstract void OnStart();
        protected abstract void OnDestroy();
        
        void ICanAddILComponent2GameObject.SetGameObject(GameObject gameObj)
        {
            gameObject = gameObj;
            transform = gameObj.transform;

            var componentBehaviour = gameObject.AddComponent<ILComponentBehaviour>();
            componentBehaviour.Script = this;
            componentBehaviour.OnDestroyAction += Destroy;

            OnStart();
        }
    }

    /// <summary>
    /// 可以从 GameObject
    /// </summary>
    public interface ICanGetILComponentFromGameObject
    {
        
    }
    
    public interface ICanAddILComponent2GameObject
    {
        void SetGameObject(GameObject gameObj);
    }
    


    public static class ILComponentExtensions
    {
        public static T AddILComponent<T>(this Component component) where T : ICanAddILComponent2GameObject, new()
        {
            var t = new T();
            t.SetGameObject(component.gameObject);
            return t;
        }

        public static T AddILComponent<T>(this GameObject gameObject) where T : ICanAddILComponent2GameObject, new()
        {
            var t = new T();
            t.SetGameObject(gameObject);
            return t;
        }

        public static T GetILComponent<T>(this GameObject gameObject) where T : class, ICanGetILComponentFromGameObject
        {
            return (T) gameObject.GetComponent<ILComponentBehaviour>()
                .Script;
        }

        public static T GetILComponent<T>(this Component component) where T : ICanGetILComponentFromGameObject
        {
            if (component)
            {
                return (T) component.GetComponent<ILComponentBehaviour>()
                    .Script;
            }
            else
            {
                return default(T);
            }
        }

        public static T[] GetILComponentsInChildren<T>(this GameObject gameObject) where T : ICanGetILComponentFromGameObject, new()
        {
            return gameObject.GetComponentsInChildren<ILComponentBehaviour>()
                .Select(behaviour => (T) behaviour.Script)
                .Where(behavior => behavior != null)
                .ToArray();
        }
        
        public static T[] GetILComponentsInChildren<T>(this Component component) where T : ICanGetILComponentFromGameObject, new()
        {
            return component.GetComponentsInChildren<ILComponentBehaviour>()
                .Select(behaviour => (T) behaviour.Script)
                .Where(behavior => behavior != null)
                .ToArray();
        }
    }
    
}