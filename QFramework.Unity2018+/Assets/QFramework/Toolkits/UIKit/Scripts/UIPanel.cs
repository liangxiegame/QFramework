/****************************************************************************
 * Copyright (c) 2016 ~ 2025 liangxiegame UNDER MIT LINCENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

namespace QFramework
{
	using UnityEngine;

	/// <summary>
	/// 每个 UIPanel 对应的Data
	/// </summary>
	public interface IUIData
	{
	}

	public class UIPanelData : IUIData
	{
	}
	
	public abstract class UIPanel :MonoBehaviour,IPanel
	{
		public virtual void Show()
		{
			gameObject.SetActive (true);

			OnShow ();
		}

		protected virtual void OnShow() {}

		public virtual void Hide()
		{
			State = PanelState.Hide;

			OnHide ();

			gameObject.SetActive (false);
		}

		protected virtual void OnHide() {}
		
		protected virtual void OnDestroy()
		{			
			if (Application.isPlaying) 
			{
				OnBeforeDestroy();
			}
		}

		protected virtual void OnBeforeDestroy()
		{
			ClearUIComponents();
		}
		public Transform Transform => transform;

		IPanelLoader IPanel.Loader { get; set; }

		public PanelInfo Info { get; set; }

		public PanelState State { get; set; }

		protected IUIData mUIData;

		

		protected virtual void ClearUIComponents()
		{
		}

		public void Init(IUIData uiData = null)
		{
			mUIData = uiData;
			OnInit(uiData);
		}

		public void Open(IUIData uiData = null)
		{
			State = PanelState.Opening;
			OnOpen(uiData);
		}




		protected virtual void OnInit(IUIData uiData = null)
		{

		}

		protected virtual void OnOpen(IUIData uiData = null)
		{

		}
		

		/// <summary>
		/// 关闭,不允许子类调用
		/// </summary>
		void IPanel.Close(bool destroyed)
		{
			Info.UIData = mUIData;
			mOnClosed?.Invoke();
			mOnClosed = null;
			Hide();
			State = PanelState.Closed;
			OnClose();

			if (destroyed)
			{
				Destroy(gameObject);
			}

			var panelInterface = this as IPanel;
			panelInterface.Loader.Unload();
			UIKit.Config.PanelLoaderPool.RecycleLoader(panelInterface.Loader);
			panelInterface.Loader = null;

			mUIData = null;
		}

		protected void CloseSelf()
		{
			UIKit.ClosePanel(this);
		}

		protected void Back()
		{
			UIKit.Back(name);
		}

		/// <summary>
		/// 必须使用这个
		/// </summary>
		protected abstract void OnClose();

		private System.Action mOnClosed;

		public void OnClosed(System.Action onPanelClosed)
		{
			mOnClosed = onPanelClosed;
		}
	}
}