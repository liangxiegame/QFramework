using System;
using System.ComponentModel;

namespace QF.GraphDesigner
{
    public static class ViewModelExtensions
    {
        public static System.Action SubscribeToProperty<TViewModel>(this TViewModel vm, string propertyName, Action<TViewModel> action) where TViewModel : ViewModel
        {
            PropertyChangedEventHandler handler = (sender, args) =>
            {
                action(sender as TViewModel);
            };;
            vm.PropertyChanged += handler;

            return ()=> { vm.PropertyChanged -= handler; };
        }
    }
}