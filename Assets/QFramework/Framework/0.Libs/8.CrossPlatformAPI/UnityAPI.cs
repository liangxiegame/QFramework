#if UNITY_5_6_OR_NEWER
using System;
using System.Collections.Generic;
using System.Text;
using CI.HttpClient;
using Newtonsoft.Json.Linq;
using UniRx;

namespace QFramework
{
	public class UnityAPI : IAPI
	{
		public IDisposable HttpGet(string url, Dictionary<string, string> headers, Action<string> onResponse)
		{
			return ObservableWWW.Get(url, headers).Subscribe(onResponse);
		}

		public IDisposable HttpPatch(string url, Dictionary<string, string> headers,
			Dictionary<string, string> contentDict, Action<string> onResponse)
		{
			var httpClient = new HttpClient();

			if (headers.IsNotNull())
			{
				headers.ForEach((key, value) => { httpClient.Headers.Add(key, value); });
			}

			var jsonForm = new JObject();

			contentDict.ForEach((key, value) => { jsonForm[key] = value; });

			var content = new ByteArrayContent(Encoding.UTF8.GetBytes(jsonForm.ToString()), "application/json");
			
			httpClient.Patch(new Uri(url), content, (responseContent) => { onResponse(responseContent.Data); });

			return Disposable.Create(() => httpClient.Abort());
		}
	}
}
#endif