using System;
using Invert.Windows;
using UnityEngine;

namespace Invert.Windows
{
    public interface IWindowsEvents
    {
        void ShowWindow(string factoryId, string title,IWindow windowViewModel, Vector2 position, Vector2 size);
        void ShowWindowNormal(string factoryId, string title, Vector2 position, Vector2 size);
        void WindowRequestCloseWithArea(Area drawer);
        void WindowRefresh(Area drawer);
        void WindowRequestCloseWithViewModel(IWindow windowViewModel);

        void ShowWindowPopup<T>(string title, Action<T> configure, Vector2 position, Vector2 size);
    }
}