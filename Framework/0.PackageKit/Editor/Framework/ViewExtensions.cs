using System;
using BindKit.ViewModels;

namespace QFramework.PackageKit
{
    public static class ViewExtensions
    {
        public static TView Do<TView>(this TView self, Action<TView> onDo) where TView : IView
        {
            onDo(self);
            return self;
        }
    }
    
    public static class ViewModelExtensions
    {
        public static TViewModel InjectSelfWithContainer<TViewModel>(this TViewModel self, IQFrameworkContainer container) where TViewModel : IViewModel
        {
            
            container.Inject(self);
            
            return self;
        }

        public static TViewModel Init<TViewModel>(this TViewModel self) where TViewModel : ViewModelBase
        {
            self.OnInit();
            return self;
        }
    }
}