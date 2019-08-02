
#if UNITY_5_6_OR_NEWER
using System;
using System.Collections.Generic;
using System.Text;
using CI.HttpClient;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using QF.Extensions;

namespace QF.Action
{
	public class UnityAPI : IAPI
	{
		public IDisposable HttpGet(string url, Dictionary<string, string> headers, Action<string> onResponse)
		{
			return ObservableWWW.Get(url, headers).Subscribe(onResponse);
		}

		public IDisposable HttpPost(string url, Dictionary<string, string> headers, Dictionary<string, string> form,
			Action<string> onResponse)
		{
			var wwwForm = new WWWForm();

			form.ForEach((k, v) => { wwwForm.AddField(k, v); });

			return ObservableWWW.Post(url, wwwForm, headers)
				.Subscribe(onResponse);
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

		public IDisposable HttpDelete(string url, Dictionary<string, string> headers, System.Action onResponse)
		{
			var request = UnityWebRequest.Delete(url);

			if (headers.IsNotNull())
			{
				headers.ForEach((k, v) => { request.SetRequestHeader(k, v); });
			}

#if UNITY_2017_1_OR_NEWER
			return request.SendWebRequest()
				.AsAsyncOperationObservable()
				.Subscribe(operation => { onResponse.Invoke(); });
#else
			return request.Send()
				.AsAsyncOperationObservable()
				.Subscribe(operation => { onResponse.Invoke(); });
#endif
		}
	}
}
#endif