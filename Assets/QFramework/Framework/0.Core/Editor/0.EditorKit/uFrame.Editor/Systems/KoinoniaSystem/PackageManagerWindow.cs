using QFramework.GraphDesigner;
using QFramework;
using UnityEditor;
using UnityEngine;

namespace QFramework.GraphDesigner.Unity.KoinoniaSystem
{
    public class PackageManagerWindow : EditorWindow
    {
        private QFrameworkContainer _container;

        void OnGUI()
        {
            Container = InvertApplication.Container;
            InvertApplication.SignalEvent<IDrawPackageManager>(_=>_.DrawPackageManager(new Rect(0,0,Screen.width,Screen.height)));
        }

        public QFrameworkContainer Container
        {
            get { return _container ?? (_container = InvertApplication.Container); }
            set { _container = value; }
        }

        void Update()
        {
            Repaint();
        }
    }
}