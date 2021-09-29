using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Unidux.Example.Todo
{
    public class TodoToggleDispatcher : MonoBehaviour
    {
        public Toggle Toggle;
        public Todo Todo { private get; set; }

        void OnEnable()
        {
            this.Toggle.OnValueChangedAsObservable()
                .TakeUntilDisable(this)
                .Where(_ => Todo != null)
                .DistinctUntilChanged()
                .Where(value => value != Todo.Completed)
                .Subscribe(value => this.Dispatch(value))
                .AddTo(this);
        }

        void Dispatch(bool value)
        {
            Unidux.Store.Dispatch(TodoDuck.ActionCreator.ToggleTodo(this.Todo, value));
        }
    }
}