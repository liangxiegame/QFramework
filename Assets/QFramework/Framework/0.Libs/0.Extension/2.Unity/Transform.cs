/****************************************************************************
 * Copyright (c) 2017 ~ 2018.5 liangxie
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

using System;

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
		public static void Example()
		{
			var selfScript = new GameObject().AddComponent<MonoBehaviour>();
			var transform = selfScript.transform;
			
			transform
				.Parent(null)
				.LocalIdentity()
				.LocalPositionIdentity()
				.LocalRotationIdentity()
				.LocalScaleIdentity()
				.LocalPosition(Vector3.zero)
				.LocalPosition(0, 0, 0)
				.LocalPosition(0, 0)
				.LocalPositionX(0)
				.LocalPositionY(0)
				.LocalPositionZ(0)
				.LocalRotation(Quaternion.identity)
				.LocalScale(Vector3.one)
				.LocalScaleX(1.0f)
				.LocalScaleY(1.0f)
				.Identity()
				.PositionIdentity()
				.RotationIdentity()
				.Position(Vector3.zero)
				.PositionX(0)
				.PositionY(0)
				.PositionZ(0)
				.Rotation(Quaternion.identity)
				.DestroyAllChild()
				.AsLastSibling()
				.AsFirstSibling()
				.SiblingIndex(0);

			selfScript
				.Parent(null)
				.LocalIdentity()
				.LocalPositionIdentity()
				.LocalRotationIdentity()
				.LocalScaleIdentity()
				.LocalPosition(Vector3.zero)
				.LocalPosition(0, 0, 0)
				.LocalPosition(0, 0)
				.LocalPositionX(0)
				.LocalPositionY(0)
				.LocalPositionZ(0)
				.LocalRotation(Quaternion.identity)
				.LocalScale(Vector3.one)
				.LocalScaleX(1.0f)
				.LocalScaleY(1.0f)
				.Identity()
				.PositionIdentity()
				.RotationIdentity()
				.Position(Vector3.zero)
				.PositionX(0)
				.PositionY(0)
				.PositionZ(0)
				.Rotation(Quaternion.identity)
				.DestroyAllChild()
				.AsLastSibling()
				.AsFirstSibling()
				.SiblingIndex(0);
		}
		
		
		/// <summary>
		/// 缓存的一些变量,免得每次声明
		/// </summary>
		private static Vector3 mLocalPos;
		private static Vector3 mScale;
		private static Vector3 mPos;
		
		#region CETR001 Parent

		public static T Parent<T>(this T selfComponent, Transform parent) where T : Component
		{
			selfComponent.transform.SetParent(parent);
			return selfComponent;
		}

		#endregion

		#region CETR002 LocalIdentity

		public static T LocalIdentity<T>(this T selfComponent) where T : Component
		{
			selfComponent.transform.localPosition = Vector3.zero;
			selfComponent.transform.localRotation = Quaternion.identity;
			selfComponent.transform.localScale = Vector3.one;
			return selfComponent;
		}

		#endregion

		#region CETR003 LocalPosition

		public static T LocalPosition<T>(this T selfComponent, Vector3 localPos) where T : Component
		{
			selfComponent.transform.localPosition = localPos;
			return selfComponent;
		}

		public static Vector3 GetLocalPosition<T>(this T selfComponent) where T : Component
		{
			return selfComponent.transform.localPosition;
		}
		


		public static T LocalPosition<T>(this T selfComponent, float x, float y, float z) where T : Component
		{
			selfComponent.transform.localPosition = new Vector3(x, y, z);
			return selfComponent;
		}
		
		public static T LocalPosition<T>(this T selfComponent, float x, float y) where T : Component
		{
			mLocalPos = selfComponent.transform.localPosition;
			mLocalPos.x = x;
			mLocalPos.y = y;
			selfComponent.transform.localPosition = mLocalPos;
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
		
		
		public static T LocalPositionIdentity<T>(this T selfComponent) where T : Component
		{
			selfComponent.transform.localPosition = Vector3.zero;
			return selfComponent;
		}

		#endregion

		#region CETR004 LocalRotation

		public static Quaternion GetLocalRotation<T>(this T selfComponent) where T : Component
		{
			return selfComponent.transform.localRotation;
		}

		public static T LocalRotation<T>(this T selfComponent, Quaternion localRotation) where T : Component
		{
			selfComponent.transform.localRotation = localRotation;
			return selfComponent;
		}

		public static T LocalRotationIdentity<T>(this T selfComponent) where T : Component
		{
			selfComponent.transform.localRotation = Quaternion.identity;
			return selfComponent;
		}

		#endregion
		
		#region CETR005 LocalScale

		public static T LocalScale<T>(this T selfComponent, Vector3 scale) where T : Component
		{
			selfComponent.transform.localScale = scale;
			return selfComponent;
		}

		public static Vector3 GetLocalScale<T>(this T selfComponent) where T : Component
		{
			return selfComponent.transform.localScale;
		}
		
		public static T LocalScale<T>(this T selfComponent, float xyz) where T : Component
		{
			selfComponent.transform.localScale = Vector3.one * xyz;
			return selfComponent;
		}

		public static T LocalScale<T>(this T selfComponent, float x, float y, float z) where T : Component
		{
			mScale = selfComponent.transform.localScale;
			mScale.x = x;
			mScale.y = y;
			mScale.z = z;
			selfComponent.transform.localScale = mScale;
			return selfComponent;
		}

		public static T LocalScale<T>(this T selfComponent, float x, float y) where T : Component
		{
			mScale = selfComponent.transform.localScale;
			mScale.x = x;
			mScale.y = y;
			selfComponent.transform.localScale = mScale;
			return selfComponent;
		}

		public static T LocalScaleX<T>(this T selfComponent, float x) where T : Component
		{
			mScale = selfComponent.transform.localScale;
			mScale.x = x;
			selfComponent.transform.localScale = mScale;
			return selfComponent;
		}

		public static T LocalScaleY<T>(this T selfComponent, float y) where T : Component
		{
			mScale = selfComponent.transform.localScale;
			mScale.y = y;
			selfComponent.transform.localScale = mScale;
			return selfComponent;
		}
		
		public static T LocalScaleZ<T>(this T selfComponent, float z) where T : Component
		{
			mScale = selfComponent.transform.localScale;
			mScale.z = z;
			selfComponent.transform.localScale = mScale;
			return selfComponent;
		}
		
		public static T LocalScaleIdentity<T>(this T selfComponent) where T : Component
		{
			selfComponent.transform.localScale = Vector3.one;
			return selfComponent;
		}
		
		#endregion

		#region CETR006 Identity

		public static T Identity<T>(this T selfComponent) where T : Component
		{
			selfComponent.transform.position = Vector3.zero;
			selfComponent.transform.rotation = Quaternion.identity;
			selfComponent.transform.localScale = Vector3.one;
			return selfComponent;
		}

		#endregion

		#region CETR007 Position

		public static T Position<T>(this T selfComponent, Vector3 position) where T : Component
		{
			selfComponent.transform.position = position;
			return selfComponent;
		}
		
		public static Vector3 GetPosition<T>(this T selfComponent) where T : Component
		{
			return selfComponent.transform.position;
		}

		public static T Position<T>(this T selfComponent, float x, float y, float z) where T : Component
		{
			selfComponent.transform.position = new Vector3(x, y, z);
			return selfComponent;
		}
		
		public static T Position<T>(this T selfComponent, float x, float y) where T : Component
		{
			mPos = selfComponent.transform.position;
			mPos.x = x;
			mPos.y = y;
			selfComponent.transform.position = mPos;
			return selfComponent;
		}

		public static T PositionIdentity<T>(this T selfComponent) where T : Component
		{
			selfComponent.transform.position = Vector3.zero;
			return selfComponent;
		}
		
		public static T PositionX<T>(this T selfComponent, float x) where T : Component
		{
			mPos = selfComponent.transform.position;
			mPos.x = x;
			selfComponent.transform.position = mPos;
			return selfComponent;
		}
		
		public static T PositionX<T>(this T selfComponent, Func<float,float> xSetter) where T : Component
		{
			mPos = selfComponent.transform.position;
			mPos.x = xSetter(mPos.x);
			selfComponent.transform.position = mPos;
			return selfComponent;
		}

		public static T PositionY<T>(this T selfComponent, float y) where T : Component
		{
			mPos = selfComponent.transform.position;
			mPos.y = y;
			selfComponent.transform.position = mPos;
			return selfComponent;
		}
		
		public static T PositionY<T>(this T selfComponent, Func<float,float> ySetter) where T : Component
		{
			mPos = selfComponent.transform.position;
			mPos.y = ySetter(mPos.y);
			selfComponent.transform.position = mPos;
			return selfComponent;
		}

		public static T PositionZ<T>(this T selfComponent, float z) where T : Component
		{
			mPos = selfComponent.transform.position;
			mPos.z = z;
			selfComponent.transform.position = mPos;
			return selfComponent;
		}

		public static T PositionZ<T>(this T selfComponent, Func<float, float> zSetter) where T : Component
		{
			mPos = selfComponent.transform.position;
			mPos.z = zSetter(mPos.z);
			selfComponent.transform.position = mPos;
			return selfComponent;
		}

		#endregion
		
		#region CETR008 Rotation

		public static T RotationIdentity<T>(this T selfComponent) where T : Component
		{
			selfComponent.transform.rotation = Quaternion.identity;
			return selfComponent;
		}
		
		public static T Rotation<T>(this T selfComponent, Quaternion rotation) where T : Component
		{
			selfComponent.transform.rotation = rotation;
			return selfComponent;
		}

		public static Quaternion GetRotation<T>(this T selfComponent) where T : Component
		{
			return selfComponent.transform.rotation;
		}

		#endregion

		#region CETR009 WorldScale/LossyScale/GlobalScale/Scale
 
		public static Vector3 GetGlobalScale<T>(this T selfComponent) where T : Component
		{
			return selfComponent.transform.lossyScale;
		}
 
		public static Vector3 GetScale<T>(this T selfComponent) where T : Component
		{
			return selfComponent.transform.lossyScale;
		}
		
		public static Vector3 GetWorldScale<T>(this T selfComponent) where T : Component
		{
			return selfComponent.transform.lossyScale;
		}
		
		public static Vector3 GetLossyScale<T>(this T selfComponent) where T : Component
		{
			return selfComponent.transform.lossyScale;
		}
 
		#endregion

		#region CETR010 Destroy All Child
		
		public static T DestroyAllChild<T>(this T selfComponent) where T : Component
		{
			var childCount = selfComponent.transform.childCount;

			for (var i = 0; i < childCount; i++)
			{
				selfComponent.transform.GetChild(i).DestroyGameObjGracefully();
			}

			return selfComponent;
		}

		public static GameObject DestroyAllChild(this GameObject selfGameObj)
		{
			var childCount = selfGameObj.transform.childCount;

			for (var i = 0; i < childCount; i++)
			{
				selfGameObj.transform.GetChild(i).DestroyGameObjGracefully();
			}

			return selfGameObj;
		}

		#endregion

		#region CETR011 Sibling Index

		public static T AsLastSibling<T>(this T selfComponent) where T : Component
		{
			selfComponent.transform.SetAsLastSibling();
			return selfComponent;
		}

		public static T AsFirstSibling<T>(this T selfComponent) where T : Component
		{
			selfComponent.transform.SetAsFirstSibling();
			return selfComponent;
		}

		public static T SiblingIndex<T>(this T selfComponent, int index) where T : Component
		{
			selfComponent.transform.SetSiblingIndex(index);
			return selfComponent;
		}

		#endregion
		
		
		public static Transform FindByPath(this Transform selfTrans, string path)
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

		public static T ShowChildTransByPath<T>(this T selfComponent, string tranformPath) where T : Component
		{
			selfComponent.transform.Find(tranformPath).gameObject.Show();
			return selfComponent;
		}

		public static T HideChildTransByPath<T>(this T selfComponent, string tranformPath) where T : Component
		{
			selfComponent.transform.Find(tranformPath).Hide();
			return selfComponent;
		}

		public static void CopyDataFromTrans(this Transform selfTrans, Transform fromTrans)
		{
			selfTrans.SetParent(fromTrans.parent);
			selfTrans.localPosition = fromTrans.localPosition;
			selfTrans.localRotation = fromTrans.localRotation;
			selfTrans.localScale = fromTrans.localScale;
		}

        public static Transform FindChildRecursion(this Transform tfParent, System.Func<Transform, bool> predicate)
        {
            if (predicate(tfParent))
            {
                Debug.Log("Hit " + tfParent.name);
                return tfParent;
            }

            foreach (Transform tfChild in tfParent)
            {
                Transform tfFinal = null;
                tfFinal = tfChild.FindChildRecursion(predicate);
                if (tfFinal)
                {
                    return tfFinal;
                }
            }

            return null;
        }

        public static string GetPath(this Transform transform)
        {
            var sb = new System.Text.StringBuilder();
            var t = transform;
            while (true)
            {
                sb.Insert(0, t.name);
                t = t.parent;
                if (t)
                {
                    sb.Insert(0, "/");
                }
                else
                {
                    return sb.ToString();
                }
            }
        }
	}
}