/****************************************************************************
 * Copyright (c) 2017 imagicbell
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 ~ 7 liangxie
 *
 * TODO: 这个应该写成扩展关键字方式的
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

    public static partial class SerializeHelper
	{
		// 为了防止进行 clrbinding
		public static string ToJson<T>(this T obj) where T : class
		{
			return Core.GetUtility<IJsonSerializeUtility>().SerializeJson(obj);
		}

		public static T FromJson<T>(this string json) where T : class
		{
			return Core.GetUtility<IJsonSerializeUtility>().DeserializeJson<T>(json);
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
		
		public static bool SerializeBinary(string path, object obj)
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

			using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
			{
				System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
					new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				bf.Serialize(fs, obj);
				return true;
			}
		}


		public static object DeserializeBinary(Stream stream)
		{
			if (stream == null)
			{
				Log.W("DeserializeBinary Failed!");
				return null;
			}

			using (stream)
			{
				System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
					new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				var data = bf.Deserialize(stream);

				// TODO:这里没风险嘛?
				return data;
			}
		}

		public static object DeserializeBinary(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				Log.W("DeserializeBinary Without Valid Path.");
				return null;
			}

			FileInfo fileInfo = new FileInfo(path);

			if (!fileInfo.Exists)
			{
				Log.W("DeserializeBinary File Not Exit.");
				return null;
			}

			using (FileStream fs = fileInfo.OpenRead())
			{
				System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
					new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				object data = bf.Deserialize(fs);

				if (data != null)
				{
					return data;
				}
			}

			Log.W("DeserializeBinary Failed:" + path);
			return null;
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