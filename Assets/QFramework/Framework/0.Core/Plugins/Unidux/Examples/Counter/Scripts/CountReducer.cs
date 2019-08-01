namespace Unidux.Example.Counter
{
    public static class Count
    {
        // specify the possible types of actions
        public enum ActionType
        {
            Increment,
            Decrement
        }

        // actions must have a type and may include a payload
        public class Action
        {
            public ActionType ActionType;
        }
        
        // ActionCreators creates actions and deliver payloads
        // in redux, you do not dispatch from the ActionCreator to allow for easy testability
        public static class ActionCreator
        {
            public static Action Create(ActionType type)
            {
                return new Action() {ActionType = type};
            }

            public static Action Increment()
            {
                return new Action() {ActionType = ActionType.Increment};
            }

            public static Action Decrement()
            {
                return new Action() {ActionType = ActionType.Decrement};
            }
        }

        // reducers handle state changes
        public class Reducer : ReducerBase<State, Action>
        {
            public override State Reduce(State state, Action action)
            {
                switch (action.ActionType)
                {
                    case ActionType.Increment:
                        state.Count++;
                        break;
                    case ActionType.Decrement:
                        state.Count--;
                        break;
                }

                return state;
            }
        }
    }
}