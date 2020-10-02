using System;
using System.Collections.Generic;

namespace Invert.Data
{
    
    public interface IDataRecord : IValueItem
    {
        IRepository Repository { get; set; }
        
        bool Changed { get; set; }

        IEnumerable<string> ForeignKeys { get; } 
    }

    public interface IDataHeirarchy : IDataRecord
    {
    }
}