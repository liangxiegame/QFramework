/*
 * 打算实现一套绑定机制
 * 支持：
 * 数据绑定
 * 依赖绑定
 * 事件到命令绑定
 * 支持注入功能
 * 支持对象映射类似 AutoMapper
 */

using BindKit.Binding;
using BindKit.Binding.Binders;
using BindKit.Binding.Builder;
using BindKit.Binding.Contexts;

namespace QFramework
{
    public class BindKit
    {
        private static IQFrameworkContainer mContainer;
        
        public static void Init(IQFrameworkContainer container = null)
        {
            if (container == null)
            {
                mContainer = new QFrameworkContainer();
            }
            else
            {
                mContainer = container;
            }
            
            new BindingServiceBundle(mContainer)
                .Start();
        }
        
        
        public static BindingSet<TView, TViewModel> CreateBindingSet<TView, TViewModel>(TView view,
            TViewModel viewModel) where TView : class
        {
            var binder = mContainer.Resolve<IBinder>();

            var bindContext = new BindingContext(view, binder) {DataContext = viewModel};

            return new BindingSet<TView, TViewModel>(bindContext, view);
        }

        public static IQFrameworkContainer GetCotnainer()
        {
            return mContainer;
        }

        public static void Clear()
        {
            new BindingServiceBundle(mContainer)
                .Stop();
        }
    }
}