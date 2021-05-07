// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace QFramework
// {
//     public abstract class UnityArchitecture<TConfig> : MonoBehaviour,IArchitecture
//         where TConfig : UnityArchitecture<TConfig>
//     {
//         public static IArchitecture Interface
//         {
//             get { return MonoSingletonProperty<TConfig>.Instance; }
//         }
//
//         T IArchitecture.GetSystem<T>()
//         {
//             return mContainer.Resolve<T>();
//         }
//
//         public void RegisterSystem<T>(T system) where T : class, ISystem
//         {
//             mContainer.RegisterInstance(system);
//             mSystemsCache.Add(system);
//             system.SetArchitecture(this);
//         }
//
//         public void RegisterModel<T>(T model) where T :class ,IModel
//         {
//             mContainer.RegisterInstance(model);
//             mModelsCache.Add(model);
//             model.SetArchitecture(this);
//         }
//
//         T IArchitecture.GetUtility<T>()
//         {
//             return mContainer.Resolve<T>();
//         }
//
//         public void RegisterUtility<T>(T utility) where T : class,IUtility
//         {
//             mContainer.RegisterInstance<T>(utility);
//         }
//
//         private readonly IQFrameworkContainer mContainer = new QFrameworkContainer();
//
//         protected readonly ITypeEventSystem mEventSystem = new TypeEventSystem();
//
//         T IArchitecture.GetModel<T>()
//         {
//             return mContainer.Resolve<T>();
//         }
//
//         void IArchitecture.SendEvent<T>()
//         {
//             mEventSystem.SendEvent<T>();
//         }
//
//         void IArchitecture.SendEvent<T>(T @event)
//         {
//             mEventSystem.SendEvent<T>(@event);
//         }
//
//
//         private void Awake()
//         {
//             // 注册命令模式
//             RegisterCommand();
//             OnUtilityConfig();
//             OnModelConfig();
//
//             foreach (var model in mModelsCache)
//             {
//                 model.Init();
//             }
//             
//             OnSystemConfig();
//             
//             foreach (var system in mSystemsCache)
//             {
//                 system.Init();
//             }
//             
//             OnLaunch();
//         }
//
//         protected virtual void RegisterCommand()
//         {
//             mEventSystem.RegisterEvent<ICommand>(OnCommandExecute);
//         }
//
//         protected virtual void OnCommandExecute(ICommand command)
//         {
//             command.Execute();
//         }
//
//         protected virtual void UnRegisterCommand()
//         {
//             mEventSystem.RegisterEvent<ICommand>(OnCommandExecute);
//         }
//
//         void IArchitecture.SendCommand<T>()
//         {
//             mEventSystem.SendEvent<ICommand>(new T());
//         }
//
//         void IArchitecture.SendCommand(ICommand command)
//         {
//             mEventSystem.SendEvent<ICommand>(command);
//         }
//
//         private List<ISystem> mSystemsCache = new List<ISystem>();
//         private List<IModel> mModelsCache = new List<IModel>();
//         
//         protected abstract void OnSystemConfig();
//         
//         protected abstract void OnModelConfig();
//         protected abstract void OnUtilityConfig();
//
//         protected abstract void OnLaunch();
//
//         public void Dispose()
//         {
//             OnDispose();
//             if (mContainer != null) mContainer.Dispose();
//             if (mEventSystem != null) mEventSystem.Dispose();
//         }
//
//         protected virtual void OnDispose()
//         {
//         }
//
//         IDisposable IArchitecture.RegisterEvent<T>(Action<T> onEvent)
//         {
//             return mEventSystem.RegisterEvent<T>(onEvent);
//         }
//
//         public void OnSingletonInit()
//         {
//             
//         }
//     }
// }