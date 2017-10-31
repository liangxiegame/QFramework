/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 imagicbell 
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
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

namespace QFramework
{
    using Newtonsoft.Json;
    using System.IO;
	using System.Xml.Serialization;


	public static class SerializeHelper
	{
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
				object data = bf.Deserialize(stream);

				// TODO:这里没风险嘛?
				if (data != null)
				{
					return data;
				}
				stream.Close();
			}

			Log.W("DeserializeBinary Failed!");
			return null;
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

			using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
			{
				XmlSerializer xmlserializer = new XmlSerializer(obj.GetType());
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

        public static string ToJson<T>(this T obj) where T : class
		{
			return JsonConvert.SerializeObject(obj,Formatting.Indented);
		}

		public static T FromJson<T>(this string json) where T : class
		{
			return JsonConvert.DeserializeObject<T>(json);
		}

		public static void SaveJson<T>(this T obj, string path) where T : class
		{
			File.WriteAllText(path, obj.ToJson<T>());
		}

		public static T LoadJson<T>(string path) where T : class
		{
			return File.ReadAllText(path).FromJson<T>();
		}

		public static byte[] ToProtoBuff<T>(this T obj) where T : class
		{
			using (MemoryStream ms = new MemoryStream())
			{
				ProtoBuf.Serializer.Serialize<T>(ms, obj);
				return ms.ToArray();
			}
		}

		public static T FromProtoBuff<T>(this byte[] bytes) where T : class
		{
			if (bytes == null || bytes.Length == 0)
			{
				throw new System.ArgumentNullException("bytes");
			}
			T t = ProtoBuf.Serializer.Deserialize<T>(new MemoryStream(bytes));
			return t;
		}

		public static void SaveProtoBuff<T>(this T obj, string path) where T : class
		{
			File.WriteAllBytes(path, obj.ToProtoBuff<T>());
		}

		public static T LoadProtoBuff<T>(string path) where T : class
		{
			return File.ReadAllBytes(path).FromProtoBuff<T>();
		}
	}
}