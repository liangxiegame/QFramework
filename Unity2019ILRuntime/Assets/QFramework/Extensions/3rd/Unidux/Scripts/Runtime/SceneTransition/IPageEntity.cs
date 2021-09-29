namespace Unidux.SceneTransition
{
    public interface IPageEntity<TPage> where TPage : struct
    {
        TPage Page { get; }
        IPageData Data { get; set; }
    }
}