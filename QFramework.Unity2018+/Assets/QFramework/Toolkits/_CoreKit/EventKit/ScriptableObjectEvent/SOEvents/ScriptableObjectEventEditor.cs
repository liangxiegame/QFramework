/****************************************************************************
 * Copyright (c) 2016 - 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


namespace QFramework.Experimental
{
    [CustomEditor(typeof(ScriptableObjectEvent))]
    [CanEditMultipleObjects]
    public class ScriptableObjectEventEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //Draw the defualt inspector options
            DrawDefaultInspector();

            ScriptableObjectEvent script = (ScriptableObjectEvent)target;

            //Draw button
            if (GUILayout.Button("Trigger Event"))
            {
                //If the application is playing - raise/trigger the event
                if (EditorApplication.isPlaying)
                {
                    script.Trigger();
                }
            }
        }
    }
}
#endif