using System;

namespace Invert.Data
{
    
    public interface IDataRecord : IValueItem
    {
        IRepository Repository { get; }
        
        bool Changed { set; }
    }

    public interface IDataHeirarchy : IDataRecord
    {
    }
}