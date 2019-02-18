using System;
using System.Collections;
using UniRx;
using UnityEngine;

namespace uFrame.Kernel
{
    /// <summary>
    /// The uFrameComponent is a simple class that extends from MonoBehaviour, and is directly plugged into the kernel.
    /// Use this component when creating any components manually or if you need to plug existing libraries into the uFrame system.
    /// <example>
    /// public class MyComponent : uFrameComponent {
    /// }
    /// </example></summary>
    /// <example>
    /// 	<para>public class MyComponent : uFrameComponent {</para>
    /// 	<para>      public override void KernelLoaded() {</para>
    /// 	<para>             this.Publish(new MyComponentCreatedEvent() { Instance = this });</para>
    /// 	<para>      }</para>
    /// 	<para>}</para>
    /// </example>
    public class uFrameComponent : MonoBehaviour, IDisposableContainer
    {
        private CompositeDisposable _disposer;

        CompositeDisposable IDisposableContainer.Disposer
        {
            get { return _disposer ?? (_disposer = new CompositeDisposable()); }
            set { _disposer = value; }
        }
        protected virtual void OnDestroy()
        {
            if (_disposer != null)
            {
                _disposer.Dispose();
            }
        }

        protected IEventAggregator EventAggregator
        {
            get { return uFrameKernel.EventAggregator; }
        }

        /// <summary>Wait for an Event to occur on the global event aggregator.</summary>
        /// <example>
        /// this.OnEvent&lt;MyEventClass&gt;().Subscribe(myEventClassInstance=&gt;{ DO_SOMETHING_HERE });
        /// </example>
        public IObservable<TEvent> OnEvent<TEvent>()
        {
            return EventAggregator.GetEvent<TEvent>();
        }

        /// <summary>Publishes a command to the event aggregator. Publish the class data you want, and let any "OnEvent" subscriptions handle them.</summary>
        /// <example>
        /// this.Publish(new MyEventClass() { Message = "Hello World" });
        /// </example>
        public void Publish(object eventMessage)
        {
            EventAggregator.Publish(eventMessage);
        }

        protected virtual void Start()
        {
            KernelLoading();
            if (!uFrameKernel.IsKernelLoaded)
                uFrameKernel.EventAggregator
                .GetEvent<KernelLoadedEvent>()
                .Take(1)
                .Subscribe(x => KernelLoaded());
            else KernelLoaded();
        }

        void Update()
        {
            
        }

        /// <summary>
        /// Before we wait for the kernel to load, even if the kernel is already loaded it will still invoke this before it attempts to wait.
        /// </summary>
        public virtual void KernelLoading()
        {

        }

        /// <summary>
        /// The first method to execute when we are sure the kernel has completed loading.
        /// </summary>
        public virtual void KernelLoaded()
        {
            
        }



    }
}