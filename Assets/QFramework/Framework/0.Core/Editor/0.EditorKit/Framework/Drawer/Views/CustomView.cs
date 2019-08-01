using System;

namespace EGO.Framework
{
    public class CustomView : View
    {
        public CustomView(Action onGuiAction)
        {
            OnGUIAction = onGuiAction;
        }

        public Action OnGUIAction { get; set; }
        
        protected override void OnGUI()
        {
            OnGUIAction.Invoke();
        }
    }
}