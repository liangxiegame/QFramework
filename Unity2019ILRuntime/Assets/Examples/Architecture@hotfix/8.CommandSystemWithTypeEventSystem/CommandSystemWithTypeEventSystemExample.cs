using UnityEngine;
using QFramework;
using QFramework.ILRuntime;
using UniRx;

namespace QFramework.Example
{
    public partial class CommandSystemWithTypeEventSystemExample
    {
        /// <summary>
        /// 命令接口
        /// </summary>
        public interface ICommand
        {
            void Execute();
        }

        public class SayHelloWorldCommand : ICommand
        {
            public void Execute()
            {
                Debug.Log("Hello World");
            }
        }
        
        public class DoSomethingCommand: ICommand
        {
            public void Execute()
            {
                Debug.Log("Do Something");
            }
        }

        public class CommandSystem
        {
            /// <summary>
            /// ILTypeEventSystem 除了可以按照全局静态类使用，也可以自己创建对象使用
            /// </summary>
            ILTypeEventSystem mEventSystem = new ILTypeEventSystem();
            public CommandSystem()
            {
                mEventSystem.RegisterEvent<ICommand>(OnCommandExecute);
            }

            void OnCommandExecute(ICommand command)
            {
                // 这里可以拦截，后续很容易实现 Command 拦截器，或者 Middleware 模式
                command.Execute();
            }

            public void Dispose()
            {
                mEventSystem.UnRegisterEvent<ICommand>(OnCommandExecute);
            }

            /// <summary>
            /// 命令系统封装
            /// </summary>
            /// <typeparam name="T"></typeparam>
            public void SendCommand<T>() where T : ICommand, new()
            {
                mEventSystem.SendEvent<ICommand>(new T());
            }

            public void SendCommand<T>(T t) where T : ICommand, new()
            {
                mEventSystem.SendEvent<ICommand>(new T());
            }
        }
        
        CommandSystem mCommandSystem = new CommandSystem();

        void OnStart()
        {
            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        mCommandSystem.SendCommand(new SayHelloWorldCommand());
                        mCommandSystem.SendCommand<DoSomethingCommand>();
                    }
                }).AddTo(gameObject);
        }

        
        void OnDestroy()
        {
            mCommandSystem.Dispose();
            mCommandSystem = null;
        }
    }
}