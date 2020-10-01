using System;

namespace QFramework
{
    public interface IController : ICanGetModel,ICanGetSystem,ICanSendCommand
    {
        
    }
    
    public class ControllerNode<TConfig> : AbstractPool<ControllerNode<TConfig>>, IController where TConfig : Architecture<TConfig>
    {
        [Obsolete("请使用 ControllerNode.Allocate() 来获取对象",true)]
        public ControllerNode(){}
        
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
    }
}