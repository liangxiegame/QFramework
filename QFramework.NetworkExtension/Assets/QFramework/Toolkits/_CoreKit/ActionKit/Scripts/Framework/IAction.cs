/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using UnityEngine;

namespace QFramework
{
    public enum ActionStatus
    {
        NotStart,
        Started,
        Finished,
    }

    public interface IActionController
    {
        bool Paused { get; set; }
        void Reset();
        void Deinit();
    }

    public interface IAction<TStatus> : IActionController
    {
        TStatus Status { get; set; }
        void OnStart();
        void OnExecute(float dt);
        void OnFinish();
        
        bool Deinited { get; set; }


    }


    public interface IAction : IAction<ActionStatus>
    {
    }
    


    public static class IActionExtensions
    {
        public static IActionController Start(this IAction self, MonoBehaviour monoBehaviour,
            Action<IAction> onFinish = null)
        {
            return monoBehaviour.ExecuteByUpdate(self, onFinish);
        }

        public static IActionController Start(this IAction self, MonoBehaviour monoBehaviour,
            Action onFinish)
        {
            return monoBehaviour.ExecuteByUpdate(self, _ => onFinish());
        }

        public static IActionController StartGlobal(this IAction self, Action<IAction> onFinish = null)
        {
            IActionExecutor executor = null;
            if (executor.UpdateAction(self, 0, onFinish)) return self;

            void Update()
            {
                if (executor.UpdateAction(self, Time.deltaTime, onFinish))
                {
                    ActionKit.OnUpdate.UnRegister(Update);
                }
            }

            ActionKit.OnUpdate.Register(Update);


            return self;
        }


        public static void Pause(this IActionController self)
        {
            self.As<IAction>().Paused = true;
        }

        public static void Resume(this IActionController self)
        {
            self.As<IAction>().Paused = false;
        }

        public static void Finish(this IAction self)
        {
            self.Status = ActionStatus.Finished;
        }

        public static bool Execute(this IAction self, float dt)
        {
            if (self.Status == ActionStatus.NotStart)
            {
                self.OnStart();

                if (self.Status == ActionStatus.Finished)
                {
                    self.OnFinish();
                    return true;
                }

                self.Status = ActionStatus.Started;
            }
            else if (self.Status == ActionStatus.Started)
            {
                self.OnExecute(dt);

                if (self.Status == ActionStatus.Finished)
                {
                    self.OnFinish();
                    return true;
                }
            }
            else if (self.Status == ActionStatus.Finished)
            {
                self.OnFinish();
                return true;
            }

            return false;
        }
    }
}