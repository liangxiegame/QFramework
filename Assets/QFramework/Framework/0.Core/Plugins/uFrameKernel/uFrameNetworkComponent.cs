using System.Collections;
using UniRx;
using UnityEngine.Networking;

namespace uFrame.Kernel
{
    public class uFrameNetworkComponent : NetworkBehaviour, IDisposableContainer
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

        protected virtual IEnumerator Start()
        {
            KernelLoading();
            while (!uFrameKernel.IsKernelLoaded) yield return null;
            KernelLoaded();
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