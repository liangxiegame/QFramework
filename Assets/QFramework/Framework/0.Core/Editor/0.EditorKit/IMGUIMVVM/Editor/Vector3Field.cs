using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnityEditorUI
{
    /// <summary>
    /// Widget for entering vectors with X, Y and Z coordinates.
    /// </summary>
    public interface IVector3Field : IWidget
    {
        /// <summary>
        /// Label shown to the left of the widget.
        /// </summary>
        IPropertyBinding<string, IVector3Field> Label { get; }

        /// <summary>
        /// Text shown on mouse hover
        /// </summary>
        IPropertyBinding<string, IVector3Field> Tooltip { get; }

        /// <summary>
        /// Vector entered in widget.
        /// </summary>
        IPropertyBinding<Vector3, IVector3Field> Vector { get; }
    }

    /// <summary>
    /// Widget for entering vectors with X, Y and Z coordinates.
    /// </summary>
    internal class Vector3Field : AbstractWidget, IVector3Field
    {
        private Vector3 vector;
        private string label;
        private string tooltip;

        private PropertyBinding<Vector3, IVector3Field> vectorProperty;
        private PropertyBinding<string, IVector3Field> labelProperty;
        private PropertyBinding<string, IVector3Field> tooltipProperty;

        public IPropertyBinding<Vector3, IVector3Field> Vector { get { return vectorProperty; } }
        public IPropertyBinding<string, IVector3Field> Label { get { return labelProperty; } }
        public IPropertyBinding<string, IVector3Field> Tooltip { get { return tooltipProperty; } }

        internal Vector3Field(ILayout parent) : base(parent)
        {
            vectorProperty = new PropertyBinding<Vector3, IVector3Field>(
                this, 
                value => this.vector = value
            );

            labelProperty = new PropertyBinding<string, IVector3Field>(
                this,
                value => this.label = value
            );

            tooltipProperty = new PropertyBinding<string, IVector3Field>(
                this,
                value => this.tooltip = value
            );
        }

        public override void OnGUI()
        {
            var newVector = EditorGUILayout.Vector3Field(new GUIContent(label, tooltip), vector);
            if (newVector != vector)
            {
                vector = newVector;
                vectorProperty.UpdateView(vector);
            }
        }

        public override void BindViewModel(object viewModel)
        {
            vectorProperty.BindViewModel(viewModel);
            labelProperty.BindViewModel(viewModel);
            tooltipProperty.BindViewModel(viewModel);
        }
    }
}
