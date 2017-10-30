/****************************************************************************
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
	using UnityEngine;

#if SLUA_SUPPORT
	using SLua;
	[CustomLuaClass]
#endif

	/// <summary>
	/// Transform's Extension
	/// </summary>
	public static class TransformExtension
	{
		/// <summary>
		/// 缓存的一些变量
		/// </summary>
		private static Vector3 mLocalPos;
		private static Vector3 mLocalScale;

		public static T LocalIdentity<T>(this T selfComponent) where T : Component
		{
			selfComponent.transform.localPosition = Vector3.zero;
			selfComponent.transform.localRotation = Quaternion.identity;
			selfComponent.transform.localScale = Vector3.one;
			return selfComponent;
		}

		public static T LocalPositionX<T>(this T selfComponent, float x) where T : Component
		{
			mLocalPos = selfComponent.transform.localPosition;
			mLocalPos.x = x;
			selfComponent.transform.localPosition = mLocalPos;
			return selfComponent;
		}

		public static T LocalPositionY<T>(this T selfComponent, float y) where T : Component
		{
			mLocalPos = selfComponent.transform.localPosition;
			mLocalPos.y = y;
			selfComponent.transform.localPosition = mLocalPos;
			return selfComponent;
		}

		public static T LocalPositionZ<T>(this T selfComponent, float z) where T : Component
		{
			mLocalPos = selfComponent.transform.localPosition;
			mLocalPos.z = z;
			selfComponent.transform.localPosition = mLocalPos;
			return selfComponent;
		}

		public static T PositionX<T>(this T selfComponent, float x) where T : Component
		{
			var pos = selfComponent.transform.position;
			pos.x = x;
			selfComponent.transform.position = pos;
			return selfComponent;
		}

		public static T PositionY<T>(this T selfComponent, float y) where T : Component
		{
			var pos = selfComponent.transform.position;
			pos.y = y;
			selfComponent.transform.position = pos;
			return selfComponent;
		}

		public static T PositionZ<T>(this T selfComponent, float z) where T : Component
		{
			var pos = selfComponent.transform.position;
			pos.z = z;
			selfComponent.transform.position = pos;
			return selfComponent;
		}

		public static T LocalScale<T>(this T selfComponent, float xyz) where T : Component
		{
			selfComponent.transform.localScale = Vector3.one * xyz;
			return selfComponent;
		}

		public static T LocalScale<T>(this T selfComponent, float x, float y, float z) where T : Component
		{
			mLocalScale = selfComponent.transform.localScale;
			mLocalScale.x = x;
			mLocalScale.y = y;
			mLocalScale.z = z;
			selfComponent.transform.localScale = mLocalScale;
			return selfComponent;
		}

		public static T LocalScale<T>(this T selfComponent, float x, float y) where T : Component
		{
			mLocalScale = selfComponent.transform.localScale;
			mLocalScale.x = x;
			mLocalScale.y = y;
			selfComponent.transform.localScale = mLocalScale;
			return selfComponent;
		}

		public static T LocalScaleX<T>(this T selfComponent, float x) where T : Component
		{
			mLocalScale = selfComponent.transform.localScale;
			mLocalScale.x = x;
			selfComponent.transform.localScale = mLocalScale;
			return selfComponent;
		}

		public static T LocalScaleY<T>(this T selfComponent, float y) where T : Component
		{
			mLocalScale = selfComponent.transform.localScale;
			mLocalScale.y = y;
			selfComponent.transform.localScale = mLocalScale;
			return selfComponent;
		}

		public static T LocalScaleIdentity<T>(this T selfComponent) where T : Component
		{
			selfComponent.transform.localScale = Vector3.one;
			return selfComponent;
		}

		public static Transform FindChildByPath(this Transform selfTrans, string path)
		{
			return selfTrans.Find(path.Replace(".", "/"));
		}

		public static Transform SeekTrans(this Transform selfTransform, string uniqueName)
		{
			var childTrans = selfTransform.Find(uniqueName);

			if (null != childTrans)
				return childTrans;

			foreach (Transform trans in selfTransform)
			{
				childTrans = trans.SeekTrans(uniqueName);

				if (null != childTrans)
					return childTrans;
			}

			return null;
		}

		public static T Show<T>(this T selfComponent, string tranformPath) where T : Component
		{
			selfComponent.transform.Find(tranformPath).gameObject.Show();
			return selfComponent;
		}

		public static T Hide<T>(this T selfComponent, string tranformPath) where T : Component
		{
			selfComponent.transform.Find(tranformPath).Hide();
			return selfComponent;
		}

		public static T LocalScale<T>(this T selfComponent, Vector3 scale) where T : Component
		{
			selfComponent.transform.localScale = scale;
			return selfComponent;
		}

		public static Quaternion GetLocalRotation<T>(this T selfComponent) where T : Component
		{
			return selfComponent.transform.localRotation;
		}

		public static T LocalRotation<T>(this T selfComponent, Quaternion localRotation) where T : Component
		{
			selfComponent.transform.localRotation = localRotation;
			return selfComponent;
		}

		public static Vector3 GetLocalPosition<T>(this T selfComponent) where T : Component
		{
			return selfComponent.transform.localPosition;
		}

		public static T LocalPosition<T>(this T selfComponent, Vector3 localPos) where T : Component
		{
			selfComponent.transform.localPosition = localPos;
			return selfComponent;
		}

		public static Vector3 GetPosition<T>(this T selfComponent) where T : Component
		{
			return selfComponent.transform.position;
		}

		public static T Position<T>(this T selfComponent, Vector3 position) where T : Component
		{
			selfComponent.transform.position = position;
			return selfComponent;
		}

		public static Quaternion GetRotation<T>(this T selfComponent) where T : Component
		{
			return selfComponent.transform.rotation;
		}

		public static T Rotation<T>(this T selfComponent, Quaternion rotation) where T : Component
		{
			selfComponent.transform.rotation = rotation;
			return selfComponent;
		}

		public static void CopyDataFromTrans(this Transform selfTrans, Transform fromTrans)
		{
			selfTrans.SetParent(fromTrans.parent);
			selfTrans.localPosition = fromTrans.localPosition;
			selfTrans.localRotation = fromTrans.localRotation;
			selfTrans.localScale = fromTrans.localScale;
		}
	}
}