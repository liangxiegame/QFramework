/****************************************************************************
 * Copyright (c) 2017 xiaojun
 * Copyright (c) 2017 imagicbell
 * Copyright (c) 2018.5 ~ 8  liangxie
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

using QF;
using QF.Extensions;

namespace QFramework
{
	using UnityEngine;
	using System.Collections.Generic;
	using UnityEngine.UI;
	using System;

#if SLUA_SUPPORT
	using SLua;
#endif
	public enum UILevel
	{
		AlwayBottom = -3, //如果不想区分太复杂那最底层的UI请使用这个
		Bg = -2, //背景层UI
		AnimationUnderPage = -1, //动画层
		Common = 0, //普通层UI
		AnimationOnPage = 1, // 动画层
		PopUI = 2, //弹出层UI
		Guide = 3, //新手引导层
		Const = 4, //持续存在层UI
		Toast = 5, //对话框层UI
		Forward = 6, //最高UI层用来放置UI特效和模型
		AlwayTop = 7, //如果不想区分太复杂那最上层的UI请使用这个
	}

#if SLUA_SUPPORT
	[CustomLuaClass]
#endif
	//// <summary>
	/// <inheritdoc />
	/// <![CDATA[The 'member' start tag on line 2 position 2 does not match the end tag of 'summary'. Line 3, position 3.]]>
	[MonoSingletonPath("UIRoot")]
	public class UIManager : QMgrBehaviour, ISingleton
	{
		private Dictionary<string, IPanel> mAllUI = new Dictionary<string, IPanel>();

        [SerializeField] private Transform mBgTrans;
		[SerializeField] private Transform mAnimationUnderTrans;
		[SerializeField] private Transform mCommonTrans;
		[SerializeField] private Transform mAnimationOnTrans;
		[SerializeField] private Transform mPopUITrans;
		[SerializeField] private Transform mConstTrans;
		[SerializeField] private Transform mToastTrans;
		[SerializeField] private Transform mForwardTrans;
		
		[SerializeField] private Camera mUICamera;
		[SerializeField] private Canvas mCanvas;
		[SerializeField] private CanvasScaler mCanvasScaler;
		[SerializeField] private GraphicRaycaster mGraphicRaycaster;

		public Stack<UIPanelInfo> mUIStack = new Stack<UIPanelInfo>(); 

		void ISingleton.OnSingletonInit()
		{
		}

		private static UIManager mInstance;

		public static UIManager Instance
		{
			get
			{
				if (null == mInstance)
				{
					mInstance = FindObjectOfType<UIManager>();
				}

				if (null == mInstance)
				{
					Instantiate(Resources.Load<GameObject>("UIRoot"));
					mInstance = MonoSingletonProperty<UIManager>.Instance;
					mInstance.name = "UIRoot";
					DontDestroyOnLoad(mInstance);
				}

				return mInstance;
			}
		}

		public Canvas RootCanvas
		{
			get { return mCanvas; }
		}

		public Camera UICamera
		{
			get { return mUICamera; }
		}

		public void SetResolution(int width, int height)
		{
			mCanvasScaler.referenceResolution = new UnityEngine.Vector2(width, height);
		}

		public void SetMatchOnWidthOrHeight(float heightPercent)
		{
			mCanvasScaler.matchWidthOrHeight = heightPercent;
		}

		public IPanel OpenUI(string uiBehaviourName, UILevel canvasLevel, IUIData uiData = null,
			string assetBundleName = null)
		{
			IPanel retPanel = null;

			if (!mAllUI.TryGetValue(uiBehaviourName, out retPanel))
			{
				retPanel = CreateUI(uiBehaviourName, canvasLevel, uiData, assetBundleName);
			}

			retPanel.Open(uiData);
			retPanel.Show();
			return retPanel;
		}

		/// <summary>
		/// 显示UIBehaiviour
		/// </summary>
		/// <param name="uiBehaviourName"></param>
		public void ShowUI(string uiBehaviourName)
		{
			IPanel iuiPanel = null;
			if (mAllUI.TryGetValue(uiBehaviourName, out iuiPanel))
			{
				iuiPanel.Show();
			}
		}

		/// <summary>
		/// 隐藏UI
		/// </summary>
		/// <param name="uiBehaviourName"></param>
		public void HideUI(string uiBehaviourName)
		{
			IPanel iuiPanel = null;
			if (mAllUI.TryGetValue(uiBehaviourName, out iuiPanel))
			{
				iuiPanel.Hide();
			}
		}

		/// <summary>
		/// 删除所有UI层
		/// </summary>
		public void CloseAllUI()
		{
			foreach (var layer in mAllUI)
			{
				Destroy(layer.Value.Transform.gameObject);
			}

			mAllUI.Clear();
		}

		/// <summary>
		/// 隐藏所有 UI
		/// </summary>
		public void HideAllUI()
		{
			mAllUI.ForEach(keyValuePair => keyValuePair.Value.Hide());
		}

		/// <summary>
		/// 关闭并卸载UI
		/// </summary>
		/// <param name="behaviourName"></param>
		public void CloseUI(string behaviourName)
		{
			IPanel behaviour = null;

			mAllUI.TryGetValue(behaviourName, out behaviour);

			if ((behaviour as UIPanel))
			{
				behaviour.Close();
				mAllUI.Remove(behaviourName);
			}
		}
		
		public void Push<T>() where T : UIPanel
		{
			Push(GetUI<T>());
		}

		public void Push(IPanel view)
		{
			if (view != null)
			{
				mUIStack.Push(view.PanelInfo);
				view.Close();
				mAllUI.Remove(view.Transform.name);
			}
		}

		public void Back(string currentPanelName)
		{
			var previousPanelInfo = mUIStack.Pop();
			CloseUI(currentPanelName);
			OpenUI(previousPanelInfo.PanelName, previousPanelInfo.Level, previousPanelInfo.UIData,
				previousPanelInfo.AssetBundleName);
		}
		

		/// <summary>
		/// 获取UIBehaviour
		/// </summary>
		/// <param name="uiBehaviourName"></param>
		/// <returns></returns>
		public UIPanel GetUI(string uiBehaviourName)
		{
			IPanel retIuiPanel = null;
			
			if (mAllUI.TryGetValue(uiBehaviourName, out retIuiPanel))
			{
				return retIuiPanel as UIPanel;
			}

			return null;
		}
		
		public override int ManagerId
		{
			get { return QMgrID.UI; }
		}

		/// <summary>
		/// 命名空间对应名字的缓存
		/// </summary>
		private Dictionary<string, string> mFullname4UIBehaviourName = new Dictionary<string, string>();

		private string GetUIBehaviourName<T>()
		{
			var fullBehaviourName = typeof(T).ToString();
			string retValue = null;

			if (mFullname4UIBehaviourName.TryGetValue(fullBehaviourName, out retValue))
			{	
			}
			else
			{
				var nameSplits = fullBehaviourName.Split('.');
				retValue = nameSplits[nameSplits.Length - 1];
				mFullname4UIBehaviourName.Add(fullBehaviourName, retValue);
			}

			return retValue;
		}

		public IPanel CreateUI(string uiBehaviourName, UILevel level = UILevel.Common, IUIData uiData = null,
			string assetBundleName = null)
		{
			IPanel ui;
			
			if (mAllUI.TryGetValue(uiBehaviourName, out ui))
			{
				return ui;
			}

			ui = UIPanel.Load(uiBehaviourName, assetBundleName);

			switch (level)
			{
				case UILevel.Bg:
					ui.Transform.SetParent(mBgTrans);
					break;
				case UILevel.AnimationUnderPage:
					ui.Transform.SetParent(mAnimationUnderTrans);
					break;
				case UILevel.Common:
					ui.Transform.SetParent(mCommonTrans);
					break;
				case UILevel.AnimationOnPage:
					ui.Transform.SetParent(mAnimationOnTrans);
					break;
				case UILevel.PopUI:
					ui.Transform.SetParent(mPopUITrans);
					break;
				case UILevel.Const:
					ui.Transform.SetParent(mConstTrans);
					break;
				case UILevel.Toast:
					ui.Transform.SetParent(mToastTrans);
					break;
				case UILevel.Forward:
					ui.Transform.SetParent(mForwardTrans);
					break;
			}

			var uiGoRectTrans = ui.Transform as RectTransform;

			uiGoRectTrans.offsetMin = Vector2.zero;
			uiGoRectTrans.offsetMax = Vector2.zero;
			uiGoRectTrans.anchoredPosition3D = Vector3.zero;
			uiGoRectTrans.anchorMin = Vector2.zero;
			uiGoRectTrans.anchorMax = Vector2.one;

			ui.Transform.LocalScaleIdentity();
			ui.Transform.gameObject.name = uiBehaviourName;

			ui.PanelInfo = new UIPanelInfo
				{AssetBundleName = assetBundleName, Level = level, PanelName = uiBehaviourName};

			mAllUI.Add(uiBehaviourName, ui);

			ui.Init(uiData);

			return ui;
		}

		#region UnityCSharp Generic Support

		/// <summary>
		/// Create&ShowUI
		/// </summary>
		public T OpenUI<T>(UILevel canvasLevel = UILevel.Common, IUIData uiData = null, string assetBundleName = null,
			string prefabName = null) where T : UIPanel
		{
			return OpenUI(prefabName ?? GetUIBehaviourName<T>(), canvasLevel, uiData, assetBundleName) as T;
		}
		
		public void ShowUI<T>() where T : UIPanel
		{
			ShowUI(GetUIBehaviourName<T>());
		}

		public void HideUI<T>() where T : UIPanel
		{
			HideUI(GetUIBehaviourName<T>());
		}

		public void CloseUI<T>() where T : UIPanel
		{
			CloseUI(GetUIBehaviourName<T>());
		}

		public T GetUI<T>() where T : UIPanel
		{
			return GetUI(GetUIBehaviourName<T>()) as T;
		}

		#endregion
	}

	public static class UIMgr
	{
		public static void Push<T>() where T : UIPanel
		{
			UIManager.Instance.Push<T>();
		}

		public static void Push(UIPanel view)
		{
			UIManager.Instance.Push(view);
		}
		
		public static Camera Camera
		{
			get { return UIManager.Instance.UICamera; }
		}
		
		public static void SetResolution(int width, int height, float matchOnWidthOrHeight)
		{
			UIManager.Instance.SetResolution(width, height);
			UIManager.Instance.SetMatchOnWidthOrHeight(matchOnWidthOrHeight);
		}

		#region 高频率调用的 api 只能在 Mono 层使用

		internal static T OpenPanel<T>(UILevel canvasLevel = UILevel.Common, IUIData uiData = null,
			string assetBundleName = null,
			string prefabName = null) where T : UIPanel
		{
			return UIManager.Instance.OpenUI<T>(canvasLevel, uiData, assetBundleName, prefabName);
		}
		
		internal static T OpenPanel<T>(IUIData uiData, string assetBundleName = null,
			string prefabName = null) where T : UIPanel
		{
			return UIManager.Instance.OpenUI<T>(UILevel.Common, uiData, assetBundleName,prefabName);
		}

		internal static void ClosePanel<T>() where T : UIPanel
		{
			UIManager.Instance.CloseUI<T>();
		}

		public static void CloseAllPanel()
        {
            UIManager.Instance.CloseAllUI();
        }

        public static void HideAllPanel()
        {
	        UIManager.Instance.HideAllUI();
        }

        internal static T GetPanel<T>() where T : UIPanel
		{
			return UIManager.Instance.GetUI<T>();
		}

		#endregion

		#region 给脚本层用的 api

		public static UIPanel GetPanel(string panelName)
		{
			return UIManager.Instance.GetUI(panelName);
		}

		public static UIPanel OpenPanel(string panelName, UILevel level = UILevel.Common, string assetBundleName = null)
		{
			return UIManager.Instance.OpenUI(panelName, level, null, assetBundleName) as UIPanel;
		}

		public static UIPanel OpenPanel(string panelName)
		{
			return UIManager.Instance.OpenUI(panelName, UILevel.Common, null, null) as UIPanel;
		}
		
		public static void ClosePanel(string panelName)
		{
			UIManager.Instance.CloseUI(panelName);
		}

		#endregion
	}
	
	[Obsolete("弃用啦")]
	public class QUIManager : UIManager
	{
			
	}
}