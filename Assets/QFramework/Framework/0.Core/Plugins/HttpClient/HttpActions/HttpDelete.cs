using System;
using System.Net;

namespace CI.HttpClient.Core
{
    public class HttpDelete : HttpBase
    {
        public HttpDelete(HttpWebRequest request, IDispatcher dispatcher)
        {
            _request = request;
            _dispatcher = dispatcher;
        }

        public void Delete(Action<HttpResponseMessage<string>> responseCallback)
        {
            try
            {
                SetMethod(HttpAction.Delete);
                HandleStringResponseRead(responseCallback);
            }
            catch (Exception e)
            {
                RaiseErrorResponse(responseCallback, e);
            }
        }

        public void Delete(HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback, int blockSize)
        {
            try
            {
                SetMethod(HttpAction.Delete);
                HandleByteArrayResponseRead(responseCallback, completionOption, blockSize);
            }
            catch (Exception e)
            {
                RaiseErrorResponse(responseCallback, e);
            }
        }
    }
}