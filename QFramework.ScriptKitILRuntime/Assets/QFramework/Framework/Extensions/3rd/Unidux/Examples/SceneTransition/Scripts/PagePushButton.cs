using Unidux.SceneTransition;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

namespace Unidux.Example.SceneTransition
{
    [RequireComponent(typeof(Button))]
    public class PagePushButton : MonoBehaviour
    {
        public Page Page;

        void Start()
        {
            this.GetComponent<Button>().OnClickAsObservable()
                .Select(_ => this.Page)
                .Select(page => PageDuck<Page, Scene>.ActionCreator.Push(page))
                .Subscribe(action => Unidux.Dispatch(action))
                .AddTo(this);
        }
    }
}