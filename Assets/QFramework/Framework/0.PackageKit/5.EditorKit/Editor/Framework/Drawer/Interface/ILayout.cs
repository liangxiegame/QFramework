namespace EGO.Framework
{
    public interface ILayout : IView
    {
        void AddChild(IView view);

        void RemoveChild(IView view);

        void Clear();
    }
}