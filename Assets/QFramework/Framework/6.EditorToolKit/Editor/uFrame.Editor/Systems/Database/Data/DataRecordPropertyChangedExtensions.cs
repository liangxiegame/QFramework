using System;
using System.Linq;
using QF;

namespace Invert.Data
{
    public static class DataRecordPropertyChangedExtensions
    {
        public static bool IsNear(this IDataRecord record, IDataRecord to)
        {
            if (to == null) return false;
            return record.Identifier == to.Identifier || record.ForeignKeys.Contains(to.Identifier);
        }
        public static TType Copy<TType>(this TType record) where TType : class, IDataRecord
        {
            var result = InvertJsonExtensions.DeserializeObject<TType>((string)InvertJsonExtensions.SerializeObject(record).ToString()) as TType;
            result.Identifier = Guid.NewGuid().ToString();
            return result; 
        }

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
            if (record.Repository != null && !object.Equals(before, after))
            {
                record.Repository.Signal<IDataRecordPropertyChanged>(_ => _.PropertyChanged(record, propertyName, before, after));
            }
        }
    }
}
