using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Unidux.Example.Todo
{
    [RequireComponent(typeof(Button))]
    public class TodoFilterButtonDispatcher : MonoBehaviour
    {
        public VisibilityFilter Filter;

        void OnEnable()
        {
            this.GetComponent<Button>()
                .OnClickAsObservable()
                .TakeUntilDisable(this)
                .Subscribe(_ => this.Dispatch())
                .AddTo(this);
        }

        void Dispatch()
        {
            Unidux.Store.Dispatch(TodoVisibilityDuck.ActionCreator.SetVisibility(Filter));
        }
    }
}