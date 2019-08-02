using System;

namespace Unidux.Example.List
{
    [Serializable]
    public class State : StateBase
    {
        public ListState List = new ListState();
    }
}
