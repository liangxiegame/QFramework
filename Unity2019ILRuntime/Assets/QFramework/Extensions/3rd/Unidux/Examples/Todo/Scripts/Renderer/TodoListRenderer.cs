using UniRx;

namespace Unidux.Example.Todo
{
    public class TodoListRenderer : BaseListRenderer<TodoCellRenderer, Todo>
    {
        public TodoCellRenderer Cell;

        void Start()
        {
            Unidux.Subject
                .Where(state => state.Todo.IsStateChanged)
                .StartWith(Unidux.State)
                .Subscribe(state => this.Render(this.transform, this.Cell, state.Todo.ListByFilter))
                .AddTo(this);
        }

        protected override void RenderCell(int index, TodoCellRenderer renderer, Todo value)
        {
            renderer.GetComponent<TodoToggleDispatcher>().Todo = value;
            base.RenderCell(index, renderer, value);
        }
    }
}