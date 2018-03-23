namespace UnityEditorUI
{
    /// <summary>
    /// Abstract class that all other widgets must implement.
    /// </summary>
    public abstract class AbstractWidget : IWidget
    {
        /// <summary>
        /// Needed in order to get back to the parent via the End() method.
        /// </summary>
        private readonly ILayout mParent;

        /// <summary>
        /// Creates the widget and sets its parent.
        /// </summary>
        protected AbstractWidget(ILayout parent)
        {
            mParent = parent;
        }

        /// <summary>
        /// Updates this widget and all children (if it is a layout)
        /// </summary>
        public abstract void OnGUI();

        /// <summary>
        /// Binds the properties and events in this widget to corrosponding ones in the supplied view model.
        /// </summary>
        public abstract void BindViewModel(object viewModel);

        /// <summary>
        /// Fluent API for getting the layout containing this widget.
        /// </summary>
        public ILayout End()
        {
            return mParent;
        }
    }
}
