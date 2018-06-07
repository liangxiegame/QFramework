using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace UnityEditorUI
{
    /// <summary>
    /// Widget for entering a date and time.
    /// </summary>
    public interface IDateTimePicker : IWidget
    {
        /// <summary>
        /// Date and time currently being displayed in the widget.
        /// </summary>
        IPropertyBinding<DateTime, IDateTimePicker> Date { get; }

        /// <summary>
        /// Widget width in pixels. Default uses auto-layout.
        /// </summary>
        IPropertyBinding<int, IDateTimePicker> Width { get; }

        /// <summary>
        /// Widget height in pixels. Default uses auto-layout.
        /// </summary>
        IPropertyBinding<int, IDateTimePicker> Height { get; }
    }

    /// <summary>
    /// Widget for entering a date and time.
    /// </summary>
    internal class DateTimePicker : AbstractWidget, IDateTimePicker
    {
        private DateTime date;
        private string text;
        private bool textValid = true;
        private int width = -1;
        private int height = -1;
        private CultureInfo culture;

        private PropertyBinding<DateTime, IDateTimePicker> dateProperty;
        private PropertyBinding<int, IDateTimePicker> widthProperty;
        private PropertyBinding<int, IDateTimePicker> heightProperty;

        public IPropertyBinding<DateTime, IDateTimePicker> Date { get { return dateProperty; } }
        public IPropertyBinding<int, IDateTimePicker> Width { get { return widthProperty; } }
        public IPropertyBinding<int, IDateTimePicker> Height { get { return heightProperty; } }

        internal DateTimePicker(ILayout parent) : base(parent)
        {
            culture = CultureInfo.CreateSpecificCulture("en-AU");

            dateProperty = new PropertyBinding<DateTime, IDateTimePicker>(
                this,
                value =>
                {
                    this.date = value;
                    this.text = date.ToString(culture);
                }
            );

            widthProperty = new PropertyBinding<int, IDateTimePicker>(
                this,
                value => this.width = value
            );

            heightProperty = new PropertyBinding<int, IDateTimePicker>(
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

            // Make the background of the widget red if the date is invalid.
            var savedColour = UnityEngine.GUI.backgroundColor;
            if (!textValid)
            {
                UnityEngine.GUI.backgroundColor = Color.red;
            }
            string newText = GUILayout.TextField(text, layoutOptions.ToArray());
            UnityEngine.GUI.backgroundColor = savedColour;

            // Update the date
            if (newText != text)
            {
                text = newText;

                textValid = DateTime.TryParse(text, culture, DateTimeStyles.None, out date);
                if (textValid)
                {
                    dateProperty.UpdateView(date);
                }
            }
        }

        public override void BindViewModel(object viewModel)
        {
            dateProperty.BindViewModel(viewModel);
            widthProperty.BindViewModel(viewModel);
            heightProperty.BindViewModel(viewModel);
        }
    }
}
