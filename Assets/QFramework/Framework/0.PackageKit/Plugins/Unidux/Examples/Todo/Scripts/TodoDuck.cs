using System.Linq;

namespace Unidux.Example.Todo
{
    public static class TodoDuck
    {
        public enum ActionType
        {
            ADD_TODO,
            TOGGLE_TODO,
        }

        public class Action
        {
            public ActionType ActionType;
            public Todo Todo;
        }

        public static class ActionCreator
        {
            public static Action CreateTodo(string text)
            {
                return new Action()
                {
                    ActionType = ActionType.ADD_TODO,
                    Todo = new Todo(text: text)
                };
            }

            public static Action ToggleTodo(Todo todo, bool completed)
            {
                return new Action()
                {
                    ActionType = ActionType.TOGGLE_TODO,
                    Todo = new Todo(id: todo.Id, text: todo.Text, completed: completed),
                };
            }
        }

        public class Reducer : ReducerBase<State, Action>
        {
            public override State Reduce(State state, Action action)
            {
                switch (action.ActionType)
                {
                    case ActionType.ADD_TODO:
                        state.Todo = AddTodo(state.Todo, action.Todo.Text);
                        return state;
                    case ActionType.TOGGLE_TODO:
                        state.Todo = ToggleTodo(state.Todo, action.Todo);
                        return state;
                }

                return state;
            }
        }

        public static TodoState AddTodo(TodoState state, string text)
        {
            state.List.Add(new Todo(
                id: state.Index,
                text: text
            ));
            state.Index = state.Index + 1;
            state.SetStateChanged();
            return state;
        }

        public static TodoState ToggleTodo(TodoState state, Todo newTodo)
        {
            var list = state.List.Select(_todo => (_todo.Id == newTodo.Id)
                    ? new Todo(
                        id: _todo.Id,
                        text: _todo.Text,
                        completed: newTodo.Completed
                    )
                    : _todo
                )
                .ToList();

            state.List = list;
            state.SetStateChanged();
            return state;
        }
    }
}