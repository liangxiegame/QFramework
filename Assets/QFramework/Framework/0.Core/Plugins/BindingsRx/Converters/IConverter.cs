namespace BindingsRx.Converters
{
    public interface IConverter<T1, T2>
    {
        T1 From(T2 value);
        T2 From(T1 value);
    }
}