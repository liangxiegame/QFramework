using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace UnityEditorUI
{
    /// <summary>
    /// Boolean check box widget.
    /// </summary>
    public interface ICheckbox : IWidget
    {
        /// <summary>
        /// Whether or not the box is checked.
        /// </summary>
        IPropertyBinding<bool, ICheckbox> Checked { get; }

        /// <summary>
        /// Text to display to the left of the check box.
        /// </summary>
        IPropertyBinding<string, ICheckbox> Label { get; }
    }

    /// <summary>
    /// Boolean check box widget.
    /// </summary>
    internal class Checkbox : AbstractWidget, ICheckbox
    {
        private bool boxChecked = false;
        private string label = String.Empty;

        private PropertyBinding<bool, ICheckbox> boxCheckedProperty;
        private PropertyBinding<string, ICheckbox> labelProperty;

        public IPropertyBinding<bool, ICheckbox> Checked { get { return boxCheckedProperty; } }
        public IPropertyBinding<string, ICheckbox> Label { get { return labelProperty; } }

        internal Checkbox(ILayout parent) : base(parent)
        {
            boxCheckedProperty = new PropertyBinding<bool, ICheckbox>(
                this,
                value => this.boxChecked = value
            );

            labelProperty = new PropertyBinding<string, ICheckbox>(
                this,
                value => this.label = value
            );
        }

        public override void OnGUI()
        {
            if (boxChecked != (boxChecked = EditorGUILayout.Toggle(label, boxChecked)))
            {
                boxCheckedProperty.UpdateView(boxChecked);
            }
        }

        public override void BindViewModel(object viewModel)
        {
            boxCheckedProperty.BindViewModel(viewModel);
            labelProperty.BindViewModel(viewModel);
        }
    }
}
