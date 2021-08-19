namespace Unidux.Util
{
    public static class StateUtil
    {
        // TODO: optimize
        public static bool ApplyStateChanged(IStateChanged oldState, IStateChanged newState)
        {
            if (oldState == null || newState == null)
            {
                return oldState != null || newState != null;
            }

            bool stateChanged = false;

            var properties = newState.GetType().GetProperties();
            foreach (var property in properties)
            {
                var newValue = property.GetValue(newState, null);
                var oldValue = property.GetValue(oldState, null);

                if (newValue is IStateChanged)
                {
                    stateChanged |= ApplyStateChanged((IStateChanged) oldValue, (IStateChanged) newValue);
                }
                else if (newValue == null && oldValue != null || newValue != null && !newValue.Equals(oldValue))
                {
                    stateChanged = true;
                }
            }

            var fields = newState.GetType().GetFields();
            foreach (var field in fields)
            {
                var newValue = field.GetValue(newState);
                var oldValue = field.GetValue(oldState);

                if (newValue is IStateChanged)
                {
                    stateChanged |= ApplyStateChanged((IStateChanged) oldValue, (IStateChanged) newValue);
                }
                else if (newValue == null && oldValue != null || newValue != null && !newValue.Equals(oldValue))
                {
                    stateChanged = true;
                }
            }

            if (stateChanged)
            {
                newState.SetStateChanged();
            }

            return stateChanged;
        }

        public static void ResetStateChanged(IStateChanged state)
        {
            if (state != null)
            {
                state.SetStateChanged(false);
            }

            var properties = state.GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(state, null);

                if (value != null && value is IStateChanged)
                {
                    var changedValue = (IStateChanged) value;
                    changedValue.SetStateChanged(false);
                }
            }

            var fields = state.GetType().GetFields();
            foreach (var field in fields)
            {
                var value = field.GetValue(state);
                if (value != null && value is IStateChanged)
                {
                    var changedValue = (IStateChanged) value;
                    changedValue.SetStateChanged(false);
                }
            }
        }
    }
}