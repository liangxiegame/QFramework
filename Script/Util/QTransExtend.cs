using UnityEngine;
using System.Collections;


namespace QFramework {
	/// <summary>
	/// 关于Transform 常用的代码封装起来
	/// </summary>
	public static class QTransExtend  {

		public static void OnClick(this Transform trans,VoidDelegate.WithGo callback)
		{
			QFramework.UI.UGUIEventListener.Get (trans.gameObject);

			var listener = QFramework.UI.UGUIEventListener.CheckAndAddListener (trans.gameObject);
			listener.onClick += callback;
		}




		/// <summary>
		/// 重置操作
		/// </summary>
		/// <param name="trans">Trans.</param>
		public static void Identity(this Transform trans)
		{
			trans.localPosition = Vector3.zero;
			trans.localScale = Vector3.one;
			trans.localRotation = Quaternion.identity;
		}


		/// <summary>
		/// localPosition 操作
		/// </summary>
		/// <returns>The x.</returns>
		/// <param name="trans">Trans.</param>
		public static float PosX(Transform trans)
		{
			return trans.localPosition.x;
		}

		public static float PosY(Transform trans)
		{
			return trans.localPosition.y;
		}

		public static float PosZ(Transform trans)
		{
			return trans.localPosition.z;
		}

		public static void SetPosX(Transform trans,float x)
		{
			var previousPos = trans.localPosition;
			previousPos.x = x;
			trans.localPosition = previousPos;
		}

		public static void SetPosY(Transform trans,float y)
		{
			var previousPos = trans.localPosition;
			previousPos.y = y;
			trans.localPosition = previousPos;
		}

		public static void SetPosXY(Transform trans,float x,float y)
		{
			var previousPos = trans.localPosition;
			previousPos.x = x;
			previousPos.y = y;
			trans.localPosition = previousPos;
		}

		public static void SetPosZ(Transform trans,float z)
		{
			var previousPos = trans.localPosition;
			previousPos.z = z;
			trans.localPosition = previousPos;
		}


		/// <summary>
		/// localScale操作
		/// </summary>
		public static void SetScaleXY(Transform trans,float x,float y)
		{
			var previousScale = trans.localScale;
			previousScale.x = x;
			previousScale.y = y;
			trans.localScale = previousScale;
		}
	}

}
