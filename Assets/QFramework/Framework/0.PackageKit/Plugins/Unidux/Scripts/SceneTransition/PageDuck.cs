using System;
using System.Linq;
using Unidux.Util;
using UnityEngine;

namespace Unidux.SceneTransition
{
    public static class PageDuck<TPage, TScene>
        where TPage : struct
        where TScene : struct
    {
        public interface IPageAction
        {
        }

        [Serializable]
        public class PushAction : PageEntity<TPage>, IPageAction
        {
            public PushAction(TPage page, IPageData data) : base(page, data)
            {
            }
        }

        public class PopAction : IPageAction
        {
        }

        [Serializable]
        public class ReplaceAction : PageEntity<TPage>, IPageAction
        {
            public ReplaceAction(TPage page, IPageData data) : base(page, data)
            {
            }
        }

        public class ResetAction : IPageAction
        {
        }

        public class AdjustAction : IPageAction
        {
        }

        public class SetDataAction : IPageAction
        {
            public IPageData Data { get; private set; }

            public SetDataAction(IPageData data)
            {
                this.Data = data;
            }
        }

        public static class ActionCreator
        {
            public static PushAction Push(TPage page, IPageData data = null)
            {
                return new PushAction(page, data);
            }

            public static PopAction Pop()
            {
                return new PopAction();
            }

            public static ReplaceAction Replace(TPage page, IPageData data = null)
            {
                return new ReplaceAction(page, data);
            }

            public static ResetAction Reset()
            {
                return new ResetAction();
            }

            public static SetDataAction SetData(IPageData data)
            {
                return new SetDataAction(data);
            }

            public static AdjustAction Adjust()
            {
                return new AdjustAction();
            }
        }

        public abstract class Reducer : IReducer
        {
            private ISceneConfig<TScene, TPage> config;

            public Reducer(ISceneConfig<TScene, TPage> config)
            {
                this.config = config;
            }

            public bool IsMatchedAction(object action)
            {
                return action is IPageAction;
            }

            public abstract object ReduceAny(object state, object action);

            public PageState<TPage> Reduce(PageState<TPage> pageState, SceneState<TScene> sceneState,
                IPageAction action)
            {
                if (action is PushAction)
                {
                    return ReducePush(pageState, sceneState, (PushAction) action);
                }
                else if (action is PopAction)
                {
                    return ReducePop(pageState, sceneState, (PopAction) action);
                }
                else if (action is ResetAction)
                {
                    return ReduceReset(pageState, sceneState, (ResetAction) action);
                }
                else if (action is SetDataAction)
                {
                    return ReduceSetData(pageState, (SetDataAction) action);
                }
                else if (action is ReplaceAction)
                {
                    return ReduceReplace(pageState, sceneState, (ReplaceAction)action);
                }
                else if (action is AdjustAction)
                {
                    return ReduceAdjust(pageState, sceneState);
                }

                return pageState;
            }

            public PageState<TPage> ReducePush(
                PageState<TPage> pageState,
                SceneState<TScene> sceneState,
                IPageEntity<TPage> action
            )
            {
                if (!config.PageMap.ContainsKey(action.Page))
                {
                    Debug.LogWarning(
                        "Page pushing failed. Missing configuration in SceneConfig.PageMap: " + action.Page);
                    return pageState;
                }

                if (pageState.Stack.Any() && pageState.Current.Page.Equals(action.Page))
                {
                    Debug.LogWarning(
                        "Page pushing failed. Cannot push same page at once: " + action.Page);
                    return pageState;
                }

                pageState.Stack.Add(action);
                pageState.SetStateChanged();

                ReduceAdjust(pageState, sceneState);
                return pageState;
            }

            public PageState<TPage> ReducePop(
                PageState<TPage> pageState,
                SceneState<TScene> sceneState,
                PopAction action
            )
            {
                // don't remove last page
                if (pageState.Stack.Count > 1)
                {
                    pageState.Stack.RemoveLast();
                }

                pageState.SetStateChanged();

                ReduceAdjust(pageState, sceneState);
                return pageState;
            }

            public PageState<TPage> ReduceReplace(
                PageState<TPage> pageState,
                SceneState<TScene> sceneState,
                IPageEntity<TPage> action
            )
            {
                if (pageState.Stack.Any())
                {
                    pageState.Stack.RemoveLast();
                }

                ReducePush(pageState, sceneState, action);
                return pageState;
            }

            public PageState<TPage> ReduceReset(PageState<TPage> pageState, SceneState<TScene> sceneState,
                ResetAction action)
            {
                pageState.Stack.Clear();
                SceneDuck<TScene>.Remove(sceneState, config.GetPageScenes());
                pageState.SetStateChanged();
                return pageState;
            }

            public PageState<TPage> ReduceSetData(PageState<TPage> state, SetDataAction action)
            {
                if (!state.IsReady)
                {
                    Debug.LogWarning("PageDuck.SetData failed. Set some page before you set page data");
                    return state;
                }
                
                state.Current.Data = action.Data;
                state.SetStateChanged();
                return state;
            }

            public PageState<TPage> ReduceAdjust(PageState<TPage> pageState, SceneState<TScene> sceneState)
            {
                SceneDuck<TScene>.Remove(sceneState, config.GetPageScenes());

                if (pageState.Stack.Any())
                {
                    var page = pageState.Current.Page;

                    if (!config.PageMap.ContainsKey(page))
                    {
                        Debug.LogWarning(
                            "Page adjust failed. Missing configuration in SceneConfig.PageMap: " + page);
                    }

                    var scenes = config.PageMap[page];

                    SceneDuck<TScene>.Add(sceneState, scenes);
                }

                sceneState.SetStateChanged();
                return pageState;
            }
        }
    }
}