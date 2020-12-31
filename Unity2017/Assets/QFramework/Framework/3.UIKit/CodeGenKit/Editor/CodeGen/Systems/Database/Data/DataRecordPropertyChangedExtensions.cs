using System;
using QF;

namespace Invert.Data
{
    public static class DataRecordPropertyChangedExtensions
    {
        public static void Changed<TType>(this IDataRecord record, string propertyName,ref TType beforeValue, TType value)
        {
            record.Changed = true; 
            var before = beforeValue;
            var after = value;
            if (record.Repository != null && !object.Equals(before, after))
            {
                record.Repository.Signal<IDataRecordPropertyBeforeChange>(_ => _.BeforePropertyChanged(record, propertyName, before, after));
            }
            beforeValue = after;
            if (record.Repository != null && !Equals(before, after))
            {
                record.Repository.Signal<IDataRecordPropertyChanged>(_ => _.PropertyChanged(record, propertyName, before, after));
            }
        }
    }
}
