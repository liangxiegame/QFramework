using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Unidux.Example.Todo
{
    public class TodoDispatcher : MonoBehaviour
    {
        public InputField InputField;

        void OnEnable()
        {
            this.InputField.OnEndEditAsObservable()
                .TakeUntilDisable(this)
                .Subscribe(text => this.Dispatch(text))
                .AddTo(this);
        }

        void Dispatch(string text)
        {
            Unidux.Store.Dispatch(TodoDuck.ActionCreator.CreateTodo(text));
            this.InputField.text = "";
        }
    }
}