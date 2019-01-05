#if __IOS__

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;

namespace QFramework
{
    public class XamariniOSAPI : IAPI
    {
        public IDisposable HttpGet(string url, Dictionary<string, string> headers, Action<string> onResponse)
        {
            var httpWebRequest = WebRequest.CreateHttp(url);

            httpWebRequest.Method = "GET";

            if (headers != null)
            {
                foreach (var headerKV in headers)
                {
                    httpWebRequest.Headers.Add(headerKV.Key, headerKV.Value);
                }
            }

            var response = httpWebRequest.GetResponseAsync();

            response.GetAwaiter().OnCompleted(() =>
            {
                var stream = new StreamReader(response.Result.GetResponseStream());

                var responseText = stream.ReadToEnd();
                stream.Close();
                response.Result.Close();

                onResponse(responseText);
            });

            return response;
        }

        public IDisposable HttpPatch(string url, Dictionary<string, string> headers, Dictionary<string, string> form,
            Action<string> onResponse)
        {

            var handler = new HttpClientHandler {AutomaticDecompression = DecompressionMethods.None};

            var httpclient = new HttpClient(handler) {BaseAddress = new Uri(url)};

            if (headers != null)
            {
                foreach (var headerKV in headers)
                {
                    httpclient.DefaultRequestHeaders.Add(headerKV.Key, headerKV.Value);
                }
            }

            var content = new FormUrlEncodedContent(form);

            var response = httpclient.PatchAsync(httpclient.BaseAddress, content);

            response.GetAwaiter().OnCompleted(() => { onResponse(response.Result.Content.ToString()); });

            return httpclient;
        }
    }
}
#endif