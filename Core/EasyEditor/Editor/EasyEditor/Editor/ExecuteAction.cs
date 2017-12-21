namespace UnityEditorUI
{
    public interface IExecutor : IWidget
    {
        /// <summary>
        /// Event invoked when the button is clicked.
        /// </summary>
        IEventBinding<IExecutor> Event { get; }
    }
    
    public class Executor : AbstractWidget, IExecutor
    {
        private readonly EventBinding<IExecutor> mEvent;
        
        public Executor(ILayout parent) : base(parent)
        {
            mEvent = new EventBinding<IExecutor>(this);
        }

        public override void OnGUI()
        {
            mEvent.Invoke();
        }

        public override void BindViewModel(object viewModel)
        {
            mEvent.BindViewModel(viewModel);
        }

        public IEventBinding<IExecutor> Event { get { return mEvent; } }
    }
}