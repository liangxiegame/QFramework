using System;
using QF;
using UnityEngine;

namespace UnityEditorUI
{
    /// <summary>
    /// Clickable push button widget.
    /// </summary>
    public interface ILayoutButton : IWidget
    {
        /// <summary>
        /// Text to be displayed on the button.
        /// </summary>
        IPropertyBinding<string, ILayoutButton> Text { get; }

        /// <summary>
        /// Tooltip displayed on mouse hover.
        /// </summary>
        IPropertyBinding<string, ILayoutButton> Tooltip { get; }

        /// <summary>
        /// Width of the widget in pixels. Default uses auto-layout.
        /// </summary>
        IPropertyBinding<int, ILayoutButton> Width { get; }

        /// <summary>
        /// Height of the widget in pixels. Default uses auto-layout.
        /// </summary>
        IPropertyBinding<int, ILayoutButton> Height { get; }

        
        /// <summary>
        /// Height of the widget in pixels. Default uses auto-layout.
        /// </summary>
        IPropertyBinding<Texture2D, ILayoutButton> Texture { get; }
        
        /// <summary>
        /// Event invoked when the button is clicked.
        /// </summary>
        IEventBinding<ILayoutButton> Click { get; }
    }

    /// <inheritdoc />
    /// <summary>
    /// Clickable push button widget.
    /// </summary>
    internal class LayoutButton : AbstractWidget, ILayoutButton
    {
        // Private members
        //private string text = String.Empty;
        //private string tooltip = String.Empty;
        //private int width = -1;
        //private int height = -1;
        private Texture2D mTexture2D;

        // Concrete property bindings
        private PropertyBinding<string, ILayoutButton> textProperty;
        private PropertyBinding<string, ILayoutButton> tooltipProperty;
        private PropertyBinding<int, ILayoutButton> widthProperty;
        private PropertyBinding<int, ILayoutButton> heightProperty;
        private PropertyBinding<Texture2D, ILayoutButton> mTexture2DProperty;
        private EventBinding<ILayoutButton> mClickEvent;

        // Public interfaces for getting PropertyBindings
        public IPropertyBinding<string, ILayoutButton> Text { get { return textProperty; } }
        public IPropertyBinding<string, ILayoutButton> Tooltip { get { return tooltipProperty; } }
        public IPropertyBinding<int, ILayoutButton> Width { get { return widthProperty; } }
        public IPropertyBinding<int, ILayoutButton> Height { get { return heightProperty; } }

        public IPropertyBinding<Texture2D, ILayoutButton> Texture
        {
            get { return mTexture2DProperty; }
        }

        public IEventBinding<ILayoutButton> Click { get { return mClickEvent; } }

        internal LayoutButton(ILayout parent) : base(parent)
        {
            //textProperty = new PropertyBinding<string, ILayoutButton>(
            //    this,
            //    value => text = value
            //);

            //tooltipProperty = new PropertyBinding<string, ILayoutButton>(
            //    this,
            //    value => tooltip = value
            //);

            //widthProperty = new PropertyBinding<int, ILayoutButton>(
            //    this,
            //    value => width = value
            //);

            //heightProperty = new PropertyBinding<int, ILayoutButton>(
            //    this,
            //    value => height = value
            //);

            mTexture2DProperty = new PropertyBinding<Texture2D, ILayoutButton>(
                this,
                value => mTexture2D = value
            );

            mClickEvent = new EventBinding<ILayoutButton>(this);
        }

        public override void OnGUI()
        {
            if (GUILayout.Button(mTexture2D,GUILayout.Width(mTexture2D.width),GUILayout.Height(mTexture2D.height)))
            {
                QF.Log.I("click");
                mClickEvent.Invoke();
            }
        }

        public override void BindViewModel(object viewModel)
        {
            textProperty.BindViewModel(viewModel);
            tooltipProperty.BindViewModel(viewModel);
            widthProperty.BindViewModel(viewModel);
            heightProperty.BindViewModel(viewModel);
            mClickEvent.BindViewModel(viewModel);
        }
    }
}
