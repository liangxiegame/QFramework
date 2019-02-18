using System.Collections;
using UniRx;

namespace uFrame.Kernel
{
    /// <summary>
    /// The base class for all services on the kernel.  Services provide an easy communication layer with the use
    /// of the EventAggregator.  You can use this.Publish(new AnyType()).  Or you can use this.OnEvent&lt;AnyType&gt;().Subscribe(anyTypeInstance=>{ });
    /// In services you can also inject any instances that are setup in any of the SystemLoaders.
    /// </summary>
    public abstract class SystemServiceMonoBehavior : uFrameComponent, ISystemService, IDisposableContainer
    {
        private CompositeDisposable _disposer = new CompositeDisposable();

        public CompositeDisposable Disposer
        {
            get { return _disposer; }
            set { _disposer = value; }
        }

        IEventAggregator ISystemService.EventAggregator
        {
            get { return EventAggregator; }
            set
            {
                // No need to set
            }
        }

        /// <summary>
        /// This method is to setup an listeners on the EventAggregator, or other initialization requirements.
        /// </summary>
        public virtual void Setup()
        {
            if (Disposer.IsDisposed)
            {
                Disposer = new CompositeDisposable();
            }
        }

        /// <summary>
        /// This method is called by the kernel to do any setup the make take some time to complete.  It is executed as 
        /// a co-routine by the kernel.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator SetupAsync()
        {
            yield break;
        }

        public virtual void Loaded()
        {
            
        }

        protected override void OnDestroy()
        {
            Disposer.Dispose();
        }
    }
}