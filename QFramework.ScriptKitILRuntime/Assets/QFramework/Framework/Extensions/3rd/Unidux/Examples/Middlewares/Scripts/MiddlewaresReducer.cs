using System;
using UnityEngine;

namespace Unidux.Example.Middlewares
{
    public static class MiddlewareDuck
    {
        public class Action
        {
            public float StartTime;

            public Action(float time)
            {
                this.StartTime = time;
            }

            public override string ToString()
            {
                return JsonUtility.ToJson(this);
            }
        }

        public class Reducer : ReducerBase<State, Action>
        {
            public override State Reduce(State state, Action action)
            {
                if (action.StartTime < 0f) throw new Exception("StartTime must be greater than 0");

                state.StartTime = action.StartTime;
                return state;
            }
        }
    }
}