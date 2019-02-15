using System.Collections.Generic;
using System.ComponentModel;

namespace QFramework.GraphDesigner
{
    public class ViewModel<TData> : ViewModel
    {
        public TData Data { get; set; }
    }

    public class ViewModel : INotifyPropertyChanged
    {
        private object _dataObject;

        public virtual object DataObject
        {
            get { return _dataObject; }
            set
            {
                _dataObject = value;
                DataObjectChanged();
                
            }
        }

        public string Identifier
        {
            get { return ((IGraphItem) DataObject).Identifier; }
        }

        public virtual void DataObjectChanged()
        {
            
        }

        protected bool SetProperty<T>(ref T backingField, T value, string name)
        {
            var changed = !EqualityComparer<T>.Default.Equals(backingField, value);

            if (changed)
            {
                backingField = value;
                this.OnPropertyChanged(name);
            }

            return changed;
        }
//#if UNITY_EDITOR
            public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
//#else 
//        public event PropertyChangedEventHandler PropertyChanged;

//        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
//        {
//            PropertyChangedEventHandler handler = PropertyChanged;
//            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
//        }
//#endif
      
    }
}