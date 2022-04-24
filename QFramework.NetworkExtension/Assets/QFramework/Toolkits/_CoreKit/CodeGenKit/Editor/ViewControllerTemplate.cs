/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEditor;

namespace QFramework
{
    [InitializeOnLoad]
    public class ViewControllerTemplate : ICodeGenTemplate
    {
        static ViewControllerTemplate()
        {
            CodeGenKit.RegisterTemplate(nameof(ViewController), new ViewControllerTemplate());
        }
        
        public CodeGenTask CreateTask(IBindGroup bindGroup)
        {
            var viewController = bindGroup.As<ViewController>();

            return new CodeGenTask()
            {
                GameObject = viewController.gameObject,
                From = GameObjectFrom.Scene,
                ClassName = viewController.ScriptName,
                ScriptsFolder = viewController.ScriptsFolder,
                Namespace = viewController.Namespace
            };
        }
    }
}
#endif