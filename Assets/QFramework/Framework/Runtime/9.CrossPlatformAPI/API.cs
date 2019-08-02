using System;
using System.Collections.Generic;

namespace QF.Action
{
    public interface IAPI
    {
        IDisposable HttpGet(string url, Dictionary<string, string> headers, Action<string> onResponse);

        IDisposable HttpPost(string url, Dictionary<string, string> headers,
            Dictionary<string, string> form, Action<string> onResponse);

        IDisposable HttpPatch(string url, Dictionary<string, string> headers,Dictionary<string,string> content, Action<string> onResponse);

        IDisposable HttpDelete(string url, Dictionary<string, string> headers, System.Action onResponse);

    }

    public static class API
    {
#if UNITY_5_6_OR_NEWER
        private static readonly IAPI mAPI = new UnityAPI();
#elif __IOS__
        private static readonly IAPI mAPI = new XamariniOSAPI();
#endif

        public static IDisposable HttpGet(string url, Dictionary<string, string> headers, Action<string> onResponse)
        {
            return mAPI.HttpGet(url, headers, onResponse);
        }

        public static IDisposable HttpPost(string url, Dictionary<string, string> headers,
            Dictionary<string, string> form, Action<string> onResponse)
        {
            return mAPI.HttpPost(url, headers, form, onResponse);
        }

        public static IDisposable HttpPatch(string url, Dictionary<string, string> headers,Dictionary<string,string> content, Action<string> onResponse)
        {
            return mAPI.HttpPatch(url, headers,content, onResponse);
        }

        public static IDisposable HttpDelete(string url, Dictionary<string, string> headers, System.Action onResponse)
        {
            return mAPI.HttpDelete(url, headers, onResponse);
        }
    }
}