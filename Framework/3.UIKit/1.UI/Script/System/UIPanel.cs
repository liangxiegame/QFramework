/****************************************************************************
 * Copyright (c) 2017 xiaojun
 * Copyright (c) 2017 liangxie
 * Copyright (c) 2017 imagicbell
 * Copyright (c) 2018.5 ~ 2018.8 liangxie
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



namespace QFramework
{
	using System;
	using UnityEngine;
	using System.Collections.Generic;

	/// <summary>
	/// 每个UIbehaviour对应的Data
	/// </summary>
	public interface IUIData
	{
	}

	public class UIPanelData : IUIData
	{
		protected UIPanel mPanel;
	}

	[Serializable]
	public class SubPanelInfo
	{
		public string  PanelName;
		public UILevel Level;
	}
	
	public abstract class UIPanel : QMonoBehaviour, IPanel
	{
		[SerializeField] private List<SubPanelInfo> mSubPanelInfos = new List<SubPanelInfo>();

		public Transform Transform
		{
			get { return transform; }
		}

		UIKitConfig.IPanelLoader IPanel.Loader { get; set; }

		public PanelInfo PanelInfo
		{
			get { return Info; }
			set { Info = value; }
		}

		public PanelInfo Info { get; set; }

		public PanelState State { get; set; }

		protected IUIData mUIData;

		public override IManager Manager
		{
			get { return UIManager.Instance; }
		}

		protected override void OnBeforeDestroy()
		{
			ClearUIComponents();
		}

		protected virtual void ClearUIComponents()
		{
		}

		public void Init(IUIData uiData = null)
		{
			mUIData = uiData;
			OnInit(uiData);
			InitUI(uiData);
			RegisterUIEvent();

			mSubPanelInfos.ForEach(subPanelInfo => UIKit.OpenPanel(subPanelInfo.PanelName, subPanelInfo.Level));

		}

		public void Open(IUIData uiData = null)
		{
			State = PanelState.Opening;
			OnOpen(uiData);
		}

		public override void Hide()
		{
			State = PanelState.Hide;
			base.Hide();
		}


		protected virtual void OnInit(IUIData uiData = null)
		{

		}

		protected virtual void OnOpen(IUIData uiData = null)
		{

		}

		/// <summary>
		/// avoid override in child class
		/// </summary>
		protected sealed override void OnDestroy()
		{
			base.OnDestroy();
		}

		/// <summary>
		/// 关闭,不允许子类调用
		/// </summary>
		void IPanel.Close(bool destroyed)
		{
			PanelInfo.UIData = mUIData;
			mOnClosed.InvokeGracefully();
			mOnClosed = null;
			Hide();
			State = PanelState.Closed;
			OnClose();

			if (destroyed)
			{
				Destroy(gameObject);
			}

			this.As<IPanel>().Loader.Unload();
			this.As<IPanel>().Loader = null;
			
			mUIData = null;

			mSubPanelInfos.ForEach(subPanelInfo => UIKit.ClosePanel(subPanelInfo.PanelName));
			mSubPanelInfos.Clear();
		}

		protected void CloseSelf()
		{
			UIKit.ClosePanel(name);
		}

		protected void Back()
		{
			UIKit.Back(name);
		}

		/// <summary>
		/// 必须使用这个
		/// </summary>
		protected abstract void OnClose();

		private Action mOnClosed;

		public void OnClosed(Action onPanelClosed)
		{
			mOnClosed = onPanelClosed;
		}


		#region 不建议啊使用

		[Obsolete("deprecated")]
		protected virtual void InitUI(IUIData uiData = null)
		{
		}

		[Obsolete("deprecated")]
		protected virtual void RegisterUIEvent()
		{
		}

		#endregion
	}
}