using System;

namespace Unidux.Example.SimpleHttp
{
    [Serializable]
    public partial class State : DvaState
    {
        public string Url = "";
        public int StatusCode = -1;
        public string Body = "";
    }
}