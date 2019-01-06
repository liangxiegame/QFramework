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

        public IDisposable HttpPost(string url, Dictionary<string, string> headers, Dictionary<string, string> form,
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

            var response = httpclient.PostAsync(httpclient.BaseAddress, content);

            response.GetAwaiter().OnCompleted(() =>
            {
                var readStringTask = response.Result.Content.ReadAsStringAsync();

                readStringTask.GetAwaiter().OnCompleted(() => { onResponse(readStringTask.Result); });
            });

            return httpclient;
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

        public IDisposable HttpDelete(string url, Dictionary<string, string> headers, Action onResponse)
        {
            var httpWebRequest = WebRequest.CreateHttp(url);

            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "DELETE";

            if (headers != null)
            {
                foreach (var headerKV in headers)
                {
                    httpWebRequest.Headers.Add(headerKV.Key, headerKV.Value);
                }
            }


            var httpWebResponse = httpWebRequest.GetResponseAsync();

            httpWebResponse.GetAwaiter().OnCompleted(() =>
            {
                var streamReader = new StreamReader(httpWebResponse.Result.GetResponseStream());

//                var responseContent = streamReader.ReadToEnd();

                streamReader.Close();
                httpWebRequest.Abort();

                onResponse();
            });

            return httpWebResponse;
        }
    }
}
#endif