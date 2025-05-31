/****************************************************************************
 * Copyright (c) 2017 xiaojun
 * Copyright (c) 2015 ~ 2024 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/


using System;
using UnityEngine;

namespace QFramework
{
    public class ViewController : MonoBehaviour, IBindGroup
    {
        [HideInInspector] public string Namespace = string.Empty;

        [HideInInspector] public string ScriptName;

        [HideInInspector] public string ScriptsFolder = string.Empty;

        [HideInInspector] public bool GeneratePrefab = false;

        [HideInInspector] public string PrefabFolder = string.Empty;
        
        [HideInInspector] public string ArchitectureFullTypeName = string.Empty;
        
        [HideInInspector] public string ViewControllerFullTypeName = string.Empty;

        public string TemplateName => nameof(ViewController);
    }
    
    public class ViewControllerChildAttribute : Attribute
    {
    }
}