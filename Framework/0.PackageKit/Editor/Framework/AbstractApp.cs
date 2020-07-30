using UnityEngine;

namespace QFramework.PackageKit
{
    public abstract class AbstractApp<TCommand> where TCommand: ICommand
    {
        static TypeEventSystem mTypeEventSystem = new TypeEventSystem();
        
        public static void Send<T>(T command) where T : TCommand
        {
            mTypeEventSystem.SendEvent<TCommand>(command);
        }

        public static void Send<T>() where T : TCommand,new()
        {
            mTypeEventSystem.SendEvent<TCommand>(new T());
        }
        
        public IQFrameworkContainer Container = new QFrameworkContainer();

        public AbstractApp()
        {
            mTypeEventSystem.Clear();
            
            // 注册好 自己的实例
            Container.RegisterInstance(Container);

            // 配置命令的执行
            mTypeEventSystem.RegisterEvent<TCommand>(OnCommandExecute);
            
            ConfigureService(Container);
        }

        protected abstract void ConfigureService(IQFrameworkContainer container);

        void OnCommandExecute(TCommand command)
        {
            Container.Inject(command);
            command.Execute();
        }

        public void Dispose()
        {
            OnDispose();
            mTypeEventSystem.UnRegisterEvent<TCommand>(OnCommandExecute);
            mTypeEventSystem.Clear();
            Container.Clear();
            Container = null;
        }

        protected virtual void OnDispose() {}
    }
}