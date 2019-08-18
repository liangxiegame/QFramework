using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QF.Master
{
    public class EditorPlatformContainer
    {
        /// <summary>
        /// 用来缓存的模s块
        /// </summary>
        private List<object> mInstances = new List<object>();

        /// <summary>
        /// 溶解（获取全部)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> ResolveAll<T>()
        {
            return mInstances.OfType<T>()
                .ToList();
        }
        

        public void Init()
        {
            // 清空掉之前的实例
            mInstances.Clear();
            
            // 1.获取当前项目中所有的 assembly (可以理解为 代码编译好的 dll)
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            // 2.获取编辑器环境(dll)
            var editorAssembly = assemblies.First(assembly => assembly.FullName.StartsWith("Assembly-CSharp-Editor"));
            // 3.获取 IEditorPlatformModule 类型
            var moduleType = typeof(IEditorPlatformModule);

            mInstances = editorAssembly
                // 获取所有的编辑器环境中的类型 
                .GetTypes() 
                // 过滤掉抽象类型（接口/抽象类)、和未实现 IEditorPlatformModule 的类型
                .Where(type => moduleType.IsAssignableFrom(type) && !type.IsAbstract) 
                // 获取类型的构造创建实例
                .Select(type => type.GetConstructors().First().Invoke(null))
                // 转换成 List<IEditorPlatformModule>
                .ToList();
        }
    }
    
    public class EditorPlatform : EditorWindow
    {
        private EditorPlatformContainer mContainer;

        /// <summary>
        /// 打开窗口
        /// </summary>
        [MenuItem("QF.Master/1.EditorPlatform")]
        public static void Open()
        {
            var editorPlatform = GetWindow<EditorPlatform>();
            editorPlatform.position = new Rect(
                Screen.width / 2,
                Screen.height * 2 / 3,
                600,
                500
            );

            // 初始化 Container
            editorPlatform.mContainer = new EditorPlatformContainer();
            
            editorPlatform.mContainer.Init();

            editorPlatform.Show();
        }

        private void OnGUI()
        {
            // 渲染
            mContainer.ResolveAll<IEditorPlatformModule>()
                .ForEach(editorPlatformModule=>editorPlatformModule.OnGUI());
        }
    }
}