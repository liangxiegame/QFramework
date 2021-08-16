/****************************************************************************
 * Copyright (c) 2017 imagicbell
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 ~ 2020.10 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
	using System.IO;
	using System.Xml.Serialization;
	
	// 为了防止进行 clrbidning

    public static class SerializeHelper
	{
		public static IQFrameworkContainer SerializeContainer = new QFrameworkContainer();

		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void Init()
		{
			// 默认注入 Unity 官方的序列化器
			SerializeContainer.RegisterInstance<IJsonSerializer>(new DefaultJsonSerializer());
		}
		
		
		// 为了防止进行 clrbinding
		public static string ToJson<T>(this T obj) where T : class
		{
			return SerializeContainer.Resolve<IJsonSerializer>().SerializeJson(obj);
		}

		public static T FromJson<T>(this string json) where T : class
		{
			return SerializeContainer.Resolve<IJsonSerializer>().DeserializeJson<T>(json);
		}

		public static string SaveJson<T>(this T obj, string path) where T : class
		{
			var jsonContent = obj.ToJson();
			File.WriteAllText(path, jsonContent);
			return jsonContent;
		}

		public static T LoadJson<T>(this string path) where T : class
		{
			if (path.Contains(Application.streamingAssetsPath))
			{
				using (var streamReader = new StreamReader(path))
				{
					return FromJson<T>(streamReader.ReadToEnd());
				}
			}
			
			return File.ReadAllText(path).FromJson<T>();
		}
		
		public static bool SerializeXML(string path, object obj)
		{
			if (string.IsNullOrEmpty(path))
			{
				Log.W("SerializeBinary Without Valid Path.");
				return false;
			}

			if (obj == null)
			{
				Log.W("SerializeBinary obj is Null.");
				return false;
			}

			using (var fs = new FileStream(path, FileMode.OpenOrCreate))
			{
				var xmlserializer = new XmlSerializer(obj.GetType());
				xmlserializer.Serialize(fs, obj);
				return true;
			}
		}

		public static object DeserializeXML<T>(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				Log.W("DeserializeBinary Without Valid Path.");
				return null;
			}

			FileInfo fileInfo = new FileInfo(path);

			using (FileStream fs = fileInfo.OpenRead())
			{
				XmlSerializer xmlserializer = new XmlSerializer(typeof(T));
				object data = xmlserializer.Deserialize(fs);

				if (data != null)
				{
					return data;
				}
			}

			Log.W("DeserializeBinary Failed:" + path);
			return null;
		}
	}
}
