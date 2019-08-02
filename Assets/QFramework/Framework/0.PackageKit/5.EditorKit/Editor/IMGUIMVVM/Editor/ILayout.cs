using QF.Editor;

namespace UnityEditorUI
{
    /// <summary>
    /// Layouts are widgets that can contain other child widgets.
    /// </summary>
    public interface ILayout : IWidget
    {
        /// <summary>
        /// Creates a new button and adds it to the layout.
        /// </summary>
        IButton Button();


        /// <summary>
        /// GUILayoutButton
        /// </summary>
        /// <returns></returns>
        ILayoutButton LayoutButton();

        /// <summary>
        /// Creates a new label and adds it to the view.
        /// </summary>
        ILabel Label(int width = -1);

        IToggle Toggle();

        IToolbar Toolbar(int defaultIndex = 0,params string[] names);
        
        /// <summary>
        /// Creates a new TextBox and adds it to the layout.
        /// </summary>
        ITextBox TextBox();

        /// <summary>
        /// Widget for choosing dates, similar do TextBox except with date validation built-in.
        /// </summary>
        IDateTimePicker DateTimePicker();

        /// <summary>
        /// Creates a new drop-down selection box and adds it to the layout.
        /// </summary>
        IDropdownBox DropdownBox();

        /// <summary>
        /// Creates a new checkbox and adds it to the layout.
        /// </summary>
        ICheckbox Checkbox();

        /// <summary>
        /// Creates a Vector3 field with X, Y and Z entry boxes.
        /// </summary>
        IVector3Field Vector3Field();

        /// <summary>
        /// Creates a widget for editing layer masks.
        /// </summary>
        ILayerPicker LayerPicker();

        /// <summary>
        /// Inserts a space between other widgets.
        /// </summary>
        ISpacer Spacer();


        IExecutor Executor();

        /// <summary>
        /// Creates a VerticalLayout and adds it to this layout.
        /// </summary>
        ILayout VerticalLayout();

        /// <summary>
        /// Creates a horizontal layout and adds it to this layout.
        /// </summary>
        ILayout HorizontalLayout(string text = null);
        
        

        /// <summary>
        /// Whether or not to draw this layout and its sub-widgets (default is true).
        /// </summary>
        IPropertyBinding<bool, ILayout> Enabled { get; }
    }
}
