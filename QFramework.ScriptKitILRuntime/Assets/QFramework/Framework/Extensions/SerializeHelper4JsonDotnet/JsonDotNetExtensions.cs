///****************************************************************************
// * Copyright (c) 2017 ~ 2019.8 liangxie
// * 
// * http://qframework.io
// * https://github.com/liangxiegame/QFramework
// * 
// * Permission is hereby granted, free of charge, to any person obtaining a copy
// * of this software and associated documentation files (the "Software"), to deal
// * in the Software without restriction, including without limitation the rights
// * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// * copies of the Software, and to permit persons to whom the Software is
// * furnished to do so, subject to the following conditions:
// * 
// * The above copyright notice and this permission notice shall be included in
// * all copies or substantial portions of the Software.
// * 
// * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// * THE SOFTWARE.
// ****************************************************************************/

namespace QF.Extensions
{
	// 防止自动进行 clrbinding
	#if UNITY_5_6_OR_NEWER
	using Newtonsoft.Json.Linq;
	using UnityEngine;

	/// <summary>
	/// Support Newtown.json,Adjust LitJons's api
	/// </summary>
	public static class JsonExtension
	{
		/// <summary>
		/// Check if json data contains key specified
		/// </summary>
		public static bool JsonDataContainsKey(this JToken self, string key)
		{
			if (self == null) return false;
			if (!self.IsObject()) return false;
			return self[key] != null;
		}

		public static bool IsNullOrUndefined(this JToken self)
		{
			return self == null || self.Type == JTokenType.Null || self.Type == JTokenType.Undefined || self.Type == JTokenType.None;
		}

		public static bool IsString(this JToken self)
		{
			return self != null && self.Type == JTokenType.String;
		}

		public static bool IsObject(this JToken self)
		{
			return self != null && self.Type == JTokenType.Object;
		}

		public static bool IsArray(this JToken self)
		{
			return self != null && self.Type == JTokenType.Array;
		}
		
		/// <summary>
		/// 临时的先放在这里
		/// </summary>
		/// <returns>The json value.</returns>
		/// <param name="vector2">Vector2.</param>
		public static string ToJsonValue(this Vector2 vector2)
		{
			return vector2.x + "," + vector2.y;
		}
		#endif
	}
}