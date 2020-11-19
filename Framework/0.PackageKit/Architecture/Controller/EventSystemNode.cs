using System;

namespace QFramework
{
    public class EventSystemNode<TConfig> : AbstractPool<EventSystemNode<TConfig>> where TConfig : Architecture<TConfig>,ICanSendEvent
    {
        [Obsolete("请通过 EventSystemNode.Allocate() 来获取对象",true)]
        public EventSystemNode() {}
        
        DisposableList mDisposableList = new DisposableList();

        public void Register<TEvent>(Action<TEvent> onEvent)
        {
            var icanDispose = SingletonProperty<TConfig>.Instance.RegisterEvent<TEvent>(onEvent);
            mDisposableList.Add(icanDispose);
        }

        void UnRegisterAll()
        {
            mDisposableList.Dispose(); 
        }

        protected override void OnRecycle()
        {
            UnRegisterAll();
        }
    }
}