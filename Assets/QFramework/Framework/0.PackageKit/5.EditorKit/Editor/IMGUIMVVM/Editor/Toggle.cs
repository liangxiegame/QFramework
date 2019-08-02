using UnityEditorUI;
using UnityEngine;

namespace QF.Editor
{
    /// <summary>
    /// Widget for displaying read-only text.
    /// </summary>
    public interface IToggle : IWidget
    {
        /// <summary>
        /// Label text.
        /// </summary>
        IPropertyBinding<string, IToggle> Text { get; }

        /// <summary>
        /// Text displayed on mouse hover.
        /// </summary>
        IPropertyBinding<bool, IToggle> On { get; }
    }

    /// <summary>
    /// Widget for displaying read-only text.
    /// </summary>
    internal class Toggle : AbstractWidget, IToggle
    {
        private string text = string.Empty;
        private bool   on   = false;

        private PropertyBinding<string, IToggle> textProperty;
        private PropertyBinding<bool, IToggle>   onProperty;

        public IPropertyBinding<string, IToggle> Text
        {
            get { return textProperty; }
        }

        public IPropertyBinding<bool, IToggle> On
        {
            get { return onProperty; }
        }

        internal Toggle(ILayout parent) : base(parent)
        {
            textProperty = new PropertyBinding<string, IToggle>(
                this,
                value => text = value
            );

            onProperty = new PropertyBinding<bool, IToggle>(
                this,
                value => on = value
            );
        }

        public override void OnGUI()
        {
            var newOn = GUILayout.Toggle(@on, text);

            if (newOn != @on)
            {
                on = newOn;
                onProperty.UpdateView(newOn);
            }
        }

        public override void BindViewModel(object viewModel)
        {
            textProperty.BindViewModel(viewModel);
            onProperty.BindViewModel(viewModel);
        }
    }
}