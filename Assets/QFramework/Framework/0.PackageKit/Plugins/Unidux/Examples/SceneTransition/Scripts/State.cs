using System;
using Unidux.SceneTransition;

namespace Unidux.Example.SceneTransition
{
    [Serializable]
    public partial class State : DvaState
    {
        public SceneState<Scene> Scene = new SceneState<Scene>();
        public PageState<Page> Page = new PageState<Page>();
    }
}
