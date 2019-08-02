using Unidux.SceneTransition;

namespace Unidux.Example.SceneTransition
{
    public class PageReducer : PageDuck<Page, Scene>.Reducer
    {
        public PageReducer() : base(new SceneConfig())
        {
        }
        
        public override object ReduceAny(object state, object action)
        {
            // It's required for detecting page scene state location
            var _state = (State) state;
            var _action = (PageDuck<Page, Scene>.IPageAction) action;
            _state.Page = base.Reduce(_state.Page, _state.Scene, _action);
            return state;
        }
    }
}