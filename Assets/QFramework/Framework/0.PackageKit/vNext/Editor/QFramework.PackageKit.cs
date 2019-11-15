using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using QFramework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace QFramework
{
    using Dependencies.PackageKit;
    
    public static class FrameworkMenuItems
    {
        public const string Preferences = "QFramework/Preferences... %e";
        public const string PackageKit  = "QFramework/PackageKit... %#e";

        public const string Feedback = "QFramework/Feedback";
    }

    public static class FrameworkMenuItemsPriorities
    {
        public const int Preferences = 1;

        public const int Feedback = 11;
    }
    
    public interface IPackageKitView 
    {
        IQFrameworkContainer Container { get; set; }
        /// <summary>
        /// 1 after 0
        /// </summary>
        int RenderOrder { get;}
		
        bool Ignore { get; }
		
        bool Enabled { get;}
		
        void Init(IQFrameworkContainer container);

        void OnUpdate();
        void OnGUI();

        void OnDispose();
    }
    
    public class PackageKitWindow : IMGUIEditorWindow
    {
        class LocaleText
        {
            public static string QFrameworkSettings
            {
                get { return Language.IsChinese ? "QFramework 设置" : "QFramework Settings"; }
            }
        }

        [MenuItem(FrameworkMenuItems.Preferences, false, FrameworkMenuItemsPriorities.Preferences)]
        [MenuItem(FrameworkMenuItems.PackageKit, false, FrameworkMenuItemsPriorities.Preferences)]
        private static void Open()
        {
            var packageKitWindow = Create<PackageKitWindow>(true);
            packageKitWindow.titleContent = new GUIContent(LocaleText.QFrameworkSettings);
            packageKitWindow.position = new Rect(100, 100, 690, 800);
            packageKitWindow.Show();
        }

        private const string URL_FEEDBACK = "http://feathub.com/liangxiegame/QFramework";

        [MenuItem(FrameworkMenuItems.Feedback, false, FrameworkMenuItemsPriorities.Feedback)]
        private static void Feedback()
        {
            Application.OpenURL(URL_FEEDBACK);
        }

        public override void OnUpdate()
        {
            mPackageKitViews.ForEach(view=>view.OnUpdate());
        }

        public List<IPackageKitView> mPackageKitViews = null;

        protected override void Init()
        {
            var label = GUI.skin.label;
            PackageApplication.Container = null;
			
            RemoveAllChidren();

            mPackageKitViews = PackageApplication.Container
                .ResolveAll<IPackageKitView>()
                .OrderBy(view => view.RenderOrder)
                .ToList();
			
            PackageApplication.Container.RegisterInstance(this);

        }
		
        public override void OnGUI()
        {
            base.OnGUI();
            mPackageKitViews.ForEach(view=>view.OnGUI());
        }

        public override void OnClose()
        {
            mPackageKitViews.ForEach(view=>view.OnDispose());
			
            RemoveAllChidren();
        }
    }	
    
    public static class PackageApplication
	{
		public static  List<Assembly>                  CachedAssemblies { get; set; }
		private static Dictionary<Type, IEventManager> mEventManagers;

		private static Dictionary<Type, IEventManager> EventManagers
		{
			get { return mEventManagers ?? (mEventManagers = new Dictionary<Type, IEventManager>()); }
			set { mEventManagers = value; }
		}

		private static QFrameworkContainer mContainer;

		public static QFrameworkContainer Container
		{
			get
			{
				if (mContainer != null) return mContainer;
				mContainer = new QFrameworkContainer();
				InitializeContainer(mContainer);
				return mContainer;
			}
			set
			{
				mContainer = value;
				if (mContainer == null)
				{
					IEventManager eventManager;
					EventManagers.TryGetValue(typeof(ISystemResetEvents), out eventManager);
					EventManagers.Clear();
					var events = eventManager as EventManager<ISystemResetEvents>;
					if (events != null)
					{
						events.Signal(_ => _.SystemResetting());
					}
				}
			}
		}

		public static IEnumerable<Type> GetDerivedTypes<T>(bool includeAbstract = false, bool includeBase = true)
		{
			var type = typeof(T);
			if (includeBase)
				yield return type;
			if (includeAbstract)
			{
				foreach (var assembly in CachedAssemblies)
				{
					foreach (var t in assembly
						.GetTypes()
						.Where(x => type.IsAssignableFrom(x)))
					{
						yield return t;
					}
				}
			}
			else
			{
				var items = new List<Type>();
				foreach (var assembly in CachedAssemblies)
				{
					try
					{
						items.AddRange(assembly.GetTypes()
							.Where(x => type.IsAssignableFrom(x) && !x.IsAbstract));
					}
					catch (Exception ex)
					{
						Log.I(ex.Message);
//						InvertApplication.Log(ex.Message);
					}
				}

				foreach (var item in items)
					yield return item;
			}
		}

		public static System.Action ListenFor(Type eventInterface, object listenerObject)
		{
			var listener = listenerObject;

			IEventManager manager;
			if (!EventManagers.TryGetValue(eventInterface, out manager))
			{
				EventManagers.Add(eventInterface,
					manager = (IEventManager) Activator.CreateInstance(
						typeof(EventManager<>).MakeGenericType(eventInterface)));
			}

			var m = manager;


			return m.AddListener(listener);
		}

		private static IPackageKitView[] mPlugins;

		public static IPackageKitView[] Plugins
		{
			get { return mPlugins ?? (mPlugins = Container.ResolveAll<IPackageKitView>().ToArray()); }
			set { mPlugins = value; }
		}

		private static void InitializeContainer(IQFrameworkContainer container)
		{
			mPlugins = null;
			container.RegisterInstance(container);
			var pluginTypes = GetDerivedTypes<IPackageKitView>(false, false).ToArray();
//			// Load all plugins
			foreach (var diagramPlugin in pluginTypes)
			{
//				if (pluginTypes.Any(p => p.BaseType == diagramPlugin)) continue;
				var pluginInstance = Activator.CreateInstance((Type) diagramPlugin) as IPackageKitView;
				if (pluginInstance == null) continue;
				container.RegisterInstance(pluginInstance, diagramPlugin.Name, false);
				container.RegisterInstance(pluginInstance.GetType(), pluginInstance);
				if (pluginInstance.Enabled)
				{
					foreach (var item in diagramPlugin.GetInterfaces())
					{
						ListenFor(item, pluginInstance);
					}
				}
			}

			container.InjectAll();

			foreach (var diagramPlugin in Plugins.OrderBy(p => p.RenderOrder).Where(p => !p.Ignore))
			{
				if (diagramPlugin.Enabled)
				{
					var start = DateTime.Now;
					diagramPlugin.Container = Container;
					diagramPlugin.Init(Container);
				}
			}

			foreach (var diagramPlugin in Plugins.OrderBy(p => p.RenderOrder).Where(p => !p.Ignore))
			{

				if (diagramPlugin.Enabled)
				{
					var start = DateTime.Now;
					container.Inject(diagramPlugin);
//					diagramPlugin.Loaded(Container);
//					diagramPlugin.LoadTime = DateTime.Now.Subtract(start);
				}


			}

			SignalEvent<ISystemResetEvents>(_ => _.SystemRestarted());
		}

		public static void SignalEvent<TEvents>(Action<TEvents> action) where TEvents : class
		{
			IEventManager manager;
			if (!EventManagers.TryGetValue(typeof(TEvents), out manager))
			{
				EventManagers.Add(typeof(TEvents), manager = new EventManager<TEvents>());
			}
			var m = manager as EventManager<TEvents>;
			m.Signal(action);
		}
		
		static PackageApplication()
		{
			CachedAssemblies = new List<Assembly>
			{
				typeof(int).Assembly, typeof(List<>).Assembly
			};

			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (assembly.FullName.StartsWith("QF") || assembly.FullName.StartsWith("Assembly-CSharp-Editor"))
				{
					CachedAssembly(assembly);
				}
			}
		}

		public static void CachedAssembly(Assembly assembly)
		{
			if (CachedAssemblies.Contains(assembly)) return;
			CachedAssemblies.Add(assembly);
		}
	}

	public interface IEventManager
	{
		System.Action AddListener(object listener);
		void Signal(Action<object> obj);
	}

	public class EventManager<T> : IEventManager where T : class
	{
		private List<T> _listeners;

		public List<T> Listeners
		{
			get { return _listeners ?? (_listeners = new List<T>()); }
			set { _listeners = value; }
		}

		public void Signal(Action<object> obj)
		{
			foreach (var item in Listeners)
			{
				var item1 = item;
				obj(item1);
			}
		}

		public void Signal(Action<T> action)
		{
			foreach (var item in Listeners)
			{
				//InvertApplication.Log(typeof(T).Name + " was signaled on " + item.GetType().Name);
				var item1 = item;
				action(item1);
			}
		}

		public System.Action Subscribe(T listener)
		{
			if (!Listeners.Contains(listener))
				Listeners.Add(listener);

			return () => { Unsubscribe(listener); };
		}

		private void Unsubscribe(T listener)
		{
			Listeners.Remove(listener);
		}

		public System.Action AddListener(object listener)
		{
			return Subscribe(listener as T);
		}
	}

	public interface ISystemResetEvents
	{
		void SystemResetting();
		void SystemRestarted();
	}
}


namespace Dependencies.PackageKit
{
    using Object = UnityEngine.Object;
    
    public class Language
    {
        public static bool IsChinese
        {
            get
            {
                return Application.systemLanguage == SystemLanguage.Chinese ||
                       Application.systemLanguage == SystemLanguage.ChineseSimplified;
            }
        }
    }

    public abstract class IMGUIEditorWindow : EditorWindow
    {
        public static T Create<T>(bool utility, string title = null) where T : IMGUIEditorWindow
        {
            return string.IsNullOrEmpty(title) ? GetWindow<T>(utility) : GetWindow<T>(utility, title);
        }

        private readonly List<IView> mChildren = new List<IView>();

        private bool mVisible = true;

        public bool Visible
        {
            get { return mVisible; }
            set { mVisible = value; }
        }

        public void AddChild(IView childView)
        {
            mChildren.Add(childView);
        }

        public void RemoveChild(IView childView)
        {
            mChildren.Remove(childView);
        }

        public List<IView> Children
        {
            get { return mChildren; }
        }

        public void RemoveAllChidren()
        {
            mChildren.Clear();
        }

        public abstract void OnClose();


        public abstract void OnUpdate();

        private void OnDestroy()
        {
            OnClose();
        }

        protected abstract void Init();

        private bool mInited = false;

        public virtual void OnGUI()
        {
            if (!mInited)
            {
                Init();
                mInited = true;
            }

            OnUpdate();

            if (Visible)
            {
                mChildren.ForEach(childView => childView.DrawGUI());
            }
        }
    }

    public class SubWindow : EditorWindow, ILayout
    {
        void IView.Hide()
        {
        }

        void IView.DrawGUI()
        {
        }

        ILayout IView.Parent { get; set; }

        private GUIStyle mStyle = null;

        public GUIStyle Style
        {
            get { return mStyle; }
            set { mStyle = value; }
        }

        Color IView.BackgroundColor { get; set; }


        private List<IView> mPrivateChildren = new List<IView>();

        private List<IView> mChildren
        {
            get { return mPrivateChildren; }
            set { mPrivateChildren = value; }
        }

        void IView.RefreshNextFrame()
        {
        }

        void IView.AddLayoutOption(GUILayoutOption option)
        {
        }

        void IView.RemoveFromParent()
        {
        }

        void IView.Refresh()
        {
        }

        public void AddChild(IView view)
        {
            mChildren.Add(view);
            view.Parent = this;
        }

        public void RemoveChild(IView view)
        {
            mChildren.Add(view);
            view.Parent = null;
        }

        public void Clear()
        {
            mChildren.Clear();
        }

        private void OnGUI()
        {
            mChildren.ForEach(view => view.DrawGUI());
        }

        public void Dispose()
        {
        }
    }

    public abstract class View : IView
    {
        public class EventRecord
        {
            public int Key;

            public Action<object> OnEvent;
        }

        List<EventRecord> mPrivteEventRecords = new List<EventRecord>();

        protected List<EventRecord> mEventRecords
        {
            get { return mPrivteEventRecords; }
        }

        protected void RegisterEvent<T>(T key, Action<object> onEvent) where T : IConvertible
        {
            EventDispatcher.Register(key, onEvent);

            mEventRecords.Add(new EventRecord
            {
                Key = key.ToInt32(null),
                OnEvent = onEvent
            });
        }

        protected void UnRegisterAll()
        {
            mEventRecords.ForEach(record => { EventDispatcher.UnRegister(record.Key, record.OnEvent); });

            mEventRecords.Clear();
        }

        protected void SendEvent<T>(T key, object arg) where T : IConvertible
        {
            EventDispatcher.Send(key, arg);
        }

        private bool mVisible = true;

        public bool Visible
        {
            get { return mVisible; }
            set { mVisible = value; }
        }

        private List<GUILayoutOption> mprivateLayoutOptions = new List<GUILayoutOption>();

        private List<GUILayoutOption> mLayoutOptions
        {
            get { return mprivateLayoutOptions; }
        }

        protected GUILayoutOption[] LayoutStyles { get; private set; }


        private GUIStyle mStyle = new GUIStyle();

        public GUIStyle Style
        {
            get { return mStyle; }
            protected set { mStyle = value; }
        }

        private Color mBackgroundColor = GUI.backgroundColor;

        public Color BackgroundColor
        {
            get { return mBackgroundColor; }
            set { mBackgroundColor = value; }
        }

        public void RefreshNextFrame()
        {
            this.PushCommand(Refresh);
        }

        public void AddLayoutOption(GUILayoutOption option)
        {
            mLayoutOptions.Add(option);
        }

        public void Show()
        {
            Visible = true;
            OnShow();
        }

        protected virtual void OnShow()
        {
        }

        public void Hide()
        {
            Visible = false;
            OnHide();
        }

        protected virtual void OnHide()
        {
        }


        private Color mPreviousBackgroundColor;

        public void DrawGUI()
        {
            BeforeDraw();

            if (Visible)
            {
                mPreviousBackgroundColor = GUI.backgroundColor;
                GUI.backgroundColor = BackgroundColor;
                OnGUI();
                GUI.backgroundColor = mPreviousBackgroundColor;
            }
        }

        private bool mBeforeDrawCalled = false;

        void BeforeDraw()
        {
            if (!mBeforeDrawCalled)
            {
                OnBeforeDraw();

                LayoutStyles = mLayoutOptions.ToArray();

                mBeforeDrawCalled = true;
            }
        }

        protected virtual void OnBeforeDraw()
        {
        }

        public ILayout Parent { get; set; }

        public void RemoveFromParent()
        {
            Parent.RemoveChild(this);
        }

        public virtual void Refresh()
        {
            OnRefresh();
        }

        protected virtual void OnRefresh()
        {
        }

        protected abstract void OnGUI();

        public void Dispose()
        {
            UnRegisterAll();
            OnDisposed();
        }

        protected virtual void OnDisposed()
        {
        }
    }

    public abstract class Window : EditorWindow, IDisposable
    {
        public static Window MainWindow { get; protected set; }

        public IMGUIViewController ViewController { get; set; }

        public T CreateViewController<T>() where T : IMGUIViewController, new()
        {
            var t = new T();
            t.SetUpView();
            return t;
        }

        public static void Open<T>(string title) where T : Window
        {
            MainWindow = GetWindow<T>(true);

            if (!MainWindow.mShowing)
            {
                MainWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 800, 600);
                MainWindow.titleContent = new GUIContent(title);
                MainWindow.Init();
                MainWindow.mShowing = true;
                MainWindow.Show();
            }
            else
            {
                MainWindow.mShowing = false;
                MainWindow.Dispose();
                MainWindow.Close();
                MainWindow = null;
            }
        }

        public static SubWindow CreateSubWindow(string name = "SubWindow")
        {
            var window = GetWindow<SubWindow>(true, name);
            window.Clear();
            return window;
        }

        void Init()
        {
            OnInit();
        }

        private Queue<Action> mPrivateCommands = new Queue<Action>();

        private Queue<Action> mCommands
        {
            get { return mPrivateCommands; }
        }

        public void PushCommand(Action command)
        {
            Debug.Log("push command");

            mCommands.Enqueue(command);
        }

        private void OnGUI()
        {
            ViewController.View.DrawGUI();

            while (mCommands.Count > 0)
            {
                Debug.Log(mCommands.Count);
                mCommands.Dequeue().Invoke();
            }
        }

        public void Dispose()
        {
            OnDispose();
        }

        protected bool mShowing = false;


        protected abstract void OnInit();
        protected abstract void OnDispose();
    }


    public class ExpandLayout : Layout
    {
        public ExpandLayout(string label)
        {
            Label = label;
        }

        public string Label { get; set; }


        protected override void OnGUIBegin()
        {
        }


        protected override void OnGUI()
        {
//            if (GUIHelpers.DoToolbarEx(Label))
//            {
//                foreach (var child in Children)
//                {
//                    child.DrawGUI();
//                }
//            }
        }

        protected override void OnGUIEnd()
        {
        }
    }

    public class HorizontalLayout : Layout
    {
        public string HorizontalStyle { get; set; }

        public HorizontalLayout(string horizontalStyle = null)
        {
            HorizontalStyle = horizontalStyle;
        }

        protected override void OnGUIBegin()
        {
            if (string.IsNullOrEmpty(HorizontalStyle))
            {
                GUILayout.BeginHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal(HorizontalStyle);
            }
        }

        protected override void OnGUIEnd()
        {
            GUILayout.EndHorizontal();
        }
    }

    public class ScrollLayout : Layout
    {
        Vector2 mScrollPos = Vector2.zero;

        protected override void OnGUIBegin()
        {
            mScrollPos = GUILayout.BeginScrollView(mScrollPos, LayoutStyles);
        }

        protected override void OnGUIEnd()
        {
            GUILayout.EndScrollView();
        }
    }

    public class TreeNode : VerticalLayout
    {
        public Property<bool> Spread = null;

        public string Content;


        HorizontalLayout mFirstLine = new HorizontalLayout();

        private VerticalLayout mSpreadView = new VerticalLayout();

        public TreeNode(bool spread, string content, int indent = 0)
        {
            Content = content;
            Spread = new Property<bool>(spread);

            Style = new GUIStyle(EditorStyles.foldout);

            mFirstLine.AddTo(this);
            mFirstLine.AddChild(new SpaceView(indent));

            new CustomView(() =>
            {
                Spread.Value = EditorGUILayout.Foldout(Spread.Value, Content, true, Style);
            }).AddTo(mFirstLine);

            new CustomView(() =>
            {
                if (Spread.Value)
                {
                    mSpreadView.DrawGUI();
                }
            }).AddTo(this);
        }

        public TreeNode Add2FirstLine(IView view)
        {
            view.AddTo(mFirstLine);
            return this;
        }

        public TreeNode FirstLineBox()
        {
            mFirstLine.HorizontalStyle = "box";

            return this;
        }

        public TreeNode SpreadBox()
        {
            mSpreadView.VerticalStyle = "box";

            return this;
        }

        public TreeNode Add2Spread(IView view)
        {
            view.AddTo(mSpreadView);
            return this;
        }
    }

    public class VerticalLayout : Layout
    {
        public string VerticalStyle { get; set; }

        public VerticalLayout(string verticalStyle = null)
        {
            VerticalStyle = verticalStyle;
        }

        protected override void OnGUIBegin()
        {
            if (string.IsNullOrEmpty(VerticalStyle))
            {
                GUILayout.BeginVertical(LayoutStyles);
            }
            else
            {
                GUILayout.BeginVertical(VerticalStyle, LayoutStyles);
            }
        }

        protected override void OnGUIEnd()
        {
            GUILayout.EndVertical();
        }
    }

    public abstract class Layout : View, ILayout
    {
        protected List<IView> Children = new List<IView>();

        public void AddChild(IView view)
        {
            Children.Add(view);
            view.Parent = this;
        }

        public void RemoveChild(IView view)
        {
            this.PushCommand(() =>
            {
                Children.Remove(view);
                view.Parent = null;
            });

            view.Dispose();
        }

        public void Clear()
        {
            Children.Clear();
        }

        public override void Refresh()
        {
            Children.ForEach(view => view.Refresh());
            base.Refresh();
        }

        protected override void OnGUI()
        {
            OnGUIBegin();
            foreach (var child in Children)
            {
                child.DrawGUI();
            }

            OnGUIEnd();
        }

        protected abstract void OnGUIBegin();
        protected abstract void OnGUIEnd();
    }

    public interface IView : IDisposable
    {
        void Show();

        void Hide();

        void DrawGUI();

        ILayout Parent { get; set; }

        GUIStyle Style { get; }

        Color BackgroundColor { get; set; }

        void RefreshNextFrame();

        void AddLayoutOption(GUILayoutOption option);

        void RemoveFromParent();

        void Refresh();
    }

    public interface ILayout : IView
    {
        void AddChild(IView view);

        void RemoveChild(IView view);

        void Clear();
    }

    public static class WindowExtension
    {
        public static T PushCommand<T>(this T view, Action command) where T : IView
        {
            Window.MainWindow.PushCommand(command);
            return view;
        }
    }

    public static class SubWindowExtension
    {
        public static T Postion<T>(this T subWindow, int x, int y) where T : SubWindow
        {
            var rect = subWindow.position;
            rect.x = x;
            rect.y = y;
            subWindow.position = rect;

            return subWindow;
        }

        public static T Size<T>(this T subWindow, int width, int height) where T : SubWindow
        {
            var rect = subWindow.position;
            rect.width = width;
            rect.height = height;
            subWindow.position = rect;

            return subWindow;
        }

        public static T PostionScreenCenter<T>(this T subWindow) where T : SubWindow
        {
            var rect = subWindow.position;
            rect.x = Screen.width / 2;
            rect.y = Screen.height / 2;
            subWindow.position = rect;

            return subWindow;
        }
    }

    public static class ViewExtension
    {
        public static T Width<T>(this T view, float width) where T : IView
        {
            view.AddLayoutOption(GUILayout.Width(width));
            return view;
        }

        public static T Height<T>(this T view, float height) where T : IView
        {
            view.AddLayoutOption(GUILayout.Height(height));
            return view;
        }

        public static T MaxHeight<T>(this T view, float height) where T : IView
        {
            view.AddLayoutOption(GUILayout.MaxHeight(height));
            return view;
        }

        public static T MinHeight<T>(this T view, float height) where T : IView
        {
            view.AddLayoutOption(GUILayout.MinHeight(height));
            return view;
        }

        public static T ExpandHeight<T>(this T view) where T : IView
        {
            view.AddLayoutOption(GUILayout.ExpandHeight(true));
            return view;
        }


        public static T TextMiddleLeft<T>(this T view) where T : IView
        {
            view.Style.alignment = TextAnchor.MiddleLeft;
            return view;
        }

        public static T TextMiddleRight<T>(this T view) where T : IView
        {
            view.Style.alignment = TextAnchor.MiddleRight;
            return view;
        }

        public static T TextLowerRight<T>(this T view) where T : IView
        {
            view.Style.alignment = TextAnchor.LowerRight;
            return view;
        }

        public static T TextMiddleCenter<T>(this T view) where T : IView
        {
            view.Style.alignment = TextAnchor.MiddleCenter;
            return view;
        }

        public static T TextLowerCenter<T>(this T view) where T : IView
        {
            view.Style.alignment = TextAnchor.LowerCenter;
            return view;
        }

        public static T Color<T>(this T view, Color color) where T : IView
        {
            view.BackgroundColor = color;
            return view;
        }

        public static T FontColor<T>(this T view, Color color) where T : IView
        {
            view.Style.normal.textColor = color;
            return view;
        }

        public static T FontBold<T>(this T view) where T : IView
        {
            view.Style.fontStyle = FontStyle.Bold;
            return view;
        }

        public static T FontNormal<T>(this T view) where T : IView
        {
            view.Style.fontStyle = FontStyle.Normal;
            return view;
        }

        public static T FontSize<T>(this T view, int fontSize) where T : IView
        {
            view.Style.fontSize = fontSize;
            return view;
        }
    }

    public static class LayoutExtension
    {
        public static T AddTo<T>(this T view, ILayout parent) where T : IView
        {
            parent.AddChild(view);
            return view;
        }
    }

    [Serializable]
    public class IntProperty : Property<int>
    {
        public int Value
        {
            get { return base.Value; }
            set { base.Value = value; }
        }
    }


    [Serializable]
    public class Property<T>
    {
        public Property()
        {
        }

        private bool setted = false;

        public Property(T initValue)
        {
            mValue = initValue;
        }

        public T Value
        {
            get { return mValue; }
            set
            {
                if (value == null || !value.Equals(mValue) || !setted)
                {
                    mValue = value;

                    if (mSetter != null)
                    {
                        mSetter.Invoke(mValue);
                    }

                    setted = true;
                }
            }
        }

        private T mValue;

        /// <summary>
        /// TODO:注销也要做下
        /// </summary>
        /// <param name="setter"></param>
        public void Bind(Action<T> setter)
        {
            mSetter += setter;
            mBindings.Add(setter);
        }

        private List<Action<T>> mBindings = new List<Action<T>>();

        public void UnBindAll()
        {
            foreach (var binding in mBindings)
            {
                mSetter -= binding;
            }
        }

        private event Action<T> mSetter;
    }

    public static class EditorUtils
    {
        public static void MarkCurrentSceneDirty()
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        public static string CurrentSelectPath
        {
            get { return Selection.activeObject == null ? null : AssetDatabase.GetAssetPath(Selection.activeObject); }
        }

        public static string AssetsPath2ABSPath(string assetsPath)
        {
            string assetRootPath = Path.GetFullPath(Application.dataPath);
            return assetRootPath.Substring(0, assetRootPath.Length - 6) + assetsPath;
        }

        public static string ABSPath2AssetsPath(string absPath)
        {
            string assetRootPath = Path.GetFullPath(Application.dataPath);
            Debug.Log(assetRootPath);
            Debug.Log(Path.GetFullPath(absPath));
            return "Assets" + Path.GetFullPath(absPath).Substring(assetRootPath.Length).Replace("\\", "/");
        }


        public static string AssetPath2ReltivePath(string path)
        {
            if (path == null)
            {
                return null;
            }

            return path.Replace("Assets/", "");
        }

        public static bool ExcuteCmd(string toolName, string args, bool isThrowExcpetion = true)
        {
            Process process = new Process();
            process.StartInfo.FileName = toolName;
            process.StartInfo.Arguments = args;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            OuputProcessLog(process, isThrowExcpetion);
            return true;
        }

        public static void OuputProcessLog(Process p, bool isThrowExcpetion)
        {
            string standardError = string.Empty;
            p.BeginErrorReadLine();

            p.ErrorDataReceived += (sender, outLine) => { standardError += outLine.Data; };

            string standardOutput = string.Empty;
            p.BeginOutputReadLine();
            p.OutputDataReceived += (sender, outLine) => { standardOutput += outLine.Data; };

            p.WaitForExit();
            p.Close();

            Log.I(standardOutput);
            if (standardError.Length > 0)
            {
                if (isThrowExcpetion)
                {
                    Log.E(standardError);
                    throw new Exception(standardError);
                }

                Log.I(standardError);
            }
        }

        public static Dictionary<string, string> ParseArgs(string argString)
        {
            int curPos = argString.IndexOf('-');
            Dictionary<string, string> result = new Dictionary<string, string>();

            while (curPos != -1 && curPos < argString.Length)
            {
                int nextPos = argString.IndexOf('-', curPos + 1);
                string item = string.Empty;

                if (nextPos != -1)
                {
                    item = argString.Substring(curPos + 1, nextPos - curPos - 1);
                }
                else
                {
                    item = argString.Substring(curPos + 1, argString.Length - curPos - 1);
                }

                item = StringTrim(item);
                int splitPos = item.IndexOf(' ');

                if (splitPos == -1)
                {
                    string key = StringTrim(item);
                    result[key] = "";
                }
                else
                {
                    string key = StringTrim(item.Substring(0, splitPos));
                    string value = StringTrim(item.Substring(splitPos + 1, item.Length - splitPos - 1));
                    result[key] = value;
                }

                curPos = nextPos;
            }

            return result;
        }

        public static string GetFileMD5Value(string absPath)
        {
            if (!File.Exists(absPath))
                return "";

            MD5CryptoServiceProvider md5CSP = new MD5CryptoServiceProvider();
            FileStream file = new FileStream(absPath, FileMode.Open);
            byte[] retVal = md5CSP.ComputeHash(file);
            file.Close();
            string result = "";

            for (int i = 0; i < retVal.Length; i++)
            {
                result += retVal[i].ToString("x2");
            }

            return result;
        }

        public static string GetStrMD5Value(string str)
        {
            MD5CryptoServiceProvider md5CSP = new MD5CryptoServiceProvider();
            byte[] retVal = md5CSP.ComputeHash(Encoding.Default.GetBytes(str));
            string retStr = "";

            for (int i = 0; i < retVal.Length; i++)
            {
                retStr += retVal[i].ToString("x2");
            }

            return retStr;
        }

        public static List<Object> GetDirSubAssetsList(string dirAssetsPath, bool isRecursive = true,
            string suffix = "", bool isLoadAll = false)
        {
            string dirABSPath = ABSPath2AssetsPath(dirAssetsPath);
            Debug.Log(dirABSPath);
            List<string> assetsABSPathList = dirABSPath.GetDirSubFilePathList(isRecursive, suffix);
            List<Object> resultObjectList = new List<Object>();

            for (int i = 0; i < assetsABSPathList.Count; ++i)
            {
                Debug.Log(assetsABSPathList[i]);
                if (isLoadAll)
                {
                    Object[] objs = AssetDatabase.LoadAllAssetsAtPath(ABSPath2AssetsPath(assetsABSPathList[i]));
                    resultObjectList.AddRange(objs);
                }
                else
                {
                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(ABSPath2AssetsPath(assetsABSPathList[i]));
                    resultObjectList.Add(obj);
                }
            }

            return resultObjectList;
        }

        public static List<T> GetDirSubAssetsList<T>(string dirAssetsPath, bool isRecursive = true, string suffix = "",
            bool isLoadAll = false) where T : Object
        {
            List<T> result = new List<T>();
            List<Object> objectList = GetDirSubAssetsList(dirAssetsPath, isRecursive, suffix, isLoadAll);

            for (int i = 0; i < objectList.Count; ++i)
            {
                if (objectList[i] is T)
                {
                    result.Add(objectList[i] as T);
                }
            }

            return result;
        }

        public static string GetSelectedDirAssetsPath()
        {
            string path = string.Empty;

            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }

            return path;
        }

        public static string StringTrim(string str, params char[] trimer)
        {
            int startIndex = 0;
            int endIndex = str.Length;

            for (int i = 0; i < str.Length; ++i)
            {
                if (!IsInCharArray(trimer, str[i]))
                {
                    startIndex = i;
                    break;
                }
            }

            for (int i = str.Length - 1; i >= 0; --i)
            {
                if (!IsInCharArray(trimer, str[i]))
                {
                    endIndex = i;
                    break;
                }
            }

            if (startIndex == 0 && endIndex == str.Length)
            {
                return string.Empty;
            }

            return str.Substring(startIndex, endIndex - startIndex + 1);
        }

        public static string StringTrim(string str)
        {
            return StringTrim(str, ' ', '\t');
        }

        static bool IsInCharArray(char[] array, char c)
        {
            for (int i = 0; i < array.Length; ++i)
            {
                if (array[i] == c)
                {
                    return true;
                }
            }

            return false;
        }

        public static void ClearAssetBundlesName()
        {
            int length = AssetDatabase.GetAllAssetBundleNames().Length;
            string[] oldAssetBundleNames = new string[length];

            for (int i = 0; i < length; i++)
            {
                oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
            }

            for (int j = 0; j < oldAssetBundleNames.Length; j++)
            {
                AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
            }

            length = AssetDatabase.GetAllAssetBundleNames().Length;
            AssetDatabase.SaveAssets();
        }

        public static bool SetAssetBundleName(string assetsPath, string bundleName)
        {
            AssetImporter ai = AssetImporter.GetAtPath(assetsPath);

            if (ai != null)
            {
                ai.assetBundleName = bundleName + ".assetbundle";
                return true;
            }

            return false;
        }

        public static void SafeRemoveAsset(string assetsPath)
        {
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetsPath);

            if (obj != null)
            {
                AssetDatabase.DeleteAsset(assetsPath);
            }
        }

        public static void Abort(string errMsg)
        {
            Log.E("BatchMode Abort Exit " + errMsg);
            Thread.CurrentThread.Abort();
            Process.GetCurrentProcess().Kill();

            Environment.ExitCode = 1;
            Environment.Exit(1);

            EditorApplication.Exit(1);
        }
    }

    public static class MouseSelector
    {
        public static string GetSelectedPathOrFallback()
        {
            var path = string.Empty;

            foreach (var obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (path.IsNotNullAndEmpty() && File.Exists(path))
                {
                }
            }

            return path;
        }
    }

    public class BoxView : View
    {
        public string Text;

        public BoxView(string text)
        {
            Text = text;
            //Style = new GUIStyle(GUI.skin.box);
        }

        protected override void OnGUI()
        {
            GUILayout.Box(Text, GUI.skin.box, LayoutStyles);
        }
    }

    public class ColorView : View
    {
        public ColorView(Color color)
        {
            Color = new Property<Color>(color);
        }

        public Property<Color> Color { get; private set; }

        protected override void OnGUI()
        {
            Color.Value = EditorGUILayout.ColorField(Color.Value, LayoutStyles);
        }
    }

    public class ButtonView : View
    {
        public ButtonView(string text, Action onClickEvent)
        {
            Text = text;
            OnClickEvent = onClickEvent;
            //Style = new GUIStyle(GUI.skin.button);
        }

        public string Text { get; set; }

        public Action OnClickEvent { get; set; }

        protected override void OnGUI()
        {
            if (GUILayout.Button(Text, GUI.skin.button, LayoutStyles))
            {
                OnClickEvent.Invoke();
            }
        }
    }

    public class CustomView : View
    {
        public CustomView(Action onGuiAction)
        {
            OnGUIAction = onGuiAction;
        }

        public Action OnGUIAction { get; set; }

        protected override void OnGUI()
        {
            OnGUIAction.Invoke();
        }
    }

    public class EnumPopupView : View //where T : struct
    {
        public Property<Enum> ValueProperty { get; private set; }

        public EnumPopupView(Enum initValue)
        {
            ValueProperty = new Property<Enum>(initValue);
            ValueProperty.Value = initValue;

            try
            {
                Style = new GUIStyle(EditorStyles.popup);
            }
            catch (Exception e)
            {
            }
        }

        protected override void OnGUI()
        {
            Enum enumType = ValueProperty.Value;
            ValueProperty.Value = EditorGUILayout.EnumPopup(enumType, Style, LayoutStyles);
        }
    }

    public class FlexibaleSpaceView : View
    {
        protected override void OnGUI()
        {
            GUILayout.FlexibleSpace();
        }
    }

    public class ImageButtonView : View
    {
        private Texture2D mTexture2D { get; set; }

        private Action mOnClick { get; set; }

        public ImageButtonView(string texturePath, Action onClick)
        {
            mTexture2D = Resources.Load<Texture2D>(texturePath);
            mOnClick = onClick;

            //Style = new GUIStyle(GUI.skin.button);
        }

        protected override void OnGUI()
        {
            if (GUILayout.Button(mTexture2D, LayoutStyles))
            {
                mOnClick.Invoke();
            }
        }
    }

    public class LabelView : View
    {
        public string Content { get; set; }

        public LabelView(string content)
        {
            Content = content;
            try
            {
                Style = new GUIStyle(GUI.skin.label);
            }
            catch (Exception e)
            {
            }
        }


        protected override void OnGUI()
        {
            GUILayout.Label(Content, Style, LayoutStyles);
        }
    }

    public class PopupView : View
    {
        public Property<int> IndexProperty { get; private set; }

        public string[] MenuArray { get; private set; }

        public PopupView(int initValue, string[] menuArray)
        {
            MenuArray = menuArray;
            IndexProperty = new Property<int>(initValue);
            IndexProperty.Value = initValue;

            Style = new GUIStyle(EditorStyles.popup);
        }

        protected override void OnGUI()
        {
            IndexProperty.Value = EditorGUILayout.Popup(IndexProperty.Value, MenuArray, LayoutStyles);
        }
    }

    public class SpaceView : View
    {
        public int Pixel { get; set; }

        public SpaceView(int pixel = 10)
        {
            Pixel = pixel;
        }

        protected override void OnGUI()
        {
            GUILayout.Space(Pixel);
        }
    }

    public class TextAreaView : View
    {
        public TextAreaView(string content)
        {
            Content = new Property<string>(content);
            //Style = new GUIStyle(GUI.skin.textArea);
        }

        public Property<string> Content { get; set; }

        protected override void OnGUI()
        {
            Content.Value = EditorGUILayout.TextArea(Content.Value, GUI.skin.textArea, LayoutStyles);
        }
    }

    public class TextView : View
    {
        public TextView(string content)
        {
            Content = new Property<string>(content);
            //Style = GUI.skin.textField;
        }

        public Property<string> Content { get; set; }

        protected override void OnGUI()
        {
            if (mPasswordMode)
            {
                Content.Value = EditorGUILayout.PasswordField(Content.Value, GUI.skin.textField, LayoutStyles);
            }
            else
            {
                Content.Value = EditorGUILayout.TextField(Content.Value, GUI.skin.textField, LayoutStyles);
            }
        }


        private bool mPasswordMode = false;

        public TextView PasswordMode()
        {
            mPasswordMode = true;
            return this;
        }
    }

    public class ToggleView : View
    {
        public string Text { get; private set; }

        public ToggleView(string text, bool initValue = false)
        {
            Text = text;
            Toggle = new Property<bool>(initValue);

            try
            {
                Style = new GUIStyle(GUI.skin.toggle);
            }
            catch (System.Exception e)
            {
            }
        }

        public Property<bool> Toggle { get; private set; }

        protected override void OnGUI()
        {
            // Toggle.Value = GUILayout.Toggle(Toggle.Value, Text, Style, LayoutStyles);
            Toggle.Value = GUILayout.Toggle(Toggle.Value, Text, LayoutStyles);
        }
    }

    public class ToolbarView : View
    {
        public ToolbarView(int defaultIndex = 0)
        {
            Index.Value = defaultIndex;
            Index.Bind(index => MenuSelected[index].Invoke(MenuNames[index]));
        }


        public ToolbarView Menus(List<string> menuNames)
        {
            this.MenuNames = menuNames;
            // empty
            this.MenuSelected = MenuNames.Select(menuName => new Action<string>((str => { }))).ToList();
            return this;
        }

        public ToolbarView AddMenu(string name, Action<string> onMenuSelected)
        {
            MenuNames.Add(name);
            MenuSelected.Add(onMenuSelected);
            return this;
        }

        List<string> MenuNames = new List<string>();

        List<Action<string>> MenuSelected = new List<Action<string>>();

        public Property<int> Index = new Property<int>(0);

        protected override void OnGUI()
        {
            Index.Value = GUILayout.Toolbar(Index.Value, MenuNames.ToArray(), GUI.skin.button, LayoutStyles);
        }
    }

    public static class EventDispatcher
    {
        private static Dictionary<int, Action<object>> mRegisteredEvents = new Dictionary<int, Action<object>>();

        public static void Register<T>(T key, Action<object> onEvent) where T : IConvertible
        {
            int intKey = key.ToInt32(null);

            Action<object> registerdEvent;
            if (!mRegisteredEvents.TryGetValue(intKey, out registerdEvent))
            {
                registerdEvent = (_) => { };
                registerdEvent += onEvent;
                mRegisteredEvents.Add(intKey, registerdEvent);
            }
            else
            {
                mRegisteredEvents[intKey] += onEvent;
            }
        }

        public static void UnRegister<T>(T key, Action<object> onEvent) where T : IConvertible
        {
            int intKey = key.ToInt32(null);

            Action<object> registerdEvent;
            if (!mRegisteredEvents.TryGetValue(intKey, out registerdEvent))
            {
            }
            else
            {
                registerdEvent -= onEvent;
            }
        }

        public static void UnRegisterAll<T>(T key) where T : IConvertible
        {
            int intKey = key.ToInt32(null);
            mRegisteredEvents.Remove(intKey);
        }

        public static void Send<T>(T key, object arg = null) where T : IConvertible
        {
            int intKey = key.ToInt32(null);

            Action<object> registeredEvent;
            if (mRegisteredEvents.TryGetValue(intKey, out registeredEvent))
            {
                registeredEvent.Invoke(arg);
            }
        }
    }

    public static class ColorUtil
    {
        public static string ToText(this Color color)
        {
            return string.Format("{0}@{1}@{2}@{3}", color.r, color.g, color.b, color.a);
        }

        public static Color ToColor(this string colorText)
        {
            var channels = colorText.Split('@');
            return new Color(
                float.Parse(channels[0]),
                float.Parse(channels[1]),
                float.Parse(channels[2]),
                float.Parse(channels[3]));
        }
    }

    public abstract class IMGUIViewController
    {
        public VerticalLayout View = new VerticalLayout();

        public abstract void SetUpView();
    }

    /// <summary>
    /// Used by the injection container to determine if a property or field should be injected.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    internal class InjectAttribute : Attribute
    {
        public InjectAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public InjectAttribute()
        {
        }
    }

    internal interface IQFrameworkContainer
    {
        /// <summary>
        /// Clears all type mappings and instances.
        /// </summary>
        void Clear();

        /// <summary>
        /// Injects registered types/mappings into an object
        /// </summary>
        /// <param name="obj"></param>
        void Inject(object obj);

        /// <summary>
        /// Injects everything that is registered at once
        /// </summary>
        void InjectAll();

        /// <summary>
        /// Register a type mapping
        /// </summary>
        /// <typeparam name="TSource">The base type.</typeparam>
        /// <typeparam name="TTarget">The concrete type</typeparam>
        void Register<TSource, TTarget>(string name = null);

        void RegisterRelation<TFor, TBase, TConcrete>();

        /// <summary>
        /// Register an instance of a type.
        /// </summary>
        /// <typeparam name="TBase"></typeparam>
        /// <param name="default"></param>
        /// <param name="injectNow"></param>
        /// <returns></returns>
        void RegisterInstance<TBase>(TBase @default, bool injectNow) where TBase : class;

        /// <summary>
        /// Register an instance of a type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="default"></param>
        /// <param name="injectNow"></param>
        /// <returns></returns>
        void RegisterInstance(Type type, object @default, bool injectNow);

        /// <summary>
        /// Register a named instance
        /// </summary>
        /// <param name="baseType">The type to register the instance for.</param>
        /// <param name="name">The name for the instance to be resolved.</param>
        /// <param name="instance">The instance that will be resolved be the name</param>
        /// <param name="injectNow">Perform the injection immediately</param>
        void RegisterInstance(Type baseType, object instance = null, string name = null, bool injectNow = true);

        void RegisterInstance<TBase>(TBase instance, string name, bool injectNow = true) where TBase : class;

        void RegisterInstance<TBase>(TBase instance) where TBase : class;

        /// <summary>
        ///  If an instance of T exist then it will return that instance otherwise it will create a new one based off mappings.
        /// </summary>
        /// <typeparam name="T">The type of instance to resolve</typeparam>
        /// <returns>The/An instance of 'instanceType'</returns>
        T Resolve<T>(string name = null, bool requireInstance = false, params object[] args) where T : class;

        TBase ResolveRelation<TBase>(Type tfor, params object[] arg);

        TBase ResolveRelation<TFor, TBase>(params object[] arg);

        /// <summary>
        /// Resolves all instances of TType or subclasses of TType.  Either named or not.
        /// </summary>
        /// <typeparam name="TType">The Type to resolve</typeparam>
        /// <returns>List of objects.</returns>
        IEnumerable<TType> ResolveAll<TType>();

        //IEnumerable<object> ResolveAll(Type type);
        void Register(Type source, Type target, string name = null);

        /// <summary>
        /// Resolves all instances of TType or subclasses of TType.  Either named or not.
        /// </summary>
        /// <typeparam name="TType">The Type to resolve</typeparam>
        /// <returns>List of objects.</returns>
        IEnumerable<object> ResolveAll(Type type);

        TypeMappingCollection  Mappings             { get; set; }
        TypeInstanceCollection Instances            { get; set; }
        TypeRelationCollection RelationshipMappings { get; set; }

        /// <summary>
        /// If an instance of instanceType exist then it will return that instance otherwise it will create a new one based off mappings.
        /// </summary>
        /// <param name="baseType">The type of instance to resolve</param>
        /// <param name="name">The type of instance to resolve</param>
        /// <param name="requireInstance">If true will return null if an instance isn't registered.</param>
        /// <returns>The/An instance of 'instanceType'</returns>
        object Resolve(Type baseType, string name = null, bool requireInstance = false,
            params object[] constructorArgs);

        object ResolveRelation(Type tfor, Type tbase, params object[] arg);
        void RegisterRelation(Type tfor, Type tbase, Type tconcrete);
        object CreateInstance(Type type, params object[] args);
    }

    /// <summary>
    /// A ViewModel Container and a factory for Controllers and commands.
    /// </summary>
    internal class QFrameworkContainer : IQFrameworkContainer
    {
        private TypeInstanceCollection _instances;
        private TypeMappingCollection  _mappings;


        public TypeMappingCollection Mappings
        {
            get { return _mappings ?? (_mappings = new TypeMappingCollection()); }
            set { _mappings = value; }
        }

        public TypeInstanceCollection Instances
        {
            get { return _instances ?? (_instances = new TypeInstanceCollection()); }
            set { _instances = value; }
        }

        public TypeRelationCollection RelationshipMappings
        {
            get { return _relationshipMappings; }
            set { _relationshipMappings = value; }
        }

        public IEnumerable<TType> ResolveAll<TType>()
        {
            foreach (var obj in ResolveAll(typeof(TType)))
            {
                yield return (TType) obj;
            }
        }

        /// <summary>
        /// Resolves all instances of TType or subclasses of TType.  Either named or not.
        /// </summary>
        /// <typeparam name="TType">The Type to resolve</typeparam>
        /// <returns>List of objects.</returns>
        public IEnumerable<object> ResolveAll(Type type)
        {
            foreach (KeyValuePair<Tuple<Type, string>, object> kv in Instances)
            {
                if (kv.Key.Item1 == type && !string.IsNullOrEmpty(kv.Key.Item2))
                    yield return kv.Value;
            }

            foreach (KeyValuePair<Tuple<Type, string>, Type> kv in Mappings)
            {
                if (!string.IsNullOrEmpty(kv.Key.Item2))
                {
#if NETFX_CORE
                    var condition = type.GetTypeInfo().IsSubclassOf(mapping.From);
#else
                    var condition = type.IsAssignableFrom(kv.Key.Item1);
#endif
                    if (condition)
                    {
                        var item = Activator.CreateInstance(kv.Value);
                        Inject(item);
                        yield return item;
                    }
                }
            }
        }

        /// <summary>
        /// Clears all type-mappings and instances.
        /// </summary>
        public void Clear()
        {
            Instances.Clear();
            Mappings.Clear();
            RelationshipMappings.Clear();
        }

        /// <summary>
        /// Injects registered types/mappings into an object
        /// </summary>
        /// <param name="obj"></param>
        public void Inject(object obj)
        {
            if (obj == null) return;
#if !NETFX_CORE
            var members = obj.GetType().GetMembers();
#else
            var members = obj.GetType().GetTypeInfo().DeclaredMembers;
#endif
            foreach (var memberInfo in members)
            {
                var injectAttribute =
                    memberInfo.GetCustomAttributes(typeof(InjectAttribute), true).FirstOrDefault() as InjectAttribute;
                if (injectAttribute != null)
                {
                    if (memberInfo is PropertyInfo)
                    {
                        var propertyInfo = memberInfo as PropertyInfo;
                        propertyInfo.SetValue(obj, Resolve(propertyInfo.PropertyType, injectAttribute.Name), null);
                    }
                    else if (memberInfo is FieldInfo)
                    {
                        var fieldInfo = memberInfo as FieldInfo;
                        fieldInfo.SetValue(obj, Resolve(fieldInfo.FieldType, injectAttribute.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Register a type mapping
        /// </summary>
        /// <typeparam name="TSource">The base type.</typeparam>
        /// <typeparam name="TTarget">The concrete type</typeparam>
        public void Register<TSource>(string name = null)
        {
            Mappings[typeof(TSource), name] = typeof(TSource);
        }


        /// <summary>
        /// Register a type mapping
        /// </summary>
        /// <typeparam name="TSource">The base type.</typeparam>
        /// <typeparam name="TTarget">The concrete type</typeparam>
        public void Register<TSource, TTarget>(string name = null)
        {
            Mappings[typeof(TSource), name] = typeof(TTarget);
        }

        public void Register(Type source, Type target, string name = null)
        {
            Mappings[source, name] = target;
        }

        /// <summary>
        /// Register a named instance
        /// </summary>
        /// <param name="baseType">The type to register the instance for.</param>        
        /// <param name="instance">The instance that will be resolved be the name</param>
        /// <param name="injectNow">Perform the injection immediately</param>
        public void RegisterInstance(Type baseType, object instance = null, bool injectNow = true)
        {
            RegisterInstance(baseType, instance, null, injectNow);
        }

        /// <summary>
        /// Register a named instance
        /// </summary>
        /// <param name="baseType">The type to register the instance for.</param>
        /// <param name="name">The name for the instance to be resolved.</param>
        /// <param name="instance">The instance that will be resolved be the name</param>
        /// <param name="injectNow">Perform the injection immediately</param>
        public virtual void RegisterInstance(Type baseType, object instance = null, string name = null,
            bool injectNow = true)
        {
            Instances[baseType, name] = instance;
            if (injectNow)
            {
                Inject(instance);
            }
        }

        public void RegisterInstance<TBase>(TBase instance) where TBase : class
        {
            RegisterInstance<TBase>(instance, true);
        }

        public void RegisterInstance<TBase>(TBase instance, bool injectNow) where TBase : class
        {
            RegisterInstance<TBase>(instance, null, injectNow);
        }

        public void RegisterInstance<TBase>(TBase instance, string name, bool injectNow = true) where TBase : class
        {
            RegisterInstance(typeof(TBase), instance, name, injectNow);
        }

        /// <summary>
        ///  If an instance of T exist then it will return that instance otherwise it will create a new one based off mappings.
        /// </summary>
        /// <typeparam name="T">The type of instance to resolve</typeparam>
        /// <returns>The/An instance of 'instanceType'</returns>
        public T Resolve<T>(string name = null, bool requireInstance = false, params object[] args) where T : class
        {
            return (T) Resolve(typeof(T), name, requireInstance, args);
        }

        /// <summary>
        /// If an instance of instanceType exist then it will return that instance otherwise it will create a new one based off mappings.
        /// </summary>
        /// <param name="baseType">The type of instance to resolve</param>
        /// <param name="name">The type of instance to resolve</param>
        /// <param name="requireInstance">If true will return null if an instance isn't registered.</param>
        /// <param name="constructorArgs">The arguments to pass to the constructor if any.</param>
        /// <returns>The/An instance of 'instanceType'</returns>
        public object Resolve(Type baseType, string name = null, bool requireInstance = false,
            params object[] constructorArgs)
        {
            // Look for an instance first
            var item = Instances[baseType, name];
            if (item != null)
            {
                return item;
            }

            if (requireInstance)
                return null;
            // Check if there is a mapping of the type
            var namedMapping = Mappings[baseType, name];
            if (namedMapping != null)
            {
                var obj = CreateInstance(namedMapping, constructorArgs);
                //Inject(obj);
                return obj;
            }

            return null;
        }

        public object CreateInstance(Type type, params object[] constructorArgs)
        {
            if (constructorArgs != null && constructorArgs.Length > 0)
            {
                //return Activator.CreateInstance(type,BindingFlags.Public | BindingFlags.Instance,Type.DefaultBinder, constructorArgs,CultureInfo.CurrentCulture);
                var obj2 = Activator.CreateInstance(type, constructorArgs);
                Inject(obj2);
                return obj2;
            }
#if !NETFX_CORE
            ConstructorInfo[] constructor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
#else
        ConstructorInfo[] constructor = type.GetTypeInfo().DeclaredConstructors.ToArray();
#endif

            if (constructor.Length < 1)
            {
                var obj2 = Activator.CreateInstance(type);
                Inject(obj2);
                return obj2;
            }

            var maxParameters = constructor.First().GetParameters();

            foreach (var c in constructor)
            {
                var parameters = c.GetParameters();
                if (parameters.Length > maxParameters.Length)
                {
                    maxParameters = parameters;
                }
            }

            var args = maxParameters.Select(p =>
            {
                if (p.ParameterType.IsArray)
                {
                    return ResolveAll(p.ParameterType);
                }

                return Resolve(p.ParameterType) ?? Resolve(p.ParameterType, p.Name);
            }).ToArray();

            var obj = Activator.CreateInstance(type, args);
            Inject(obj);
            return obj;
        }

        public TBase ResolveRelation<TBase>(Type tfor, params object[] args)
        {
            try
            {
                return (TBase) ResolveRelation(tfor, typeof(TBase), args);
            }
            catch (InvalidCastException castIssue)
            {
                throw new Exception(
                    string.Format("Resolve Relation couldn't cast  to {0} from {1}", typeof(TBase).Name, tfor.Name),
                    castIssue);
            }
        }

        public void InjectAll()
        {
            foreach (object instance in Instances.Values)
            {
                Inject(instance);
            }
        }

        private TypeRelationCollection _relationshipMappings = new TypeRelationCollection();

        public void RegisterRelation<TFor, TBase, TConcrete>()
        {
            RelationshipMappings[typeof(TFor), typeof(TBase)] = typeof(TConcrete);
        }

        public void RegisterRelation(Type tfor, Type tbase, Type tconcrete)
        {
            RelationshipMappings[tfor, tbase] = tconcrete;
        }

        public object ResolveRelation(Type tfor, Type tbase, params object[] args)
        {
            var concreteType = RelationshipMappings[tfor, tbase];

            if (concreteType == null)
            {
                return null;
            }

            var result = CreateInstance(concreteType, args);
            //Inject(result);
            return result;
        }

        public TBase ResolveRelation<TFor, TBase>(params object[] arg)
        {
            return (TBase) ResolveRelation(typeof(TFor), typeof(TBase), arg);
        }
    }

    // http://stackoverflow.com/questions/1171812/multi-key-dictionary-in-c
    internal class Tuple<T1, T2> //FUCKING Unity: struct is not supported in Mono
    {
        public readonly T1 Item1;
        public readonly T2 Item2;

        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public override bool Equals(object obj)
        {
            Tuple<T1, T2> p = obj as Tuple<T1, T2>;
            if (obj == null) return false;

            if (Item1 == null)
            {
                if (p.Item1 != null) return false;
            }
            else
            {
                if (p.Item1 == null || !Item1.Equals(p.Item1)) return false;
            }

            if (Item2 == null)
            {
                if (p.Item2 != null) return false;
            }
            else
            {
                if (p.Item2 == null || !Item2.Equals(p.Item2)) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            if (Item1 != null)
                hash ^= Item1.GetHashCode();
            if (Item2 != null)
                hash ^= Item2.GetHashCode();
            return hash;
        }
    }

    // Kanglai: Using Dictionary rather than List!
    internal class TypeMappingCollection : Dictionary<Tuple<Type, string>, Type>
    {
        public Type this[Type from, string name = null]
        {
            get
            {
                Tuple<Type, string> key = new Tuple<Type, string>(from, name);
                Type mapping = null;
                if (this.TryGetValue(key, out mapping))
                {
                    return mapping;
                }

                return null;
            }
            set
            {
                Tuple<Type, string> key = new Tuple<Type, string>(from, name);
                this[key] = value;
            }
        }
    }

    internal class TypeInstanceCollection : Dictionary<Tuple<Type, string>, object>
    {
        public object this[Type from, string name = null]
        {
            get
            {
                Tuple<Type, string> key = new Tuple<Type, string>(from, name);
                object mapping = null;
                if (this.TryGetValue(key, out mapping))
                {
                    return mapping;
                }

                return null;
            }
            set
            {
                Tuple<Type, string> key = new Tuple<Type, string>(from, name);
                this[key] = value;
            }
        }
    }

    internal class TypeRelationCollection : Dictionary<Tuple<Type, Type>, Type>
    {
        public Type this[Type from, Type to]
        {
            get
            {
                Tuple<Type, Type> key = new Tuple<Type, Type>(from, to);
                Type mapping = null;
                if (this.TryGetValue(key, out mapping))
                {
                    return mapping;
                }

                return null;
            }
            set
            {
                Tuple<Type, Type> key = new Tuple<Type, Type>(from, to);
                this[key] = value;
            }
        }
    }
}