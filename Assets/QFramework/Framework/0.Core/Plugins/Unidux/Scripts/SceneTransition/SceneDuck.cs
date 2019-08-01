using System;
using System.Collections.Generic;
using System.Linq;
using Unidux.Util;

namespace Unidux.SceneTransition
{
    public static class SceneDuck<TScene>
        where TScene : struct
    {
        public enum ActionType
        {
            Adjust,
            Add,
            Remove,
        }

        public class Action
        {
            public IEnumerable<TScene> Scenes;
            public ActionType Type;

            public Action(ActionType type, IEnumerable<TScene> scenes)
            {
                this.Scenes = scenes;
                this.Type = type;
            }
        }

        public static class ActionCreator
        {
            public static Action Adjust(IEnumerable<TScene> scenes)
            {
                return new Action(ActionType.Adjust, scenes);
            }

            public static Action Add(IEnumerable<TScene> scenes)
            {
                return new Action(ActionType.Add, scenes);
            }

            public static Action Remove(IEnumerable<TScene> scenes)
            {
                return new Action(ActionType.Remove, scenes);
            }
        }

        public class Reducer : ReducerBase<SceneState<TScene>, Action>
        {
            public override SceneState<TScene> Reduce(SceneState<TScene> state, Action action)
            {
                switch (action.Type)
                {
                    case ActionType.Adjust:
                        ResetAll(state);
                        Add(state, action.Scenes);
                        state.SetStateChanged();
                        break;
                    case ActionType.Add:
                        Add(state, action.Scenes);
                        state.SetStateChanged();
                        break;
                    case ActionType.Remove:
                        Remove(state, action.Scenes);
                        state.SetStateChanged();
                        break;
                }

                return state;
            }
        }

        public static void ResetAll(SceneState<TScene> state)
        {
            Set(state.ActiveMap, EnumUtil.All<TScene>(), false);
        }

        public static void Add(SceneState<TScene> state, IEnumerable<TScene> scenes)
        {
            Set(state.ActiveMap, scenes, true);
        }

        public static void Remove(SceneState<TScene> state, IEnumerable<TScene> scenes)
        {
            Set(state.ActiveMap, scenes, false);
        }

        public static void Set(IDictionary<TScene, bool> activeMap, IEnumerable<TScene> scenes, bool value)
        {
            foreach (var scene in scenes)
            {
                activeMap[scene] = value;
            }
        }
    }
}