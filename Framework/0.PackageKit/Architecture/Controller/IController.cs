using System;
using UnityEngine;

namespace QFramework
{
    public interface IController : ICanGetModel, ICanGetSystem, ICanSendCommand, ICanSendEvent
    {
    }

    public class ControllerNode<TConfig> : AbstractPool<ControllerNode<TConfig>>, IController
        where TConfig : Architecture<TConfig>
    {
        [Obsolete("请使用 ControllerNode.Allocate() 来获取对象", true)]
        public ControllerNode()
        {
        }

        public T GetModel<T>() where T : class, IModel
        {
            return SingletonProperty<TConfig>.Instance.GetModel<T>();
        }

        public void SendCommand<T>() where T : ICommand, new()
        {
            SingletonProperty<TConfig>.Instance.SendCommand<T>();
        }

        public void SendCommand(ICommand command)
        {
            SingletonProperty<TConfig>.Instance.SendCommand(command);
        }

        protected override void OnRecycle()
        {
        }

        public void SendEvent<T>() where T : new()
        {
            SingletonProperty<TConfig>.Instance.SendEvent<T>();
        }

        public void SendEvent<T>(T t)
        {
            SingletonProperty<TConfig>.Instance.SendEvent<T>(t);
        }
    }
    
    public class ViewController<TConfig> : MonoBehaviour, IController
        where TConfig : Architecture<TConfig>
    {

        public T GetModel<T>() where T : class, IModel
        {
            return SingletonProperty<TConfig>.Instance.GetModel<T>();
        }

        public void SendCommand<T>() where T : ICommand, new()
        {
            SingletonProperty<TConfig>.Instance.SendCommand<T>();
        }

        public void SendCommand(ICommand command)
        {
            SingletonProperty<TConfig>.Instance.SendCommand(command);
        }

        public void SendEvent<T>() where T : new()
        {
            SingletonProperty<TConfig>.Instance.SendEvent<T>();
        }

        public void SendEvent<T>(T t)
        {
            SingletonProperty<TConfig>.Instance.SendEvent<T>(t);
        }
    }
}