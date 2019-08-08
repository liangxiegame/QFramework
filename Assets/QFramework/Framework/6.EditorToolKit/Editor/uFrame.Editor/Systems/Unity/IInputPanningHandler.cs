using JetBrains.Annotations;
using UnityEngine;

namespace QF.GraphDesigner.Unity
{
    public interface IInputPanningHandler
    {
        void HandlePanning(Vector2 delta);
    }
    //public class InputSystem : DiagramPlugin, IDesignerWindowLostFocus, IUpdate, IDrawDesignerWindow
    //{


    //    public void DesignerWindowLostFocus()
    //    {
       
    //    }

    //    public void Update()
    //    {
           
    //    }

    //    public DesignerWindow DesignerWindow
    //    {
    //        get
    //        {
                
    //            return InvertGraphEditor.Container.Resolve<DesignerWindow>();
    //        }
    //    }


    //    public void DrawDesigner()
    //    {
        
    //    }
    //}


}