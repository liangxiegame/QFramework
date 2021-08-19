using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Unidux.Example.Todo
{
    [RequireComponent(typeof(Button))]
    public class TodoFilterButtonRenderer : MonoBehaviour
    {
        public VisibilityFilter Filter;

        void Start()
        {
            var button = this.GetComponent<Button>();

            Unidux.Subject
                .Where(state => state.Todo.IsStateChanged)
                .StartWith(Unidux.State)
                .Subscribe(state => button.interactable = Filter != state.Todo.Filter)
                .AddTo(this);
        }
    }
}