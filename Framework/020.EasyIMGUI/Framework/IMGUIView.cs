using System;
using UnityEngine;

namespace QFramework
{
    public interface IMGUIView : IDisposable
    {
        void Show();

        void Hide();

        void DrawGUI();

        IMGUILayout Parent { get; set; }

        GUIStyleProperty Style { get; }

        Color BackgroundColor { get; set; }

        void RefreshNextFrame();

        void AddLayoutOption(GUILayoutOption option);

        void RemoveFromParent();

        void Refresh();
    }
    
    [Obsolete("use imguiview instead")]
    public interface IView : IMGUIView{}
}