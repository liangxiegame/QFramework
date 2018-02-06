using UnityEngine;
using UnityEditor;
using UnityEditorUI;
using System.ComponentModel;
using System.Runtime.CompilerServices;

/// <summary>
/// An editor window that binds to a view, subscribing to INotifyPropertyChanged
/// </summary>
class Example2 : EditorWindow
{
    /// <summary>
    /// Our GUI object.
    /// </summary>
    private UnityEditorUI.GUI gui;
    
    /// <summary>
    /// Allow the window to be opened via a menu item.
    /// </summary>
    [MenuItem("UnityEditorUI Examples/Example 2")]
    public static void ShowWindow()
    {
        var window = (Example2) EditorWindow.GetWindow<Example2>(false, "Editor window");
        
        if (window.gui == null)
        {
            window.SetUpGUI();
        }
        window.Show();
    }
    
    /// <summary>
    /// Set up the GUI.
    /// </summary>
    private void SetUpGUI()
    {
        // First, create an instance of the view we want to bind the GUI to
        var viewModel = new ExampleView();
        
        // Set up the GUI widgets
        gui = new UnityEditorUI.GUI();
        gui.Label()
                .Text.Value("Object movement tool")
                .Bold.Value(true)
            .End()
            .HorizontalLayout()
                .Label()
                    .Text.Value("Object to look for")
                .End()
                .TextBox()
                    .Text.Bind(() => viewModel.SelectedObjectName)
                .End()
            .End()
            .Vector3Field()
                .Label.Value("Position")
                .Vector.Bind(() => viewModel.ObjectPosition)
            .End()
            .Button()
                .Text("Capture position")
                .Click(() => viewModel.CaptureObjectPosition())
            .End()
            .Button()
                .Text("Set position")
                .Click(() => viewModel.SetObjectPosition())
            .End();
            
        // Bind the resulting GUI to the view.
        gui.BindViewModel(viewModel);
    }
    
    void OnGUI()
    {
        // Calling OnGUI on the root will update the whole GUI.
        gui.OnGUI();
    }
    
    /// <summary>
    /// A simple view to bind our GUI to.
    /// </summary>
    private class ExampleView : INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}


        // Properties and events to bind to go here
        public void CaptureObjectPosition()
        {
            var go = GameObject.Find(SelectedObjectName);
            if (go == null)
            {
                EditorUtility.DisplayDialog("Error", "Can't find object '" + "'", "Ok");
                return;
            }

            ObjectPosition = go.transform.position;
        }

        public void SetObjectPosition()
        {
			var go = GameObject.Find(SelectedObjectName);
			if (go == null)
			{
				EditorUtility.DisplayDialog("Error", "Can't find object '" + "'", "Ok");
				return;
			}

			go.transform.position = ObjectPosition;
        }

		private Vector3 objectPosition;

        public Vector3 ObjectPosition 
		{ 
			get
			{
				return objectPosition;
			}
			set
			{
				objectPosition = value;
				NotifyPropertyChanged("ObjectPosition");
			}
		}

		private string selectedObjectName;

        public string SelectedObjectName 
		{ 
			get
			{
				return selectedObjectName;
			}
			set
			{
				selectedObjectName = value;
				NotifyPropertyChanged("SelectedObjectName");
			}
		}
    }
}