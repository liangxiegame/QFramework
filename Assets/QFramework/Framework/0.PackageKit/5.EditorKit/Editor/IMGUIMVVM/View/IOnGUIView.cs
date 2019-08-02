
using System.Collections.Generic;

namespace QF
{
    public interface IOnGUIView
    {
        bool Visible { get; set; }
       
        void AddChild(IOnGUIView childView);
        void RemoveChild(IOnGUIView childView);
        
        List<IOnGUIView> Children { get; }
        
        void OnGUI();

        IOnGUIView End();
    }
}