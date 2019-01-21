using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityEditorUI
{
    /// <summary>
    /// Widget for entering text.
    /// </summary>
    public interface ITextBox : IWidget
    {
        /// <summary>
        /// Editable text.
        /// </summary>
        IPropertyBinding<string, ITextBox> Text { get; }

        /// <summary>
        /// Width of the widget in pixels. Default uses auto-layout.
        /// </summary>
        IPropertyBinding<int, ITextBox> Width { get; }
        
        /// <summary>
        /// Height of the widget in pixels. Default uses auto-layout.
        /// </summary>
        IPropertyBinding<int, ITextBox> Height { get; }
    }

    /// <summary>
    /// Widget for entering text.
    /// </summary>
    internal class TextBox : AbstractWidget, ITextBox
    {
        private string text = string.Empty;
        private int width = -1;
        private int height = -1;

        private PropertyBinding<string, ITextBox> textProperty;
        private PropertyBinding<int, ITextBox> widthProperty;
        private PropertyBinding<int, ITextBox> heightProperty;

        public IPropertyBinding<string, ITextBox> Text { get { return textProperty; } }
        public IPropertyBinding<int, ITextBox> Width { get { return widthProperty; } }
        public IPropertyBinding<int, ITextBox> Height { get { return heightProperty; } }

        internal TextBox(ILayout parent) : base(parent)
        {
            textProperty = new PropertyBinding<string, ITextBox>(
                this,
                value => text = value == null ? String.Empty : value
            );

            widthProperty = new PropertyBinding<int, ITextBox>(
                this,
                value => this.width = value
            );

            heightProperty = new PropertyBinding<int, ITextBox>(
                this,
                value => this.height = value
            );
        }

        public override void OnGUI()
        {
            var layoutOptions = new List<GUILayoutOption>();
            if (width >= 0)
            {
                layoutOptions.Add(GUILayout.Width(width));
            }

            if (height >= 0)
            {
                layoutOptions.Add(GUILayout.Height(height));
            }
            
            string newText = height >= 0 // Use TextField if height isn't specified, otherwise use TextArea
                ? GUILayout.TextArea(text, layoutOptions.ToArray())
                : GUILayout.TextField(text, layoutOptions.ToArray());
            if (newText != text)
            {
                text = newText;
                textProperty.UpdateView(newText);
            }
        }

        public override void BindViewModel(object viewModel)
        {
            textProperty.BindViewModel(viewModel);
            widthProperty.BindViewModel(viewModel);
            heightProperty.BindViewModel(viewModel);
        }
    }
}
