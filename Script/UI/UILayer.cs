using UnityEngine;
using System.Collections;

namespace QFramework.UI {

	/// <summary>
	/// UI,为UI的基础
	/// </summary>
	public abstract class UILayer : MonoBehaviour {


		public void Enter(object uiData)
		{
			gameObject.SetActive (false);
			OnEnter (uiData);
		}

		/// <summary>
		/// 资源加载之后用
		/// </summary>
		protected virtual void OnEnter(object uiData)
		{
			Debug.LogWarning ("On Enter");
		}


		public void Show()
		{
			OnShow ();
		}

		/// <summary>
		/// 显示时候用,或者,Active为True
		/// </summary>
		protected virtual void OnShow()
		{
			gameObject.SetActive (true);
			Debug.LogWarning ("On Show");

		}


		public void Hide()
		{
			OnHide ();
		}

		/// <summary>
		/// 隐藏时候调用,即将删除 或者,Active为False
		/// </summary>
		protected virtual void OnHide()
		{
			gameObject.SetActive (false);
			Debug.LogWarning ("On Hide");
		}

		public void Exit()
		{
			OnExit ();
		}

		/// <summary>
		/// 删除时候调用
		/// </summary>
		protected virtual void OnExit()
		{
			Debug.LogWarning ("On Exit");
		}



	}

}