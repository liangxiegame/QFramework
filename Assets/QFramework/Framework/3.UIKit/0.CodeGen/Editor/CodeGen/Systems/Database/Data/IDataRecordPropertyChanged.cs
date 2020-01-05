namespace Invert.Data
{
    public interface IDataRecordPropertyChanged
    {
        void PropertyChanged(IDataRecord record, string name,object previousValue, object nextValue);
    }
}