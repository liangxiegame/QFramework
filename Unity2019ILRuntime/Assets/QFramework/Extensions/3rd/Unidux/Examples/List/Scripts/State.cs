using System;

namespace Unidux.Example.List
{
    [Serializable]
    public class State : DvaState
    {
        public ListState List = new ListState();
    }
}
