using Unidux.SceneTransition;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Unidux.Example.SceneTransition
{
    [RequireComponent(typeof(Button))]
    public class PagePopButton : MonoBehaviour
    {
        void Start()
        {
            this.GetComponent<Button>().OnClickAsObservable()
                .Select(_ => PageDuck<Page, Scene>.ActionCreator.Pop())
                .Subscribe(action => Unidux.Dispatch(action))
                .AddTo(this);
        }
    }
}