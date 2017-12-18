using System.Collections.Generic;

namespace QFramework
{
    public interface IEditorView
    {
        bool Visible { get; set; }
       
        void AddChild(IEditorView childView);
        void RemoveChild(IEditorView childView);
        
        void OnGUI();
        
        
    }
}