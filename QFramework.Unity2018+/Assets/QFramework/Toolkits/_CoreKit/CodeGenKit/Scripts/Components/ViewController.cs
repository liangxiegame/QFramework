/****************************************************************************
 * Copyright (c) 2017 xiaojun
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/


using UnityEngine;

namespace QFramework
{
    public class ViewController : MonoBehaviour, IBindGroup
    {
        [HideInInspector] public bool IsUseNamespace = true;

        [HideInInspector] public string Namespace = string.Empty;

        [HideInInspector] public string ScriptName;

        [HideInInspector] public string ScriptsFolder = string.Empty;

        [HideInInspector] public bool GeneratePrefab = false;


        [HideInInspector] public string PrefabFolder = string.Empty;

        public string TemplateName => nameof(ViewController);
    }
}