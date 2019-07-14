using System;

namespace EGO.Framework
{
    public static class WindowExtension
    {
        public static T PushCommand<T>(this T view,Action command) where T : IView
        {
            Window.MainWindow.PushCommand(command);
            return view;
        }
    }
}