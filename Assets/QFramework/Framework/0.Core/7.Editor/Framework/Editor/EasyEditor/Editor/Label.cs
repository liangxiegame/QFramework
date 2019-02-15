using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnityEditorUI
{
    /// <summary>
    /// Widget for displaying read-only text.
    /// </summary>
    public interface ILabel : IWidget
    {
        /// <summary>
        /// Label text.
        /// </summary>
        IPropertyBinding<string, ILabel> Text { get; }

        /// <summary>
        /// Text displayed on mouse hover.
        /// </summary>
        IPropertyBinding<string, ILabel> Tooltip { get; }

        /// <summary>
        /// Whether or not the label should be displayed in bold (default is false).
        /// </summary>
        //IPropertyBinding<bool, ILabel> Bold { get; }

        /// <summary>
        /// Width of the widget in pixels. Default uses auto-layout.
        /// </summary>
        //IPropertyBinding<int, ILabel> Width { get; }

        /// <summary>
        /// Height of the widget in pixels. Default uses auto-layout.
        /// </summary>
        //IPropertyBinding<int, ILabel> Height { get; }
    }

    /// <summary>
    /// Widget for displaying read-only text.
    /// </summary>
    internal class Label : AbstractWidget, ILabel
    {
        private string text = String.Empty;
        private string tooltip = String.Empty;
        //private bool bold = false;
        //private int width = -1;
        //private int height = -1;

        private PropertyBinding<string, ILabel> textProperty;
        private PropertyBinding<string, ILabel> tooltipProperty;
        //private PropertyBinding<bool, ILabel> boldProperty;
        //private PropertyBinding<int, ILabel> widthProperty;
        //private PropertyBinding<int, ILabel> heightProperty;

        public IPropertyBinding<string, ILabel> Text { get { return textProperty; } }
        public IPropertyBinding<string, ILabel> Tooltip { get { return tooltipProperty; } }
        //public IPropertyBinding<bool, ILabel> Bold { get { return boldProperty; } }
        //public IPropertyBinding<int, ILabel> Width { get { return widthProperty; } }
        //public IPropertyBinding<int, ILabel> Height { get { return heightProperty; } }

        internal Label(ILayout parent) : base(parent)
        {
            textProperty = new PropertyBinding<string, ILabel>(
                this,
                value => text = value
            );

            tooltipProperty = new PropertyBinding<string, ILabel>(
                this,
                value => tooltip = value
            );
            
            //boldProperty = new PropertyBinding<bool, ILabel>(
            //    this,
            //    value => bold = value
            //);

            //widthProperty = new PropertyBinding<int, ILabel>(
            //    this,
            //    value => width = value
            //);
            
            //heightProperty = new PropertyBinding<int, ILabel>(
            //    this,
            //    value => height = value
            //);
        }

        public override void OnGUI()
        {
            var guiContent = new GUIContent(text, tooltip);
            /*用这些有问题
            var style = bold ? EditorStyles.boldLabel : EditorStyles.label;
            var layoutOptions = new List<GUILayoutOption>();
            if (width >= 0)
            {
                layoutOptions.Add(GUILayout.Width(width));
            }
            if (height >= 0)
            {
                layoutOptions.Add(GUILayout.Height(height));
            }

            GUILayout.Label(guiContent, style, layoutOptions.ToArray());
            */

            // 默认用这个其他的有一堆问题
            GUILayout.Label(guiContent);
        }

        public override void BindViewModel(object viewModel)
        {
            textProperty.BindViewModel(viewModel);
            tooltipProperty.BindViewModel(viewModel);
            //boldProperty.BindViewModel(viewModel);
            //widthProperty.BindViewModel(viewModel);
            //heightProperty.BindViewModel(viewModel);
        }
    }
}
