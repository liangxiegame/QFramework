/****************************************************************************
 * Copyright (c) 2015 - 2023 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;

namespace QFramework
{
    public class StringEventSystem
    {
        public static readonly StringEventSystem Global = new StringEventSystem();
        
        private Dictionary<string, IEasyEvent> mEvents = new Dictionary<string, IEasyEvent>();
        
        public  IUnRegister Register(string key, Action onEvent)
        {
            if (mEvents.TryGetValue(key, out var e))
            {
                var easyEvent = e.As<EasyEvent>();
                return easyEvent.Register(onEvent);
            }
            else
            {
                var easyEvent = new EasyEvent();
                mEvents.Add(key,easyEvent);
                return easyEvent.Register(onEvent);
            }
        }

        public void UnRegister(string key, Action onEvent)
        {
            
            if (mEvents.TryGetValue(key, out var e))
            {
                var easyEvent = e.As<EasyEvent>();
                easyEvent?.UnRegister(onEvent);
            }
        }

        public void Send(string key)
        {
            if (mEvents.TryGetValue(key, out var e))
            {
                var easyEvent = e.As<EasyEvent>();
                easyEvent?.Trigger();
            }
        }
        
        
        public IUnRegister Register<T>(string key, Action<T> onEvent)
        {
            if (mEvents.TryGetValue(key, out var e))
            {
                var easyEvent = e.As<EasyEvent<T>>();
                return easyEvent.Register(onEvent);
            }
            else
            {
                var easyEvent = new EasyEvent<T>();
                mEvents.Add(key,easyEvent);
                return easyEvent.Register(onEvent);
            }
        }
        

        public void UnRegister<T>(string key, Action<T> onEvent)
        {
            
            if (mEvents.TryGetValue(key, out var e))
            {
                var easyEvent = e.As<EasyEvent<T>>();
                easyEvent?.UnRegister(onEvent);
            }
        }

        public void Send<T>(string key, T data)
        {
            if (mEvents.TryGetValue(key, out var e))
            {
                var easyEvent = e.As<EasyEvent<T>>();
                easyEvent?.Trigger(data);
            }
        }
    }
    
    [Obsolete("请使用 StringEventSystem,please use StringEventSystem instead",true)]
    public class MsgDispatcher : StringEventSystem
    {
    }

    [Obsolete("请使用 StringEventSystem,please use StringEventSystem instead",true)]
    public interface IMsgReceiver
    {
        
    }

    [Obsolete("请使用 StringEventSystem,please use StringEventSystem instead",true)]
    public interface IMsgSender
    {
        
    }

    public static class MsgDispatcherExtensions
    {
        [Obsolete("请使用 StringEventSystem.Global.Register,please use StringEventSystem.Global.Register instead",true)]
        public static void RegisterLogicMsg(this IMsgReceiver self, string msgName, Action<object[]> callback)
        {
            StringEventSystem.Global.Register(msgName, callback);
        }

        [Obsolete("请使用 StringEventSystem.Global.Register,please use StringEventSystem.Global.Register instead", true)]
        public static void SendLogicMsg(this IMsgSender sender, string msgName, params object[] paramList)
        {
            StringEventSystem.Global.Send(msgName, paramList);
        }
    }
}