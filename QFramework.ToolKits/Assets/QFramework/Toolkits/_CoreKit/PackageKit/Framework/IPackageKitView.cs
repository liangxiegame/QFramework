#if UNITY_EDITOR
using System;
using UnityEditor;

namespace QFramework
{
    public interface IPackageKitView
    {

        EditorWindow EditorWindow { get; set; }

        Type Type { get; }

        void Init();

        void OnUpdate();
        void OnGUI();

        void OnWindowGUIEnd();

        void OnDispose();
        void OnShow();
        void OnHide();
    }
}
#endif