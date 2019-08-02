using System;
using System.Collections.Generic;

namespace Unidux.Example.List
{
    [Serializable]
    public class ListState : StateElement
    {
        public List<string> Texts = new List<string>();
    }
}
