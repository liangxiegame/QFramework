/****************************************************************************
 * Copyright (c) 2015 - 2024 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class DeclareKitMenu
    {
        
        [MenuItem("GameObject/QFramework/DeclareKit/@(Alt+D)Add Declare Component &d", false, 0)]
        static void AddView()
        {
            var gameObject = Selection.objects.First() as GameObject;

            if (!gameObject)
            {
                Debug.LogWarning("需要选择 GameObject");
                return;
            }

            var view = gameObject.GetComponent<DeclareComponent>();

            if (!view)
            {
                gameObject.AddComponent<DeclareComponent>();
            }
        }

    }
}
#endif