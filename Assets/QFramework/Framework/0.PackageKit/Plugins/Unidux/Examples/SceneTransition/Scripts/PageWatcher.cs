using Unidux.SceneTransition;
using UniRx;
using UnityEngine;

namespace Unidux.Example.SceneTransition
{
    public class PageWatcher : MonoBehaviour
    {
        private ISceneConfig<Scene, Page> config = new SceneConfig();

        void Start()
        {
            Unidux.Subject
                .Where(state => state.Page.IsStateChanged)
                .StartWith(Unidux.State)
                .Where(state => state.Page.IsReady)
                .Subscribe(state => UpdateScene(state))
                .AddTo(this);
        }

        void UpdateScene(State state)
        {
            if (state.Scene.NeedsAdjust(config.GetPageScenes(), config.PageMap[state.Page.Current.Page]))
            {
                Unidux.Dispatch(PageDuck<Page, Scene>.ActionCreator.Adjust());
            }
        }
    }
}