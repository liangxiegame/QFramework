/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;

namespace QFramework
{
    public interface IActionExecutor
    {
        void Execute(IAction action,Action<IActionController> onFinish = null);
    }
    

    public static class IActionExecutorExtensions
    {
        public static bool UpdateAction(this IActionExecutor self,IAction action,float dt,Action<IActionController> onFinish = null)
        {
            if (!action.Deinited && action.Execute(dt))
            {
                onFinish?.Invoke(new ActionController()
                {
                    Action = action,
                    ActionID = action.ActionID
                });
                
                action.Deinit();
            }

            return action.Deinited;
        }
    }
}