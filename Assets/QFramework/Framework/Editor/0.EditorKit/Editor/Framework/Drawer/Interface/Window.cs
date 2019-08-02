using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EGO.Framework
{
    public abstract class Window : EditorWindow, IDisposable
    {
        public static Window MainWindow { get; protected set; }
        
        public ViewController ViewController { get; set; }

        public T CreateViewController<T>() where T : Framework.ViewController ,new()
        {
            var t = new T();
            t.SetUpView();
            return t;
        }
        
        public static void Open<T>(string title) where T : Window
        {
            MainWindow = GetWindow<T>(true);

            if (!MainWindow.mShowing)
            {
                MainWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 800, 600);
                MainWindow.titleContent = new GUIContent(title);
                MainWindow.Init();
                MainWindow.mShowing = true;
                MainWindow.Show();
            }
            else
            {
                MainWindow.mShowing = false;
                MainWindow.Dispose();
                MainWindow.Close();
                MainWindow = null;
            }
        }

        public static SubWindow CreateSubWindow(string name = "SubWindow")
        {
            var window = GetWindow<SubWindow>(true, name);
            window.Clear();
            return window;
        }

        void Init()
        {
            OnInit();
        }
        
        private Queue<Action>  mPrivateCommands = new Queue<Action>();

        private Queue<Action> mCommands
        {
            get { return mPrivateCommands; }
        }

        public void PushCommand(Action command)
        {
            Debug.Log("push command");

            mCommands.Enqueue(command);
        }

        private void OnGUI()
        {
            ViewController.View.DrawGUI();

            while (mCommands.Count > 0)
            {
                Debug.Log(mCommands.Count);
                mCommands.Dequeue().Invoke();
            }
        }

        public void Dispose()
        {
            OnDispose();
        }

        protected bool mShowing = false;

        
        protected abstract void OnInit();
        protected abstract void OnDispose();
    }
}