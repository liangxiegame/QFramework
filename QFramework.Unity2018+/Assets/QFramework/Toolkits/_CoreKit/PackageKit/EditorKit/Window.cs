#if UNITY_EDITOR
using System;
using UnityEditor;

namespace QFramework
{
    public abstract class Window : EditorWindow, IDisposable
    {
        public IMGUIViewController ViewController { get; set; }


        private void OnGUI()
        {
            if (ViewController != null)
            {
                ViewController.View.DrawGUI();
            }

            RenderEndCommandExecutor.ExecuteCommand();
        }

        public void Dispose()
        {
            OnDispose();
        }


        protected abstract void OnDispose();
    }
}
#endif