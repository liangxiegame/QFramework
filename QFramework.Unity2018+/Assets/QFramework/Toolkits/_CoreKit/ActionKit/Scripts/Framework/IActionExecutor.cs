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
        void Execute(IAction action,Action<IAction> onFinish = null);
    }
    

    public static class IActionExecutorExtensions
    {
        public static bool UpdateAction(this IActionExecutor self,IAction action,float dt,Action<IAction> onFinish = null)
        {
            if (action.Execute(dt))
            {
                onFinish?.Invoke(action);
                action.Deinit();
            }

            if (action.Deinited)
            {
                return true;
            }

            return false;
        }
    }
}