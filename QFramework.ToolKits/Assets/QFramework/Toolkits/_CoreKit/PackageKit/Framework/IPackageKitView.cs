/****************************************************************************
 * Copyright (c) 2017 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEditor;

namespace QFramework
{
    public interface IPackageKitView
    {
        EditorWindow EditorWindow { get; set; }

        void Init();

        void OnShow();

        void OnUpdate();
        void OnGUI();

        void OnWindowGUIEnd();
        void OnHide();

        void OnDispose();
    }
}
#endif