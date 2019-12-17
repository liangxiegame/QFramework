using System.Collections.Generic;
using System.ComponentModel;

namespace QFramework.CodeGen
{
    public class ViewModel : INotifyPropertyChanged
    {
        private object mDataObject;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}