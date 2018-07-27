/****************************************************************************
 * Copyright (c) 2017 magicbel
 * Copyright (c) 2018.5 liangxie
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
	using UnityEngine;
	using System.Collections.Generic;
	using UnityEngine.UI;
	using System;

#if SLUA_SUPPORT
	using SLua;
#endif
	public enum UILevel
	{
		AlwayBottom        = -3, //如果不想区分太复杂那最底层的UI请使用这个
		Bg                 = -2, //背景层UI
		AnimationUnderPage = -1, //动画层
		Common             = 0, //普通层UI
		AnimationOnPage    = 1, // 动画层
		PopUI              = 2, //弹出层UI
		Guide              = 3, //新手引导层
		Const              = 4, //持续存在层UI
		Toast              = 5, //对话框层UI
		Forward            = 6, //最高UI层用来放置UI特效和模型
		AlwayTop           = 7, //如果不想区分太复杂那最上层的UI请使用这个
	}

#if SLUA_SUPPORT
	[CustomLuaClass]
#endif
	//// <summary>
	/// <inheritdoc />
	/// <![CDATA[The 'member' start tag on line 2 position 2 does not match the end tag of 'summary'. Line 3, position 3.]]>
	public class QUIManager : QMgrBehaviour, ISingleton
	{
		private Dictionary<string, IUIBehaviour> mAllUI = new Dictionary<string, IUIBehaviour>();
		private QLayerLogic mLayerLogic;
		private QUIPanelStack mUIPanelStack;

        private bool mReSetLayerIndexDirty = false;

		[SerializeField] private Camera mUICamera;
		[SerializeField] private Canvas mCanvas;
		[SerializeField] private CanvasScaler mCanvasScaler;
		[SerializeField] private GraphicRaycaster mGraphicRaycaster;

		private void Awake()
		{
            mLayerLogic = GetComponent<QLayerLogic>();
            mLayerLogic.InitLayer(mCanvas.transform);
            mUIPanelStack = QUIPanelStack.Instance;
		}

		public void OnSingletonInit()
		{
			Log.I("QUIManager Init");
        }

		private static QUIManager mInstance;

		public static QUIManager Instance
		{
			get
			{
				if (null == mInstance)
				{
					mInstance = FindObjectOfType<QUIManager>();
				}

				if (null == mInstance)
				{
					Instantiate(Resources.Load<GameObject>("UIRoot"));
					mInstance = MonoSingletonProperty<QUIManager>.Instance;
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

		// TODO: 全局唯一事件管理
		public GraphicRaycaster GlobalGraphicRaycaster
		{
			get { return mGraphicRaycaster; }
		}

		public void SetResolution(int width, int height)
		{
			mCanvasScaler.referenceResolution = new UnityEngine.Vector2(width, height);
		}

		public void SetMatchOnWidthOrHeight(float heightPercent)
		{
			mCanvasScaler.matchWidthOrHeight = heightPercent;
		}

        public void ReSetLayerIndexDirty() {
            mReSetLayerIndexDirty = true;
        }

        //层级排序
        public void ReSortUILayer() {
            mReSetLayerIndexDirty = false;
            mLayerLogic.SortAllUIPanel();
        }
		
		public IUIBehaviour OpenUI(string uiBehaviourName, UILevel canvasLevel,string assetBundleName)
		{
			if (!mAllUI.ContainsKey(uiBehaviourName))
			{
				CreateUI(uiBehaviourName, canvasLevel,null,assetBundleName);
			}

			mAllUI[uiBehaviourName].Show();
			return mAllUI[uiBehaviourName];
		}

        public IUIBehaviour OpenUI(string uiBehaviourName, UILevel canvasLevel)
		{
			if (!mAllUI.ContainsKey(uiBehaviourName))
			{
				CreateUI(uiBehaviourName, canvasLevel);
			}

			mAllUI[uiBehaviourName].Show();
            ReSetLayerIndexDirty();
            return mAllUI[uiBehaviourName];
		}


		/// <summary>
		/// 创建UIPanel
		/// </summary>
		/// <param name="uiBehaviourName"></param>
		/// <param name="uiLevel"></param>
		/// <param name="initData"></param>
		/// <returns></returns>
		public GameObject CreateUIObj(string uiBehaviourName, UILevel uiLevel,string assetBundleName = null)
		{
			IUIBehaviour ui;
			if (mAllUI.TryGetValue(uiBehaviourName, out ui))
			{
				Log.W("{0}: already exist", uiBehaviourName);
				// 直接返回,不要再调一次Init(),Init()应该只能调用一次
				return ui.Transform.gameObject;
			}

			ui = UIPanel.Load(uiBehaviourName,assetBundleName);

			mLayerLogic.SetLayer((int)uiLevel,ui as UIPanel);
            mUIPanelStack.OnCreatUI((int)uiLevel,ui,uiBehaviourName);
            ReSetLayerIndexDirty();
            ui.Transform.gameObject.name = uiBehaviourName;

			return ui.Transform.gameObject;
		}

		/// <summary>
		/// 显示UIBehaiviour
		/// </summary>
		/// <param name="uiBehaviourName"></param>
		public void ShowUI(string uiBehaviourName)
		{
			IUIBehaviour uiBehaviour = null;
			if (mAllUI.TryGetValue(uiBehaviourName, out uiBehaviour))
			{
				uiBehaviour.Show();
                mLayerLogic.OnUIPanelShow(uiBehaviour as UIPanel);
                ReSetLayerIndexDirty();
            }
		}

		/// <summary>
		/// 隐藏UI
		/// </summary>
		/// <param name="uiBehaviourName"></param>
		public void HideUI(string uiBehaviourName)
		{
			IUIBehaviour uiBehaviour = null;
			if (mAllUI.TryGetValue(uiBehaviourName, out uiBehaviour))
			{
				uiBehaviour.Hide();
                mLayerLogic.OnUIPanelHide(uiBehaviour as UIPanel);
                ReSetLayerIndexDirty();
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
            mUIPanelStack.ClearStack();
            mLayerLogic.ClearUILayerMap();
        }

		/// <summary>
		/// 关闭并卸载UI
		/// </summary>
		/// <param name="behaviourName"></param>
		public void CloseUI(string behaviourName)
		{
			IUIBehaviour behaviour = null;

			mAllUI.TryGetValue(behaviourName, out behaviour);

			if (null != behaviour)
			{
				behaviour.Close();
				mAllUI.Remove(behaviourName);
                mUIPanelStack.RemoveUI(behaviourName,behaviour);
                mLayerLogic.OnUIPanelClose(behaviour as UIPanel);
                ReSetLayerIndexDirty();
            }
		}

		/// <summary>
		/// 获取UIBehaviour
		/// </summary>
		/// <param name="uiBehaviourName"></param>
		/// <returns></returns>
		public UIPanel GetUI(string uiBehaviourName)
		{
			IUIBehaviour retUiBehaviour = null;
			if (mAllUI.TryGetValue(uiBehaviourName, out retUiBehaviour))
			{
				return retUiBehaviour as UIPanel;
			}
			return null;
		}

		protected override void SetupMgrId()
		{
			mMgrId = QMgrID.UI;
		}

		/// <summary>
		/// 命名空间对应名字的缓存
		/// </summary>
		private Dictionary<string, string> mFullname4UIBehaviourName = new Dictionary<string, string>();

		private string GetUIBehaviourName<T>()
		{
			string fullBehaviourName = typeof(T).ToString();
			string retValue = null;

			if (mFullname4UIBehaviourName.ContainsKey(fullBehaviourName))
			{
				retValue = mFullname4UIBehaviourName[fullBehaviourName];
			}
			else
			{
				string[] nameSplits = fullBehaviourName.Split(new char[] {'.'});
				retValue = nameSplits[nameSplits.Length - 1];
				mFullname4UIBehaviourName.Add(fullBehaviourName, retValue);
			}

			return retValue;
		}

		public IUIBehaviour CreateUI(string uiBehaviourName, UILevel level = UILevel.Common, IUIData uiData = null,string assetBundleName = null)
		{
			IUIBehaviour ui;
			if (mAllUI.TryGetValue(uiBehaviourName, out ui))
			{
				return ui;
			}
			var uiObj = CreateUIObj(uiBehaviourName, level, assetBundleName);

			ui = uiObj.GetComponent<IUIBehaviour>();

			mAllUI.Add(uiBehaviourName, ui);

			ui.Init(uiData);

			return ui;
		}

        #region 生命周期 

        public void Update()
        {
            if (mReSetLayerIndexDirty)
                ReSortUILayer();
        }

        private void LateUpdate()
        {
            if (mReSetLayerIndexDirty)
                ReSortUILayer();
        }


        #endregion

        #region UnityCSharp Generic Support

        /// <summary>
        /// Create&ShowUI
        /// </summary>
        public T OpenUI<T>(UILevel canvasLevel = UILevel.Common, IUIData uiData = null,string assetBundleName = null,string prefabName = null) where T : UIPanel
		{
			var behaviourName = prefabName ?? GetUIBehaviourName<T>();

            if (!mAllUI.ContainsKey(behaviourName))
            {
                CreateUI(behaviourName, canvasLevel, uiData, assetBundleName);
            }
            else {
                mLayerLogic.OnUIPanelShow(mAllUI[behaviourName] as UIPanel);
            }
            
            mAllUI[behaviourName].Show();
            ReSetLayerIndexDirty();
            return mAllUI[behaviourName] as T;
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

		#region LuaSupport

		#endregion
	}

	public static class UIMgr
	{
		internal static T OpenPanel<T>(UILevel canvasLevel = UILevel.Common, IUIData uiData = null, string assetBundleName = null,
			string prefabName = null) where T : UIPanel
		{
			return QUIManager.Instance.OpenUI<T>(canvasLevel, uiData, assetBundleName,prefabName);
		}

		internal static UIPanel OpenPanel(string panelName, UILevel canvasLevel = UILevel.Common, IUIData uiData = null,
			string assetBundleName = null)
		{
			return QUIManager.Instance.OpenUI(panelName,canvasLevel,assetBundleName) as UIPanel;
		}

		internal static void ClosePanel<T>() where T : UIPanel
		{
			QUIManager.Instance.CloseUI<T>();
		}

		internal static T GetPanel<T>() where T : UIPanel
		{
			return QUIManager.Instance.GetUI<T>();
		}
		
		public static void SetResolution(int width, int height, float matchOnWidthOrHeight)
		{
			QUIManager.Instance.SetResolution(width, height);
			QUIManager.Instance.SetMatchOnWidthOrHeight(matchOnWidthOrHeight);
		}

		#region 给脚本层用的api
		public static UIPanel GetPanel(string panelName)
		{
			return QUIManager.Instance.GetUI(panelName);
		}

		public static void ClosePanel(string panelName)
		{
			QUIManager.Instance.CloseUI(panelName);
		}
		#endregion
	}
}