using System;
using System.Collections.Generic;

namespace Invert.Data
{
    public class KeyProperty : Attribute
    {
        
    }

    public interface IDataRecord : IValueItem
    {
        IRepository Repository { get; set; }
        
        bool Changed { get; set; }

        IEnumerable<string> ForeignKeys { get; } 
    }

    public interface IDataHeirarchy : IDataRecord
    {
        IEnumerable<IDataRecord> ChildRecords { get; } 
    }

    public interface IDynamicDataRecord : IDataRecord
    {
        
    }
}