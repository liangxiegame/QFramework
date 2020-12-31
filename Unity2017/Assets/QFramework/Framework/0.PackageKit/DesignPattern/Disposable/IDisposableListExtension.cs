using System;

namespace QFramework
{
    /// <summary>
    /// 可回收List 拓展方法
    /// </summary>
    public static class IDisposableExtensions
    {
        /// <summary>
        /// 给继承IDisposable接口的对象 拓展相关Add方法
        /// </summary>
        /// <param name="self"></param>
        /// <param name="component"></param>
        public static void AddTo(this IDisposable self, IDisposableList component)
        {
            component.Add(self);
        }
    }
    
}