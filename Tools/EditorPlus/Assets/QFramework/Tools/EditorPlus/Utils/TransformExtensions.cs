using System;
using System.Text;
using UnityEngine;

namespace QFramework
{
	public static class TransformExtensions
	{
		#region 遍历或查找

		public static void ActionRecursionInterruptible(this Transform tfParent, Func<Transform, bool> predicate)
		{
			if (predicate(tfParent))
			{
				foreach (Transform tfChild in tfParent) { tfChild.ActionRecursionInterruptible(predicate); }
			}
		}

		public static void ActionRecursion(this Transform tfParent, Action<Transform> action)
		{
			action(tfParent);
			foreach (Transform tfChild in tfParent) { tfChild.ActionRecursion(action); }
		}

		public static Transform FindChildRecursionExt(this Transform tfParent, string name,
			StringComparison stringComparison)
		{
			if (tfParent.name.Equals(name, stringComparison))
			{
				if (Application.isEditor) { Debug.Log("FindChildRecursion: Hit " + tfParent.name); }

				return tfParent;
			}

			foreach (Transform tfChild in tfParent)
			{
				Transform tfFinal = tfChild.FindChildRecursionExt(name, stringComparison);
				if (tfFinal) { return tfFinal; }
			}

			return null;
		}

		public static Transform FindChildRecursionExt(this Transform tfParent, string name)
		{
			if (tfParent.name.Equals(name))
			{
				if (Application.isEditor) { Debug.Log("FindChildRecursion: Hit " + tfParent.name); }

				return tfParent;
			}

			foreach (Transform tfChild in tfParent)
			{
				Transform tfFinal = tfChild.FindChildRecursionExt(name);
				if (tfFinal) { return tfFinal; }
			}

			return null;
		}

		public static Transform FindChildRecursionExt(this Transform tfParent, Func<Transform, bool> predicate)
		{
			if (predicate(tfParent))
			{
				if (Application.isEditor) { Debug.Log("FindChildRecursion: Hit " + tfParent.name); }

				return tfParent;
			}

			foreach (Transform tfChild in tfParent)
			{
				Transform tfFinal = null;
				tfFinal = tfChild.FindChildRecursionExt(predicate);
				if (tfFinal) { return tfFinal; }
			}

			return null;
		}

		public static Transform FindParentRecursionExt(this Transform tfParent, string name)
		{
			if (tfParent.name.Equals(name))
			{
				if (Application.isEditor) { Debug.Log("FindParentRecursion: Hit " + tfParent.name); }

				return tfParent;
			}

			if (tfParent.parent)
			{
				Transform tfFinal = tfParent.parent.FindParentRecursionExt(name);
				if (tfFinal) { return tfFinal; }
			}

			return null;
		}

		public static Transform FindParentRecursionExt(this Transform tfParent, Func<Transform, bool> predicate)
		{
			if (predicate(tfParent))
			{
				if (Application.isEditor) { Debug.Log("FindParentRecursion: Hit " + tfParent.name); }

				return tfParent;
			}

			if (tfParent.parent)
			{
				Transform tfFinal = null;
				tfFinal = tfParent.parent.FindParentRecursionExt(predicate);
				if (tfFinal) { return tfFinal; }
			}

			return null;
		}

		public static void FindTransformByKeyWord(MonoBehaviour mono, ref Transform tfAnchor, params string[] keyWord)
		{
			tfAnchor = mono.transform.FindChildRecursionExt(t =>
			{
				for (int i = 0; i < keyWord.Length; i++)
				{
					string s = keyWord[i];
					if (t.name.Contains(s)) { return true; }
				}

				return false;
			});
		}

		public static string GetPathExt(this Transform transform)
		{
			StringBuilder sb = new StringBuilder();
			Transform t = transform;
			while (true)
			{
				sb.Insert(0, t.name);
				t = t.parent;
				if (t) { sb.Insert(0, "/"); }
				else { return sb.ToString(); }
			}
		}

		#endregion
	}
}