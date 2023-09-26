/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2015 - 2023 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

namespace QFramework
{
    using System;
    using System.Collections.Generic;
    
    public class EnumEventSystem 
    {
        public static readonly EnumEventSystem Global = new EnumEventSystem();
        
        private readonly Dictionary<int, IEasyEvent> mEvents = new Dictionary<int, IEasyEvent>(50);
        
        protected EnumEventSystem(){}

        #region 功能函数

        public IUnRegister Register<T>(T key, Action<int,object[]> onEvent) where T : IConvertible
        {
            var kv = key.ToInt32(null);

            if (mEvents.TryGetValue(kv, out var e))
            {
                var easyEvent = e.As<EasyEvent<int,object[]>>();
                return easyEvent.Register(onEvent);
            }
            else
            {
                var easyEvent = new EasyEvent<int,object[]>();
                mEvents.Add(kv, easyEvent);
                return easyEvent.Register(onEvent);
            }
        }

        public void UnRegister<T>(T key, Action<int,object[]> onEvent) where T : IConvertible
        {
            var kv = key.ToInt32(null);

            if (mEvents.TryGetValue(kv, out var e))
            {
                e.As<EasyEvent<int,object[]>>()?.UnRegister(onEvent);
            }
        }

        public void UnRegister<T>(T key) where T : IConvertible
        {
            var kv = key.ToInt32(null);

            if (mEvents.ContainsKey(kv))
            {
                mEvents.Remove(kv);
            }
        }

        public void UnRegisterAll()
        {
            mEvents.Clear();
        }

        public void Send<T>(T key, params object[] args) where T : IConvertible
        {
            var kv = key.ToInt32(null);

            if (mEvents.TryGetValue(kv, out var e))
            {
                e.As<EasyEvent<int,object[]>>().Trigger(kv,args);
            }
        }

        #endregion
        
    }

    [Obsolete("请使用 EnumEventSystem,Please use EnumEventSystem instead", APIVersion.Force)]
    public class QEventSystem : EnumEventSystem
    {
        protected QEventSystem() : base()
        {
            
        }
        [Obsolete("请使用 Global,Please use Global instead", APIVersion.Force)]
        public static EnumEventSystem Instance => Global;
        
        
        [Obsolete("请使用 Global.Send,Please use Global.Send instead", APIVersion.Force)]
        public static void SendEvent<T>(T key, params object[] param) where T : IConvertible
        {
            Global.Send(key, param);
        }

        [Obsolete("请使用 Global.Register,Please use Global.Register instead", APIVersion.Force)]
        public static void RegisterEvent<T>(T key, Action<int,object[]> fun) where T : IConvertible
        {
            Global.Register(key, fun);
        }

        [Obsolete("请使用 Global.UnRegister,Please use Global.UnRegister instead", APIVersion.Force)]
        public static void UnRegisterEvent<T>(T key, Action<int,object[]> fun) where T : IConvertible
        {
            Global.UnRegister(key, fun);
        }
    }
}