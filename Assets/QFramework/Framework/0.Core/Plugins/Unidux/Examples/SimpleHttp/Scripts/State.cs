using System;

namespace Unidux.Example.SimpleHttp
{
    [Serializable]
    public partial class State : StateBase
    {
        public string Url = "";
        public int StatusCode = -1;
        public string Body = "";
    }
}