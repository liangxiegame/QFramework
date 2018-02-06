
namespace QFramework
{
    public interface IOnGUIView
    {
        bool Visible { get; set; }
       
        void AddChild(IOnGUIView childView);
        void RemoveChild(IOnGUIView childView);
        
        void OnGUI();

        IOnGUIView End();
    }
}