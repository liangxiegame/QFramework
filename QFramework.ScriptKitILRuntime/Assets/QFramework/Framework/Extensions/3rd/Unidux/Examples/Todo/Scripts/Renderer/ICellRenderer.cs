namespace Unidux.Example.Todo
{
    public interface ICellRenderer<TValue>
    {
        void Render(int index, TValue item);
    }
}