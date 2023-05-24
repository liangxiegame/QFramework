/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QFramework
{
    internal class MonoUpdateActionExecutor : MonoBehaviour, IActionExecutor
    {

        private Dictionary<IAction, Action<IActionController>> mActionAndFinishCallbacks = new Dictionary<IAction, Action<IActionController>>();

        public void Execute(IAction action,Action<IActionController> onFinish = null)
        {
            if (action.Status == ActionStatus.Finished) action.Reset();
            if (this.UpdateAction(action, 0, onFinish)) return;

            if (mActionAndFinishCallbacks.ContainsKey(action))
            {
                mActionAndFinishCallbacks[action] = onFinish;
            }
            else
            {
                mActionAndFinishCallbacks.Add(action, onFinish);
            }
        }

        private List<IAction> mToActionRemove = new List<IAction>();
        private void Update()
        {
            foreach (var actionAndFinishCallback in mActionAndFinishCallbacks)
            {
                if (this.UpdateAction(actionAndFinishCallback.Key, Time.deltaTime, actionAndFinishCallback.Value))
                {
                    mToActionRemove.Add(actionAndFinishCallback.Key);
                }
            }

            if (mToActionRemove.Count > 0)
            {
                foreach (var action in mToActionRemove)
                {
                    mActionAndFinishCallbacks.Remove(action);
                }
                
                mToActionRemove.Clear();
            }
        }
    }

    public static class MonoUpdateActionExecutorExtension
    {
        public static IAction ExecuteByUpdate<T>(this T self, IAction action,Action<IActionController> onFinish = null) where T : MonoBehaviour
        {
            if (action.Status == ActionStatus.Finished) action.Reset();
            self.gameObject.GetOrAddComponent<MonoUpdateActionExecutor>().Execute(action,onFinish);
            return action;
        }
    }
}