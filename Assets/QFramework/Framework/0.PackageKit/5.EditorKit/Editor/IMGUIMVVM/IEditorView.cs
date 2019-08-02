
namespace QF
{
    public interface IEditorView
    {
        bool Visible { get; set; }
       
        void AddChild(IEditorView childView);
        void RemoveChild(IEditorView childView);
        
        void OnGUI();
    }
}