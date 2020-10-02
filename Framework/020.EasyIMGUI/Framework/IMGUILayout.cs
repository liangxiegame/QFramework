using System;

namespace QFramework
{
    public interface IMGUILayout : IMGUIView
    {
        IMGUILayout AddChild(IMGUIView view);

        void RemoveChild(IMGUIView view);

        void Clear();
    }
    
    [Obsolete("depreacted please use imguilayout")]
    public interface ILayout : IMGUILayout
    {

    }
}