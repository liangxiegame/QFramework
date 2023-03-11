/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QFramework
{
    public class EasyInspectorEditor : Editor, IMGUILayoutRoot
    {
        VerticalLayout IMGUILayoutRoot.Layout { get; set; }
        RenderEndCommandExecutor IMGUILayoutRoot.RenderEndCommandExecutor { get; set; }

        protected void Save()
        {
            EditorUtility.SetDirty(target);
            UnityEditor.SceneManagement.EditorSceneManager
                .MarkSceneDirty(SceneManager.GetActiveScene());
            GUIUtility.ExitGUI();
        }
    }
}
#endif