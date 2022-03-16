using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;


namespace QFramework
{
    [InitializeOnLoad]
    public sealed class EasyIMGUI
    {
        public static ILabel Label()
        {
            return new Label();
        }


        public static IButton Button()
        {
            return new IMGUIButton();
        }

        public static ISpace Space()
        {
            return new Space();
        }

        public static IFlexibleSpace FlexibleSpace()
        {
            return new FlexibleSpace();
        }

        public static ITextField TextField()
        {
            return new TextField();
        }

        public static ITextArea TextArea()
        {
            return new TextArea();
        }

        public static ICustom Custom()
        {
            return new CustomView();
        }

        public static IToggle Toggle()
        {
            return new IMGUIToggle();
        }

        public static IBox Box()
        {
            return new BoxView();
        }

        public static IToolbar Toolbar()
        {
            return new ToolbarView();
        }

        public static IVerticalLayout Vertical()
        {
            return new VerticalLayout();
        }

        public static IHorizontalLayout Horizontal()
        {
            return new HorizontalLayout();
        }

        public static IScrollLayout Scroll()
        {
            return new ScrollLayout();
        }

        public static IAreaLayout Area()
        {
            return new AreaLayout();
        }


        public static IXMLView XMLView()
        {
            return new XMLView();
        }

        public static ILabelWithRect LabelWithRect()
        {
            return new LabelWithRect();
        }

        public static IBoxWithRect BoxWithRect()
        {
            return new BoxWithRect();
        }

        static EasyIMGUI()
        {
            XMLKit.Get.SystemLayer.Get<IXMLToObjectConvertSystem>()
                .AddModule("EasyIMGUI", new EasyIMGUIXMLModule());
        }
    }

    public interface IMGUILayoutRoot
    {
        VerticalLayout Layout { get; set; }

        RenderEndCommandExecutor RenderEndCommandExecutor { get; set; }
    }

    public interface IBox : IMGUIView, IHasText<IBox>
    {
    }

    public class BoxView : View, IBox
    {
        public BoxView()
        {
            mStyleProperty = new GUIStyleProperty(() =>
            {
                // Box 的颜色保持和文本的颜色一致
                var boxStyle = new GUIStyle(GUI.skin.box) { normal = { textColor = GUI.skin.label.normal.textColor } };
                return boxStyle;
            });
        }

        protected override void OnGUI()
        {
            GUILayout.Box(mText, mStyleProperty.Value, LayoutStyles);
        }

        private string mText = string.Empty;

        public IBox Text(string labelText)
        {
            mText = labelText;
            return this;
        }
    }

    public class GUIStyleProperty
    {
        private readonly Func<GUIStyle> mCreator;


        private Action<GUIStyle> mOperations = (style) => { };

        public GUIStyleProperty Set(Action<GUIStyle> operation)
        {
            mOperations += operation;
            return this;
        }

        public GUIStyleProperty(Func<GUIStyle> creator)
        {
            mCreator = creator;
        }

        private GUIStyle mValue = null;

        public GUIStyle Value
        {
            get
            {
                if (mValue != null) return mValue;
                mValue = mCreator.Invoke();
                mOperations(mValue);

                return mValue;
            }
            set { mValue = value; }
        }
    }

    public interface IBoxWithRect : IMGUIView, IHasRect<IBoxWithRect>
    {
        IBoxWithRect Text(string text);
    }

    public class BoxWithRect : View, IBoxWithRect
    {
        public BoxWithRect()
        {
            mStyleProperty = new GUIStyleProperty(() => GUI.skin.box);
        }

        private Rect mRect = new Rect(0, 0, 200, 100);

        private string mText = string.Empty;

        protected override void OnGUI()
        {
            GUI.Box(mRect, mText, mStyleProperty.Value);
        }

        public IBoxWithRect Text(string labelText)
        {
            mText = labelText;

            return this;
        }

        public T Convert<T>(XmlNode node) where T : class
        {
            throw new System.NotImplementedException();
        }

        public IBoxWithRect Rect(Rect rect)
        {
            mRect = rect;
            return this;
        }

        public IBoxWithRect Position(Vector2 position)
        {
            mRect.position = position;
            return this;
        }

        public IBoxWithRect Position(float x, float y)
        {
            mRect.x = x;
            mRect.y = y;
            return this;
        }

        public IBoxWithRect Size(float width, float height)
        {
            mRect.width = width;
            mRect.height = height;
            return this;
        }

        public IBoxWithRect Size(Vector2 size)
        {
            mRect.size = size;
            return this;
        }

        public IBoxWithRect Width(float width)
        {
            mRect.width = width;
            return this;
        }

        public IBoxWithRect Height(float height)
        {
            mRect.height = height;
            return this;
        }
    }

    public interface ILabelWithRect : IMGUIView, IHasText<ILabelWithRect>, IXMLToObjectConverter,
        IHasRect<ILabelWithRect>
    {
    }

    public class LabelWithRect : View, ILabelWithRect
    {
        public LabelWithRect()
        {
            mStyleProperty = new GUIStyleProperty(() => new GUIStyle(GUI.skin.label));
        }

        private string mText = string.Empty;
        private Rect mRect = new Rect(0, 0, 200, 100);

        protected override void OnGUI()
        {
            GUI.Label(mRect, mText, mStyleProperty.Value);
        }

        public ILabelWithRect Text(string labelText)
        {
            mText = labelText;

            return this;
        }

        public T Convert<T>(XmlNode node) where T : class
        {
            throw new System.NotImplementedException();
        }

        public ILabelWithRect Rect(Rect rect)
        {
            mRect = rect;
            return this;
        }

        public ILabelWithRect Position(Vector2 position)
        {
            mRect.position = position;
            return this;
        }

        public ILabelWithRect Position(float x, float y)
        {
            mRect.x = x;
            mRect.y = y;
            return this;
        }

        public ILabelWithRect Size(float width, float height)
        {
            mRect.width = width;
            mRect.height = height;
            return this;
        }

        public ILabelWithRect Size(Vector2 size)
        {
            mRect.size = size;
            return this;
        }

        public ILabelWithRect Width(float width)
        {
            mRect.width = width;
            return this;
        }

        public ILabelWithRect Height(float height)
        {
            mRect.height = height;
            return this;
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

    public interface IToolbar : IMGUIView
    {
        IToolbar Menus(List<string> menuNames);
        IToolbar AddMenu(string name, Action<string> onMenuSelected = null);

        BindableProperty<int> IndexProperty { get; }

        IToolbar Index(int index);
    }

    internal class ToolbarView : View, IToolbar
    {
        public ToolbarView()
        {
            IndexProperty = new BindableProperty<int>(0);

            IndexProperty.Register(index =>
            {
                if (MenuSelected.Count > index)
                {
                    MenuSelected[index].Invoke(MenuNames[index]);
                }
            });

            Style = new GUIStyleProperty(() => GUI.skin.button);
        }

        public IToolbar Menus(List<string> menuNames)
        {
            this.MenuNames = menuNames;
            // empty
            this.MenuSelected = MenuNames.Select(menuName => new Action<string>((str => { }))).ToList();
            return this;
        }

        public IToolbar AddMenu(string name, Action<string> onMenuSelected = null)
        {
            MenuNames.Add(name);
            if (onMenuSelected == null)
            {
                MenuSelected.Add((item) => { });
            }
            else
            {
                MenuSelected.Add(onMenuSelected);
            }

            return this;
        }

        List<string> MenuNames = new List<string>();

        List<Action<string>> MenuSelected = new List<Action<string>>();

        public BindableProperty<int> IndexProperty { get; private set; }

        public IToolbar Index(int index)
        {
            IndexProperty.Value = index;
            return this;
        }

        protected override void OnGUI()
        {
            IndexProperty.Value =
                GUILayout.Toolbar(IndexProperty.Value, MenuNames.ToArray(), Style.Value, LayoutStyles);
        }
    }

    public interface IToggle : IMGUIView, IHasText<IToggle>
    {
        BindableProperty<bool> ValueProperty { get; }

        IToggle IsOn(bool isOn);
        IToggle IsOn(Func<bool> isOngetter);
    }

    internal class IMGUIToggle : View, IToggle
    {
        private Func<bool> mIsOnGetter;
        private string mText { get; set; }

        public IMGUIToggle()
        {
            ValueProperty = new BindableProperty<bool>(false);

            Style = new GUIStyleProperty(() => GUI.skin.toggle);
        }

        public BindableProperty<bool> ValueProperty { get; private set; }

        public IToggle IsOn(bool isOn)
        {
            ValueProperty.Value = isOn;
            return this;
        }

        public IToggle IsOn(Func<bool> isOnGetter)
        {
            mIsOnGetter = isOnGetter;
            return this;
        }

        protected override void OnGUI()
        {
            ValueProperty.Value =
                GUILayout.Toggle(mIsOnGetter?.Invoke() ?? ValueProperty.Value, mText ?? string.Empty, Style.Value,
                    LayoutStyles);
        }

        public IToggle Text(string text)
        {
            mText = text;
            return this;
        }
    }

    [Serializable]
    public abstract class ReorderableArray<T> : ICloneable, IList<T>, ICollection<T>, IEnumerable<T>
    {
        [SerializeField] private List<T> array = new List<T>();

        public ReorderableArray()
            : this(0)
        {
        }

        public ReorderableArray(int length)
        {
            array = new List<T>(length);
        }

        public T this[int index]
        {
            get { return array[index]; }
            set { array[index] = value; }
        }

        public int Length
        {
            get { return array.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public int Count
        {
            get { return array.Count; }
        }

        public object Clone()
        {
            return new List<T>(array);
        }

        public void CopyFrom(IEnumerable<T> value)
        {
            array.Clear();
            array.AddRange(value);
        }

        public bool Contains(T value)
        {
            return array.Contains(value);
        }

        public int IndexOf(T value)
        {
            return array.IndexOf(value);
        }

        public void Insert(int index, T item)
        {
            array.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            array.RemoveAt(index);
        }

        public void Add(T item)
        {
            array.Add(item);
        }

        public void Clear()
        {
            array.Clear();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.array.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return array.Remove(item);
        }

        public T[] ToArray()
        {
            return array.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return array.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return array.GetEnumerator();
        }
    }

    public class ReorderableAttribute : PropertyAttribute
    {
        public bool add;
        public bool remove;
        public bool draggable;
        public bool singleLine;
        public string elementNameProperty;
        public string elementNameOverride;
        public string elementIconPath;

        public ReorderableAttribute()
            : this(null)
        {
        }

        public ReorderableAttribute(string elementNameProperty)
            : this(true, true, true, elementNameProperty, null, null)
        {
        }

        public ReorderableAttribute(string elementNameProperty, string elementIconPath)
            : this(true, true, true, elementNameProperty, null, elementIconPath)
        {
        }

        public ReorderableAttribute(string elementNameProperty, string elementNameOverride, string elementIconPath)
            : this(true, true, true, elementNameProperty, elementNameOverride, elementIconPath)
        {
        }

        public ReorderableAttribute(bool add, bool remove, bool draggable, string elementNameProperty = null,
            string elementIconPath = null)
            : this(add, remove, draggable, elementNameProperty, null, elementIconPath)
        {
        }

        public ReorderableAttribute(bool add, bool remove, bool draggable, string elementNameProperty = null,
            string elementNameOverride = null, string elementIconPath = null)
        {
            this.add = add;
            this.remove = remove;
            this.draggable = draggable;
            this.elementNameProperty = elementNameProperty;
            this.elementNameOverride = elementNameOverride;
            this.elementIconPath = elementIconPath;
        }
    }

    public interface ITextArea : IMGUIView, IHasText<ITextArea>, IXMLToObjectConverter
    {
        BindableProperty<string> Content { get; }
    }

    internal class TextArea : View, ITextArea
    {
        public TextArea()
        {
            Content = new BindableProperty<string>(string.Empty);
            mStyleProperty = new GUIStyleProperty(() => GUI.skin.textArea);
        }

        public BindableProperty<string> Content { get; private set; }

        protected override void OnGUI()
        {
            Content.Value = CrossPlatformGUILayout.TextArea(Content.Value, mStyleProperty.Value, LayoutStyles);
        }

        public ITextArea Text(string labelText)
        {
            Content.Value = labelText;
            return this;
        }


        public T Convert<T>(XmlNode node) where T : class
        {
            var textArea = EasyIMGUI.TextArea();

            foreach (XmlAttribute nodeAttribute in node.Attributes)
            {
                if (nodeAttribute.Name == "Id")
                {
                    textArea.Id = nodeAttribute.Value;
                }
                else if (nodeAttribute.Name == "Text")
                {
                    textArea.Text(nodeAttribute.Value);
                }
            }

            return textArea as T;
        }
    }

    public interface ITextField : IMGUIView, IHasText<ITextField>, IXMLToObjectConverter
    {
        BindableProperty<string> Content { get; }

        ITextField PasswordMode();
    }

    public class TextField : View, ITextField
    {
        public TextField()
        {
            Content = new BindableProperty<string>(string.Empty);

            mStyleProperty = new GUIStyleProperty(() => GUI.skin.textField);
        }

        public BindableProperty<string> Content { get; private set; }

        protected override void OnGUI()
        {
            if (mPasswordMode)
            {
                Content.Value = CrossPlatformGUILayout.PasswordField(Content.Value, Style.Value, LayoutStyles);
            }
            else
            {
                Content.Value = CrossPlatformGUILayout.TextField(Content.Value, Style.Value, LayoutStyles);
            }
        }


        private bool mPasswordMode = false;

        public ITextField PasswordMode()
        {
            mPasswordMode = true;
            return this;
        }

        public ITextField Text(string labelText)
        {
            Content.Value = labelText;
            return this;
        }

        public T Convert<T>(XmlNode node) where T : class
        {
            var textArea = EasyIMGUI.TextField();

            foreach (XmlAttribute nodeAttribute in node.Attributes)
            {
                if (nodeAttribute.Name == "Id")
                {
                    textArea.Id = nodeAttribute.Value;
                }
                else if (nodeAttribute.Name == "Text")
                {
                    textArea.Text(nodeAttribute.Value);
                }
            }

            return textArea as T;
        }
    }

    public interface IFlexibleSpace : IMGUIView, IXMLToObjectConverter
    {
    }

    internal class FlexibleSpace : View, IFlexibleSpace
    {
        protected override void OnGUI()
        {
            GUILayout.FlexibleSpace();
        }

        public T Convert<T>(XmlNode node) where T : class
        {
            var flexibleSpace = EasyIMGUI.FlexibleSpace();

            foreach (XmlAttribute childNodeAttribute in node.Attributes)
            {
                if (childNodeAttribute.Name == "Id")
                {
                    flexibleSpace.Id = childNodeAttribute.Value;
                }
            }

            return flexibleSpace as T;
        }
    }

    public interface ISpace : IMGUIView, IXMLToObjectConverter
    {
        ISpace Pixel(int pixel);
    }

    internal class Space : View, ISpace
    {
        private int mPixel = 10;

        protected override void OnGUI()
        {
            GUILayout.Space(mPixel);
        }

        public ISpace Pixel(int pixel)
        {
            mPixel = pixel;
            return this;
        }

        public T Convert<T>(XmlNode node) where T : class
        {
            var space = EasyIMGUI.Space();

            foreach (XmlAttribute nodeAttribute in node.Attributes)
            {
                if (nodeAttribute.Name == "Id")
                {
                    space.Id = nodeAttribute.Value;
                }
                else if (nodeAttribute.Name == "Pixel")
                {
                    space.Pixel(int.Parse(nodeAttribute.Value));
                }
            }

            return space as T;
        }
    }

    public interface ILabel : IMGUIView, IHasText<ILabel>, IHasTextGetter<ILabel>, IXMLToObjectConverter
    {
    }

    internal class Label : View, ILabel
    {
        public string Content { get; set; }

        public Label()
        {
            mStyleProperty = new GUIStyleProperty(() => new GUIStyle(GUI.skin.label));
        }

        protected override void OnGUI()
        {
            GUILayout.Label(mTextGetter == null ? Content : mTextGetter(), Style.Value, LayoutStyles);
        }

        public ILabel Text(string labelText)
        {
            Content = labelText;
            return this;
        }

        public ILabel TextGetter(Func<string> textGetter)
        {
            mTextGetter = textGetter;
            return this;
        }

        private Func<string> mTextGetter;

        public T Convert<T>(XmlNode node) where T : class
        {
            var label = EasyIMGUI.Label();

            foreach (XmlAttribute childNodeAttribute in node.Attributes)
            {
                if (childNodeAttribute.Name == "Id")
                {
                    label.Id = childNodeAttribute.Value;
                }
                else if (childNodeAttribute.Name == "Text")
                {
                    label.Text(childNodeAttribute.Value);
                }
                else if (childNodeAttribute.Name == "FontBold")
                {
                    label.FontBold();
                }
                else if (childNodeAttribute.Name == "FontSize")
                {
                    label.FontSize(int.Parse(childNodeAttribute.Value));
                }
            }

            return label as T;
        }
    }

    public interface ICustom : IMGUIView, IXMLToObjectConverter
    {
        ICustom OnGUI(Action onGUI);
    }

    internal class CustomView : View, ICustom
    {
        private Action mOnGUIAction { get; set; }

        protected override void OnGUI()
        {
            mOnGUIAction.Invoke();
        }

        public ICustom OnGUI(Action onGUI)
        {
            mOnGUIAction = onGUI;
            return this;
        }

        public T Convert<T>(XmlNode node) where T : class
        {
            var custom = EasyIMGUI.Custom();

            foreach (XmlAttribute nodeAttribute in node.Attributes)
            {
                if (nodeAttribute.Name == "Id")
                {
                    custom.Id = nodeAttribute.Value;
                }
            }

            return custom as T;
        }
    }

    public interface IButton : IMGUIView,
        IHasText<IButton>,
        IHasTextGetter<IButton>,
        ICanClick<IButton>,
        IXMLToObjectConverter
    {
    }

    internal class IMGUIButton : View, IButton
    {
        private string mLabelText = string.Empty;
        private Action mOnClick = () => { };

        protected override void OnGUI()
        {
            if (GUILayout.Button(mTextGetter == null ? mLabelText : mTextGetter(), GUI.skin.button, LayoutStyles))
            {
                mOnClick.Invoke();
                // GUIUtility.ExitGUI();
            }
        }

        public IButton Text(string labelText)
        {
            mLabelText = labelText;
            return this;
        }


        public IButton OnClick(Action action)
        {
            mOnClick = action;
            return this;
        }

        public T Convert<T>(XmlNode node) where T : class
        {
            var button = EasyIMGUI.Button();

            foreach (XmlAttribute childNodeAttribute in node.Attributes)
            {
                if (childNodeAttribute.Name == "Id")
                {
                    button.Id = childNodeAttribute.Value;
                }
                else if (childNodeAttribute.Name == "Text")
                {
                    button.Text(childNodeAttribute.Value);
                }
                else if (childNodeAttribute.Name == "Width")
                {
                    button.Width(int.Parse(childNodeAttribute.Value));
                }
            }

            return button as T;
        }


        private Func<string> mTextGetter;

        public IButton TextGetter(Func<string> textGetter)
        {
            mTextGetter = textGetter;
            return this;
        }
    }

    public interface IVerticalLayout : IMGUILayout, IXMLToObjectConverter
    {
        IVerticalLayout Box();
    }

    public class VerticalLayout : Layout, IVerticalLayout
    {
        public VerticalLayout()
        {
        }

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

        public IVerticalLayout Box()
        {
            VerticalStyle = "box";
            return this;
        }


        public T Convert<T>(XmlNode node) where T : class
        {
            var scroll = EasyIMGUI.Vertical();

            foreach (XmlAttribute childNodeAttribute in node.Attributes)
            {
                if (childNodeAttribute.Name == "Id")
                {
                    scroll.Id = childNodeAttribute.Value;
                }
            }

            return scroll as T;
        }
    }

    public interface IScrollLayout : IMGUILayout, IXMLToObjectConverter
    {
    }

    internal class ScrollLayout : Layout, IScrollLayout
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

        public T Convert<T>(XmlNode node) where T : class
        {
            var scroll = EasyIMGUI.Scroll();

            foreach (XmlAttribute childNodeAttribute in node.Attributes)
            {
                if (childNodeAttribute.Name == "Id")
                {
                    scroll.Id = childNodeAttribute.Value;
                }
            }

            return scroll as T;
        }
    }

    public interface IHorizontalLayout : IMGUILayout, IXMLToObjectConverter
    {
        IHorizontalLayout Box();
    }

    public class HorizontalLayout : Layout, IHorizontalLayout
    {
        public string HorizontalStyle { get; set; }


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

        public IHorizontalLayout Box()
        {
            HorizontalStyle = "box";
            return this;
        }

        public T Convert<T>(XmlNode node) where T : class
        {
            var horizontal = EasyIMGUI.Horizontal();

            foreach (XmlAttribute childNodeAttribute in node.Attributes)
            {
                if (childNodeAttribute.Name == "Id")
                {
                    horizontal.Id = childNodeAttribute.Value;
                }
            }

            return horizontal as T;
        }
    }

    public interface IAreaLayout : IMGUILayout
    {
        IAreaLayout WithRect(Rect rect);
        IAreaLayout WithRectGetter(Func<Rect> rectGetter);
    }

    public class AreaLayout : Layout, IAreaLayout
    {
        private Rect mRect;
        private Func<Rect> mRectGetter;

        public IAreaLayout WithRect(Rect rect)
        {
            mRect = rect;
            return this;
        }

        public IAreaLayout WithRectGetter(Func<Rect> rectGetter)
        {
            mRectGetter = rectGetter;
            return this;
        }

        protected override void OnGUIBegin()
        {
            GUILayout.BeginArea(mRectGetter == null ? mRect : mRectGetter());
        }

        protected override void OnGUIEnd()
        {
            GUILayout.EndArea();
        }
    }

    public abstract class View : IMGUIView
    {
        private bool mVisible = true;

        public string Id { get; set; }

        public bool Visible
        {
            get { return VisibleCondition == null ? mVisible : VisibleCondition(); }
            set { mVisible = value; }
        }

        public Func<bool> VisibleCondition { get; set; }

        private readonly List<GUILayoutOption> mPrivateLayoutOptions = new List<GUILayoutOption>();

        private List<GUILayoutOption> mLayoutOptions
        {
            get { return mPrivateLayoutOptions; }
        }

        protected GUILayoutOption[] LayoutStyles { get; private set; }


        protected GUIStyleProperty mStyleProperty = new GUIStyleProperty(() => new GUIStyle());

        public GUIStyleProperty Style
        {
            get { return mStyleProperty; }
            protected set { mStyleProperty = value; }
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

            if (mCommands.Count > 0)
            {
                mCommands.Dequeue().Invoke();
            }
        }

        protected void PushCommand(Action command)
        {
            mCommands.Enqueue(command);
        }

        Queue<Action> mCommands = new Queue<Action>();

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

        public IMGUILayout Parent { get; set; }

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
            OnDisposed();
        }

        protected virtual void OnDisposed()
        {
        }
    }

    public abstract class Layout : View, IMGUILayout
    {
        protected List<IMGUIView> Children = new List<IMGUIView>();

        public IMGUILayout AddChild(IMGUIView view)
        {
            Children.Add(view);
            view.Parent = this;
            return this;
        }

        public void RemoveChild(IMGUIView view)
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
            this.Children.ForEach(view =>
            {
                view.Parent = null;
                view.Dispose();
            });

            this.Children.Clear();
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

        protected virtual void OnGUIBegin()
        {
        }

        protected virtual void OnGUIEnd()
        {
        }
    }

    public interface IMGUIView : IDisposable
    {
        string Id { get; set; }
        bool Visible { get; set; }

        Func<bool> VisibleCondition { get; set; }

        void DrawGUI();

        IMGUILayout Parent { get; set; }

        GUIStyleProperty Style { get; }

        Color BackgroundColor { get; set; }

        void RefreshNextFrame();

        void AddLayoutOption(GUILayoutOption option);

        void RemoveFromParent();

        void Refresh();

        void Hide();
        void Show();
    }

    public interface IMGUILayout : IMGUIView
    {
        IMGUILayout AddChild(IMGUIView view);

        void RemoveChild(IMGUIView view);

        void Clear();
    }

    public interface ICanClick<T>
    {
        T OnClick(Action action);
    }

    public interface IHasRect<T>
    {
        T Rect(Rect rect);
        T Position(Vector2 position);
        T Position(float x, float y);
        T Size(float width, float height);
        T Size(Vector2 size);
        T Width(float width);
        T Height(float height);
    }

    public interface IHasText<T>
    {
        T Text(string labelText);
    }

    public interface IHasTextGetter<T>
    {
        T TextGetter(Func<string> textGetter);
    }

    public static class IMGUILayoutRootExtension
    {
        public static IMGUILayout GetLayout(this IMGUILayoutRoot self)
        {
            if (self.Layout == null)
            {
                self.Layout = new VerticalLayout();
            }

            return self.Layout;
        }

        public static void AddChild(this IMGUILayoutRoot self, IMGUIView child)
        {
            self.GetLayout().AddChild(child);
        }

        public static void RemoveChild(this IMGUILayoutRoot self, IMGUIView child)
        {
            self.GetLayout().RemoveChild(child);
        }


        public static RenderEndCommandExecutor GetCommandExecutor(this IMGUILayoutRoot self)
        {
            if (self.RenderEndCommandExecutor == null)
            {
                self.RenderEndCommandExecutor = new RenderEndCommandExecutor();
            }

            return self.RenderEndCommandExecutor;
        }

        public static void PushRenderEndCommand(this IMGUILayoutRoot self, Action command)
        {
            self.GetCommandExecutor().Push(command);
        }

        public static void ExecuteRenderEndCommand(this IMGUILayoutRoot self)
        {
            self.GetCommandExecutor().Execute();
        }
    }

    public static class LayoutExtension
    {
        public static T Parent<T>(this T view, IMGUILayout parent) where T : IMGUIView
        {
            parent.AddChild(view);
            return view;
        }
    }

    public static class ViewExtension
    {

        public static T Width<T>(this T view, float width) where T : IMGUIView
        {
            view.AddLayoutOption(GUILayout.Width(width));
            return view;
        }

        public static T Height<T>(this T view, float height) where T : IMGUIView
        {
            view.AddLayoutOption(GUILayout.Height(height));
            return view;
        }

        public static T MaxHeight<T>(this T view, float height) where T : IMGUIView
        {
            view.AddLayoutOption(GUILayout.MaxHeight(height));
            return view;
        }

        public static T MinHeight<T>(this T view, float height) where T : IMGUIView
        {
            view.AddLayoutOption(GUILayout.MinHeight(height));
            return view;
        }

        public static T ExpandHeight<T>(this T view) where T : IMGUIView
        {
            view.AddLayoutOption(GUILayout.ExpandHeight(true));
            return view;
        }


        public static T TextMiddleLeft<T>(this T view) where T : IMGUIView
        {
            view.Style.Set(style => style.alignment = TextAnchor.MiddleLeft);
            return view;
        }

        public static T TextMiddleRight<T>(this T view) where T : IMGUIView
        {
            view.Style.Set(style => style.alignment = TextAnchor.MiddleRight);
            return view;
        }

        public static T TextLowerRight<T>(this T view) where T : IMGUIView
        {
            view.Style.Set(style => style.alignment = TextAnchor.LowerRight);
            return view;
        }

        public static T TextMiddleCenter<T>(this T view) where T : IMGUIView
        {
            view.Style.Set(style => style.alignment = TextAnchor.MiddleCenter);
            return view;
        }

        public static T TextLowerCenter<T>(this T view) where T : IMGUIView
        {
            view.Style.Set(style => style.alignment = TextAnchor.LowerCenter);
            return view;
        }

        public static T Color<T>(this T view, Color color) where T : IMGUIView
        {
            view.BackgroundColor = color;
            return view;
        }

        public static T FontColor<T>(this T view, Color color) where T : IMGUIView
        {
            view.Style.Set(style => style.normal.textColor = color);
            return view;
        }
        
        /// <summary>
        /// Change Font Color After Build Any Time
        /// </summary>
        /// <param name="view"></param>
        /// <param name="color"></param>
        /// <typeparam name="T"></typeparam>
        public static void ChangeFontColor<T>(this T view, Color color) where T : IMGUIView
        {
            view.Style.Value.normal.textColor = color;
        }
        
        public static T FontBold<T>(this T view) where T : IMGUIView
        {
            view.Style.Set(style => style.fontStyle = FontStyle.Bold);
            return view;
        }

        public static T FontNormal<T>(this T view) where T : IMGUIView
        {
            view.Style.Set(style => style.fontStyle = FontStyle.Normal);
            return view;
        }

        public static T FontSize<T>(this T view, int fontSize) where T : IMGUIView
        {
            view.Style.Set(style => style.fontSize = fontSize);
            return view;
        }

        public static T Visible<T>(this T view, bool visible) where T : IMGUIView
        {
            view.Visible = visible;
            return view;
        }

        public static T WithVisibleCondition<T>(this T view, Func<bool> visibleCondition) where T : IMGUIView
        {
            view.VisibleCondition = visibleCondition;
            return view;
        }
    }

    public class CrossPlatformGUILayout
    {
        public static string PasswordField(string value, GUIStyle style, GUILayoutOption[] options)
        {
#if UNITY_EDITOR
            return EditorGUILayout.PasswordField(value, style, options);
#else
            return GUILayout.PasswordField(value, '*', style, options);
#endif
        }

        public static string TextField(string value, GUIStyle style, GUILayoutOption[] options)
        {
#if UNITY_EDITOR

            return EditorGUILayout.TextField(value, style, options);
#else
            return GUILayout.TextField(value, style, options);
#endif
        }

        public static string TextArea(string value, GUIStyle style, GUILayoutOption[] options)
        {
#if UNITY_EDITOR

            return EditorGUILayout.TextArea(value, style, options);
#else
            return GUILayout.TextArea(value, style, options);
#endif
        }
    }

    public class RenderEndCommandExecutor
    {
        // 全局的
        private static RenderEndCommandExecutor mGlobal = new RenderEndCommandExecutor();

        private Queue<System.Action> mPrivateCommands = new Queue<System.Action>();

        private Queue<System.Action> mCommands
        {
            get { return mPrivateCommands; }
        }

        public static void PushCommand(System.Action command)
        {
            mGlobal.Push(command);
        }

        public static void ExecuteCommand()
        {
            mGlobal.Execute();
        }

        public void Push(System.Action command)
        {
            mCommands.Enqueue(command);
        }

        public void Execute()
        {
            while (mCommands.Count > 0)
            {
                mCommands.Dequeue().Invoke();
            }
        }
    }

    // ReSharper disable once InconsistentNaming
    public class EasyIMGUIXMLModule : IConvertModule
    {
        private Dictionary<string, IXMLToObjectConverter> mConverters = new Dictionary<string, IXMLToObjectConverter>();

        public EasyIMGUIXMLModule()
        {
            mConverters.Add("IButton", new IMGUIButton());
            mConverters.Add("ILabel", new Label());
            mConverters.Add("IFlexibleSpace", new FlexibleSpace());
            mConverters.Add("IHorizontal", new HorizontalLayout());
            mConverters.Add("IVertical", new VerticalLayout());
            mConverters.Add("ICustom", new CustomView());
            mConverters.Add("ISpace", new Space());
            mConverters.Add("ITextField", new TextField());
            mConverters.Add("ITextArea", new TextArea());
        }

        public IXMLToObjectConverter GetConverter(string name)
        {
            return mConverters[name];
        }

        public void RegisterConverter(string name, IXMLToObjectConverter converter)
        {
            if (mConverters.ContainsKey(name))
            {
                mConverters[name] = converter;
            }
            else
            {
                mConverters.Add(name, converter);
            }
        }
    }

    public interface IXMLView : IVerticalLayout
    {
        T GetById<T>(string id) where T : class, IMGUIView;

        IXMLView LoadXML(string xmlFilePath);

        IXMLView LoadXMLContent(string xmlContent);
    }

    internal class XMLView : VerticalLayout, IXMLView
    {
        private readonly Dictionary<string, IMGUIView> mIdIndex = new Dictionary<string, IMGUIView>();

        public T GetById<T>(string id) where T : class, IMGUIView
        {
            return mIdIndex[id] as T;
        }

        public IXMLView LoadXML(string xmlFilePath)
        {
            var content = File.ReadAllText(xmlFilePath);
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(content);

            var node = xmlDocument.FirstChild;
            GenView(this, node);

            return this;
        }

        public IXMLView LoadXMLContent(string xmlContent)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlContent);

            var node = xmlDocument.FirstChild;
            GenView(this, node);

            return this;
        }

        void GenView(IMGUIView parentLayout, XmlNode parentNode)
        {
            foreach (XmlNode childNode in parentNode)
            {
                var converter = Converter.GetConverter(childNode.Name);

                var view = converter.Convert<IMGUIView>(childNode);

                var layout = parentLayout as IMGUILayout;

                if (layout != null)
                {
                    layout.AddChild(view);
                }
                else
                {
                    break;
                }

                if (!string.IsNullOrEmpty(view.Id))
                {
                    mIdIndex.Add(view.Id, view);
                }

                GenView(view, childNode);
            }
        }

        IConvertModule Converter
        {
            get
            {
                return mConverter ?? (mConverter = XMLKit.Get.SystemLayer.Get<IXMLToObjectConvertSystem>()
                    .GetConvertModule("EasyIMGUI"));
            }
        }

        private IConvertModule mConverter = null;
    }

    public interface IConvertModule
    {
        IXMLToObjectConverter GetConverter(string name);

        void RegisterConverter(string name, IXMLToObjectConverter converter);
    }

    public interface IXMLToObjectConverter
    {
        T Convert<T>(XmlNode node) where T : class;
    }

    public class CustomXMLToObjectConverter<T> : IXMLToObjectConverter where T : class
    {
        private readonly Func<XmlNode, T> mConverter;

        public CustomXMLToObjectConverter(Func<XmlNode, T> converter)
        {
            mConverter = converter;
        }


        public T1 Convert<T1>(XmlNode node) where T1 : class
        {
            return mConverter(node) as T1;
        }
    }

    public interface IXMLToObjectConvertSystem
    {
        IConvertModule GetConvertModule(string moduleName);
        void AddModule(string key, IConvertModule module);

        bool ContainsModule(string key);
    }

    internal class XMLToObjectConvertSystem : IXMLToObjectConvertSystem
    {
        private Dictionary<string, IConvertModule> mModules =
            new Dictionary<string, IConvertModule>();

        public IConvertModule GetConvertModule(string moduleName)
        {
            return mModules[moduleName];
        }

        public void AddModule(string key, IConvertModule module)
        {
            if (mModules.ContainsKey(key))
            {
                mModules[key] = module;
            }
            else
            {
                mModules.Add(key, module);
            }
        }

        public bool ContainsModule(string key)
        {
            return mModules.ContainsKey(key);
        }
    }

    public class XMLKit
    {
        private static Lazy<XMLKit> mInstance = new Lazy<XMLKit>(() =>
        {
            var xmlKit = new XMLKit();
            xmlKit.Init();
            return xmlKit;
        });

        public static XMLKit Get => mInstance.Value;

        public readonly EditorKitIOCContainer SystemLayer = new EditorKitIOCContainer();

        private void Init()
        {
            SystemLayer.Register<IXMLToObjectConvertSystem>(new XMLToObjectConvertSystem());
        }
    }


    public class EditorKitIOCContainer
    {
        private Dictionary<Type, object> mInstances = new Dictionary<Type, object>();

        public void Register<T>(T instance)
        {
            var key = typeof(T);

            if (mInstances.ContainsKey(key))
            {
                mInstances[key] = instance;
            }
            else
            {
                mInstances.Add(key, instance);
            }
        }

        public T Get<T>() where T : class
        {
            var key = typeof(T);

            if (mInstances.TryGetValue(key, out var retInstance))
            {
                return retInstance as T;
            }

            return null;
        }
    }

    public class EasyInspectorEditor : UnityEditor.Editor, IMGUILayoutRoot
    {
        VerticalLayout IMGUILayoutRoot.Layout { get; set; }
        RenderEndCommandExecutor IMGUILayoutRoot.RenderEndCommandExecutor { get; set; }

        protected void Save()
        {
            EditorUtility.SetDirty(target);
            UnityEditor.SceneManagement.EditorSceneManager
                .MarkSceneDirty(SceneManager.GetActiveScene());
            GUIUtility.ExitGUI();
        }
    }

    public class TreeNode : VerticalLayout
    {
        public BindableProperty<bool> Spread = null;

        public string Content;


        private readonly IHorizontalLayout mFirstLine = EasyIMGUI.Horizontal();

        private VerticalLayout mSpreadView = new VerticalLayout();

        public TreeNode(bool spread, string content, int indent = 0, bool autosaveSpreadState = false)
        {
            if (autosaveSpreadState)
            {
                spread = EditorPrefs.GetBool(content, spread);
            }

            Content = content;
            Spread = new BindableProperty<bool>(spread);

            Style = new GUIStyleProperty(() => EditorStyles.foldout);

            mFirstLine.Parent(this);
            mFirstLine.AddChild(EasyIMGUI.Space().Pixel(indent));

            if (autosaveSpreadState)
            {
                Spread.Register(value => EditorPrefs.SetBool(content, value));
            }


            EasyIMGUI.Custom().OnGUI(() =>
                {
                    Spread.Value = EditorGUILayout.Foldout(Spread.Value, Content, true, Style.Value);
                })
                .Parent(mFirstLine);

            EasyIMGUI.Custom().OnGUI(() =>
            {
                if (Spread.Value)
                {
                    mSpreadView.DrawGUI();
                }
            }).Parent(this);
        }

        public TreeNode Add2FirstLine(IMGUIView view)
        {
            view.Parent(mFirstLine);
            return this;
        }

        public TreeNode FirstLineBox()
        {
            mFirstLine.Box();

            return this;
        }

        public TreeNode SpreadBox()
        {
            mSpreadView.VerticalStyle = "box";

            return this;
        }

        public TreeNode Add2Spread(IMGUIView view)
        {
            view.Parent(mSpreadView);
            return this;
        }
    }

    public abstract class EasyEditorWindow : EditorWindow, IMGUILayoutRoot
    {
        public static T Create<T>(bool utility, string title = null, bool focused = true) where T : EasyEditorWindow
        {
            return string.IsNullOrEmpty(title) ? GetWindow<T>(utility) : GetWindow<T>(utility, title, focused);
        }

        public bool Openning { get; set; }

        public void Open()
        {
            Openning = true;
            EditorApplication.update += OnUpdate;
            Show();
        }

        public new void Close()
        {
            Openning = false;
            base.Close();
        }


        public void RemoveAllChildren()
        {
            this.GetLayout().Clear();
        }

        public abstract void OnClose();


        public abstract void OnUpdate();

        private void OnDestroy()
        {
            EditorApplication.update -= OnUpdate;
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

            this.GetLayout().DrawGUI();
        }

        VerticalLayout IMGUILayoutRoot.Layout { get; set; }
        RenderEndCommandExecutor IMGUILayoutRoot.RenderEndCommandExecutor { get; set; }
    }

    public interface IGenericMenu
    {
    }

    public class GenericMenuView : IGenericMenu
    {
        protected GenericMenuView()
        {
        }

        public static GenericMenuView Create()
        {
            return new GenericMenuView();
        }

        private GenericMenu mMenu = new GenericMenu();

        public GenericMenuView Separator()
        {
            mMenu.AddSeparator(string.Empty);
            return this;
        }

        public GenericMenuView AddMenu(string menuPath, GenericMenu.MenuFunction click)
        {
            mMenu.AddItem(new GUIContent(menuPath), false, click);
            return this;
        }

        public void Show()
        {
            mMenu.ShowAsContext();
        }
    }

    public interface IPopup : IMGUIView
    {
        IPopup WithIndexAndMenus(int index, params string[] menus);

        IPopup OnIndexChanged(Action<int> indexChanged);

        IPopup ToolbarStyle();

        BindableProperty<int> IndexProperty { get; }
        IPopup Menus(List<string> value);
    }

    public class PopupView : View, IPopup
    {
        protected PopupView()
        {
            mStyleProperty = new GUIStyleProperty(() => EditorStyles.popup);
        }

        public static IPopup Create()
        {
            return new PopupView();
        }

        private BindableProperty<int> mIndexProperty = new BindableProperty<int>(0);

        public BindableProperty<int> IndexProperty
        {
            get { return mIndexProperty; }
        }

        public IPopup Menus(List<string> menus)
        {
            mMenus = menus.ToArray();
            return this;
        }

        private string[] mMenus = { };

        protected override void OnGUI()
        {
            IndexProperty.Value =
                EditorGUILayout.Popup(IndexProperty.Value, mMenus, mStyleProperty.Value, LayoutStyles);
        }

        public IPopup WithIndexAndMenus(int index, params string[] menus)
        {
            IndexProperty.Value = index;
            mMenus = menus;
            return this;
        }

        public IPopup OnIndexChanged(Action<int> indexChanged)
        {
            IndexProperty.Register(indexChanged);
            return this;
        }

        public IPopup ToolbarStyle()
        {
            mStyleProperty = new GUIStyleProperty(() => EditorStyles.toolbarPopup);
            return this;
        }
    }

    public class AssetTree
    {
        private TreeNode<AssetData> _root;

        public AssetTree()
        {
            _root = new TreeNode<AssetData>(null);
        }

        public TreeNode<AssetData> Root
        {
            get { return _root; }
        }

        public void Clear()
        {
            _root.Clear();
        }

        public void AddAsset(string guid, HashSet<string> incluedPathes)
        {
            if (string.IsNullOrEmpty(guid)) return;

            TreeNode<AssetData> node = _root;

            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            if (assetPath.StartsWith("Packages")) return;

            int startIndex = 0, length = assetPath.Length;

            var isSelected = incluedPathes.Contains(assetPath);

            var isExpanded = incluedPathes.Any(path => path.Contains(assetPath));

            while (startIndex < length)
            {
                int endIndex = assetPath.IndexOf('/', startIndex);
                int subLength = endIndex == -1 ? length - startIndex : endIndex - startIndex;
                string directory = assetPath.Substring(startIndex, subLength);

                var pathNode = new AssetData(endIndex == -1 ? guid : null, directory,
                    assetPath.Substring(0, endIndex == -1 ? length : endIndex), node.Level == 0 || isExpanded,
                    isSelected);

                var child = node.FindInChildren(pathNode);

                if (child == null) child = node.AddChild(pathNode);

                node = child;
                startIndex += subLength + 1;
            }
        }
    }

    public class AssetData : ITreeIMGUIData
    {
        public readonly string guid;
        public readonly string path;
        public readonly string fullPath;
        public bool isExpanded { get; set; }
        public bool isSelected { get; set; }

        public AssetData(string guid, string path, string fullPath, bool isExpanded, bool isSelected)
        {
            this.guid = guid;
            this.path = path;
            this.fullPath = fullPath;
            this.isExpanded = isExpanded;
            this.isSelected = isSelected;
        }

        public override string ToString()
        {
            return path;
        }

        public override int GetHashCode()
        {
            return path.GetHashCode() + 10;
        }

        public override bool Equals(object obj)
        {
            AssetData node = obj as AssetData;
            return node != null && node.path == path;
        }

        public bool Equals(AssetData node)
        {
            return node.path == path;
        }
    }

    public class AssetTreeIMGUI : TreeIMGUI<AssetData>
    {
        public AssetTreeIMGUI(TreeNode<AssetData> root) : base(root)
        {
        }

        protected override void OnDrawTreeNode(Rect rect, TreeNode<AssetData> node, bool selected, bool focus)
        {
            GUIContent labelContent = new GUIContent(node.Data.path, AssetDatabase.GetCachedIcon(node.Data.fullPath));

            if (!node.IsLeaf)
            {
                node.Data.isExpanded = EditorGUI.Foldout(new Rect(rect.x - 12, rect.y, 12, rect.height),
                    node.Data.isExpanded, GUIContent.none);
            }

            EditorGUI.BeginChangeCheck();
            node.Data.isSelected = EditorGUI.ToggleLeft(rect, labelContent, node.Data.isSelected);
        }
    }

    public class TreeIMGUI<T> where T : ITreeIMGUIData
    {
        private readonly TreeNode<T> _root;

        private Rect _controlRect;
        private float _drawY;
        private float _height;
        private TreeNode<T> _selected;
        private int _controlID;

        public TreeIMGUI(TreeNode<T> root)
        {
            _root = root;
        }

        public void DrawTreeLayout()
        {
            _height = 0;
            _drawY = 0;
            _root.Traverse(OnGetLayoutHeight);

            _controlRect = EditorGUILayout.GetControlRect(false, _height);
            _controlID = GUIUtility.GetControlID(FocusType.Passive, _controlRect);
            _root.Traverse(OnDrawRow);
        }

        protected virtual float GetRowHeight(TreeNode<T> node)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        protected virtual bool OnGetLayoutHeight(TreeNode<T> node)
        {
            if (node.Data == null) return true;

            _height += GetRowHeight(node);
            return node.Data.isExpanded;
        }

        protected virtual bool OnDrawRow(TreeNode<T> node)
        {
            if (node.Data == null) return true;

            float rowIndent = 14 * node.Level;
            float rowHeight = GetRowHeight(node);

            Rect rowRect = new Rect(0, _controlRect.y + _drawY, _controlRect.width, rowHeight);
            Rect indentRect = new Rect(rowIndent, _controlRect.y + _drawY, _controlRect.width - rowIndent, rowHeight);

            // render
            if (_selected == node)
            {
                EditorGUI.DrawRect(rowRect, Color.gray);
            }

            OnDrawTreeNode(indentRect, node, _selected == node, false);

            // test for events
            EventType eventType = Event.current.GetTypeForControl(_controlID);
            if (eventType == EventType.MouseUp && rowRect.Contains(Event.current.mousePosition))
            {
                _selected = node;

                GUI.changed = true;
                Event.current.Use();
            }

            _drawY += rowHeight;

            return node.Data.isExpanded;
        }

        protected virtual void OnDrawTreeNode(Rect rect, TreeNode<T> node, bool selected, bool focus)
        {
            GUIContent labelContent = new GUIContent(node.Data.ToString());

            if (!node.IsLeaf)
            {
                node.Data.isExpanded = EditorGUI.Foldout(new Rect(rect.x - 12, rect.y, 12, rect.height),
                    node.Data.isExpanded, GUIContent.none);
            }

            EditorGUI.LabelField(rect, labelContent, selected ? EditorStyles.whiteLabel : EditorStyles.label);
        }
    }

    public interface ITreeIMGUIData
    {
        bool isExpanded { get; set; }
    }

    public class TreeNode<T>
    {
        public delegate bool TraversalDataDelegate(T data);

        public delegate bool TraversalNodeDelegate(TreeNode<T> node);

        private readonly T _data;
        private readonly TreeNode<T> _parent;
        private readonly int _level;
        private readonly List<TreeNode<T>> _children;

        public TreeNode(T data)
        {
            _data = data;
            _children = new List<TreeNode<T>>();
            _level = 0;
        }

        public TreeNode(T data, TreeNode<T> parent) : this(data)
        {
            _parent = parent;
            _level = _parent != null ? _parent.Level + 1 : 0;
        }

        public int Level
        {
            get { return _level; }
        }

        public int Count
        {
            get { return _children.Count; }
        }

        public bool IsRoot
        {
            get { return _parent == null; }
        }

        public bool IsLeaf
        {
            get { return _children.Count == 0; }
        }

        public T Data
        {
            get { return _data; }
        }

        public TreeNode<T> Parent
        {
            get { return _parent; }
        }

        public TreeNode<T> this[int key]
        {
            get { return _children[key]; }
        }

        public void Clear()
        {
            _children.Clear();
        }

        public TreeNode<T> AddChild(T value)
        {
            TreeNode<T> node = new TreeNode<T>(value, this);
            _children.Add(node);

            return node;
        }

        public bool HasChild(T data)
        {
            return FindInChildren(data) != null;
        }

        public TreeNode<T> FindInChildren(T data)
        {
            int i = 0, l = Count;
            for (; i < l; ++i)
            {
                TreeNode<T> child = _children[i];
                if (child.Data.Equals(data)) return child;
            }

            return null;
        }

        public bool RemoveChild(TreeNode<T> node)
        {
            return _children.Remove(node);
        }

        public void Traverse(TraversalDataDelegate handler)
        {
            if (handler(_data))
            {
                int i = 0, l = Count;
                for (; i < l; ++i) _children[i].Traverse(handler);
            }
        }

        public void Traverse(TraversalNodeDelegate handler)
        {
            if (handler(this))
            {
                int i = 0, l = Count;
                for (; i < l; ++i) _children[i].Traverse(handler);
            }
        }
    }

    [CustomPropertyDrawer(typeof(ReorderableAttribute))]
    public class ReorderableDrawer : PropertyDrawer
    {
        private static Dictionary<int, ReorderableList> lists = new Dictionary<int, ReorderableList>();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ReorderableList list = GetList(property, attribute as ReorderableAttribute);

            return list != null ? list.GetHeight() : EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ReorderableList list = GetList(property, attribute as ReorderableAttribute);

            if (list != null)
            {
                list.DoList(EditorGUI.IndentedRect(position), label);
            }
            else
            {
                GUI.Label(position, "Array must extend from ReorderableArray", EditorStyles.label);
            }
        }

        public static int GetListId(SerializedProperty property)
        {
            if (property != null)
            {
                int h1 = property.serializedObject.targetObject.GetHashCode();
                int h2 = property.propertyPath.GetHashCode();

                return (((h1 << 5) + h1) ^ h2);
            }

            return 0;
        }

        public static ReorderableList GetList(SerializedProperty property)
        {
            return GetList(property, null, GetListId(property));
        }

        public static ReorderableList GetList(SerializedProperty property, ReorderableAttribute attrib)
        {
            return GetList(property, attrib, GetListId(property));
        }

        public static ReorderableList GetList(SerializedProperty property, int id)
        {
            return GetList(property, null, id);
        }

        public static ReorderableList GetList(SerializedProperty property, ReorderableAttribute attrib, int id)
        {
            if (property == null)
            {
                return null;
            }

            ReorderableList list = null;
            SerializedProperty array = property.FindPropertyRelative("array");

            if (array != null && array.isArray)
            {
                if (!lists.TryGetValue(id, out list))
                {
                    if (attrib != null)
                    {
                        Texture icon = !string.IsNullOrEmpty(attrib.elementIconPath)
                            ? AssetDatabase.GetCachedIcon(attrib.elementIconPath)
                            : null;

                        ReorderableList.ElementDisplayType displayType = attrib.singleLine
                            ? ReorderableList.ElementDisplayType.SingleLine
                            : ReorderableList.ElementDisplayType.Auto;

                        list = new ReorderableList(array, attrib.add, attrib.remove, attrib.draggable, displayType,
                            attrib.elementNameProperty, attrib.elementNameOverride, icon);
                    }
                    else
                    {
                        list = new ReorderableList(array, true, true, true);
                    }

                    lists.Add(id, list);
                }
                else
                {
                    list.List = array;
                }
            }

            return list;
        }
    }

    public class ReorderableList
    {
#if UNITY_2019_3_OR_NEWER
		private const float ELEMENT_EDGE_TOP = 1;
		private const float ELEMENT_EDGE_BOT = 2;
#else
        private const float ELEMENT_EDGE_TOP = 1;
        private const float ELEMENT_EDGE_BOT = 3;
#endif
        private const float ELEMENT_HEIGHT_OFFSET = ELEMENT_EDGE_TOP + ELEMENT_EDGE_BOT;

        private static int selectionHash = "ReorderableListSelection".GetHashCode();
        private static int dragAndDropHash = "ReorderableListDragAndDrop".GetHashCode();

        private const string EMPTY_LABEL = "List is Empty";
        private const string ARRAY_ERROR = "{0} is not an Array!";

        public enum ElementDisplayType
        {
            Auto,
            Expandable,
            SingleLine
        }

        public delegate void DrawHeaderDelegate(Rect rect, GUIContent label);

        public delegate void DrawFooterDelegate(Rect rect);

        public delegate void DrawElementDelegate(Rect rect, SerializedProperty element, GUIContent label, bool selected,
            bool focused);

        public delegate void ActionDelegate(ReorderableList list);

        public delegate bool ActionBoolDelegate(ReorderableList list);

        public delegate void AddDropdownDelegate(Rect buttonRect, ReorderableList list);

        public delegate UnityEngine.Object DragDropReferenceDelegate(UnityEngine.Object[] references,
            ReorderableList list);

        public delegate void DragDropAppendDelegate(UnityEngine.Object reference, ReorderableList list);

        public delegate float GetElementHeightDelegate(SerializedProperty element);

        public delegate float GetElementsHeightDelegate(ReorderableList list);

        public delegate string GetElementNameDelegate(SerializedProperty element);

        public delegate GUIContent GetElementLabelDelegate(SerializedProperty element);

        public delegate void SurrogateCallback(SerializedProperty element, UnityEngine.Object objectReference,
            ReorderableList list);

        public event DrawHeaderDelegate drawHeaderCallback;
        public event DrawFooterDelegate drawFooterCallback;
        public event DrawElementDelegate drawElementCallback;
        public event DrawElementDelegate drawElementBackgroundCallback;
        public event GetElementHeightDelegate getElementHeightCallback;
        public event GetElementsHeightDelegate getElementsHeightCallback;
        public event GetElementNameDelegate getElementNameCallback;
        public event GetElementLabelDelegate getElementLabelCallback;
        public event DragDropReferenceDelegate onValidateDragAndDropCallback;
        public event DragDropAppendDelegate onAppendDragDropCallback;
        public event ActionDelegate onReorderCallback;
        public event ActionDelegate onSelectCallback;
        public event ActionDelegate onAddCallback;
        public event AddDropdownDelegate onAddDropdownCallback;
        public event ActionDelegate onRemoveCallback;
        public event ActionDelegate onMouseUpCallback;
        public event ActionBoolDelegate onCanRemoveCallback;
        public event ActionDelegate onChangedCallback;

        public bool canAdd;
        public bool canRemove;
        public bool draggable;
        public bool sortable;
        public bool expandable;
        public bool multipleSelection;
        public GUIContent label;
        public float headerHeight;
        public float footerHeight;
        public float paginationHeight;
        public float slideEasing;
        public float verticalSpacing;
        public bool showDefaultBackground;
        public ElementDisplayType elementDisplayType;
        public string elementNameProperty;
        public string elementNameOverride;
        public bool elementLabels;
        public Texture elementIcon;
        public Surrogate surrogate;

        public bool paginate
        {
            get { return pagination.enabled; }
            set { pagination.enabled = value; }
        }

        public int pageSize
        {
            get { return pagination.fixedPageSize; }
            set { pagination.fixedPageSize = value; }
        }

        internal readonly int id;

        private SerializedProperty list;
        private int controlID = -1;
        private Rect[] elementRects;
        private GUIContent elementLabel;
        private GUIContent pageInfoContent;
        private GUIContent pageSizeContent;
        private ListSelection selection;
        private SlideGroup slideGroup;
        private int pressIndex;

        private bool doPagination
        {
            get { return pagination.enabled && !list.serializedObject.isEditingMultipleObjects; }
        }

        private float elementSpacing
        {
            get { return Mathf.Max(0, verticalSpacing - 2); }
        }

        private bool dragging;
        private float pressPosition;
        private float dragPosition;
        private int dragDirection;
        private DragList dragList;
        private ListSelection beforeDragSelection;
        private Pagination pagination;

        private int dragDropControlID = -1;

        public ReorderableList(SerializedProperty list)
            : this(list, true, true, true)
        {
        }

        public ReorderableList(SerializedProperty list, bool canAdd, bool canRemove, bool draggable)
            : this(list, canAdd, canRemove, draggable, ElementDisplayType.Auto, null, null, null)
        {
        }

        public ReorderableList(SerializedProperty list, bool canAdd, bool canRemove, bool draggable,
            ElementDisplayType elementDisplayType, string elementNameProperty, Texture elementIcon)
            : this(list, canAdd, canRemove, draggable, elementDisplayType, elementNameProperty, null, elementIcon)
        {
        }

        public ReorderableList(SerializedProperty list, bool canAdd, bool canRemove, bool draggable,
            ElementDisplayType elementDisplayType, string elementNameProperty, string elementNameOverride,
            Texture elementIcon)
        {
            if (list == null)
            {
                throw new MissingListExeption();
            }
            else if (!list.isArray)
            {
                //check if user passed in a ReorderableArray, if so, that becomes the list object

                SerializedProperty array = list.FindPropertyRelative("array");

                if (array == null || !array.isArray)
                {
                    throw new InvalidListException();
                }

                this.list = array;
            }
            else
            {
                this.list = list;
            }

            this.canAdd = canAdd;
            this.canRemove = canRemove;
            this.draggable = draggable;
            this.elementDisplayType = elementDisplayType;
            this.elementNameProperty = elementNameProperty;
            this.elementNameOverride = elementNameOverride;
            this.elementIcon = elementIcon;

            id = GetHashCode();
            list.isExpanded = true;
            label = new GUIContent(list.displayName);
            pageInfoContent = new GUIContent();
            pageSizeContent = new GUIContent();

#if UNITY_5_6_OR_NEWER
            verticalSpacing = EditorGUIUtility.standardVerticalSpacing;
#else
			verticalSpacing = 2f;
#endif
            slideEasing = 0.15f;
            expandable = true;
            elementLabels = true;
            showDefaultBackground = true;
            multipleSelection = true;
            pagination = new Pagination();
            elementLabel = new GUIContent();

            dragList = new DragList(0);
            selection = new ListSelection();
            slideGroup = new SlideGroup();
            elementRects = new Rect[0];

            //We can't access Style information yet as GUISkin hasn't loaded, so hard code the values

#if UNITY_2019_3_OR_NEWER
			headerHeight = 20f;
			footerHeight = 20f;
			paginationHeight = 18f;
#else
            headerHeight = 20f;
            footerHeight = 13f;
            paginationHeight = 20f;
#endif
        }

        //
        // -- PROPERTIES --
        //

        public SerializedProperty List
        {
            get { return list; }
            set { list = value; }
        }

        public bool HasList
        {
            get { return list != null && list.isArray; }
        }

        public int Length
        {
            get
            {
                if (!HasList)
                {
                    return 0;
                }
                else if (!list.hasMultipleDifferentValues)
                {
                    return list.arraySize;
                }

                //When multiple objects are selected, because of a Unity bug, list.arraySize is never guranteed to actually be the smallest
                //array size. So we have to find it. Not that great since we're creating SerializedObjects here. There has to be a better way!

                int smallerArraySize = list.arraySize;

                foreach (UnityEngine.Object targetObject in list.serializedObject.targetObjects)
                {
                    SerializedObject serializedObject = new SerializedObject(targetObject);
                    SerializedProperty property = serializedObject.FindProperty(list.propertyPath);

                    smallerArraySize = Mathf.Min(property.arraySize, smallerArraySize);
                }

                return smallerArraySize;
            }
        }

        public int VisibleLength
        {
            get { return pagination.GetVisibleLength(Length); }
        }

        public int[] Selected
        {
            get { return selection.ToArray(); }
            set { selection = new ListSelection(value); }
        }

        public int Index
        {
            get { return selection.First; }
            set { selection.Select(value); }
        }

        public bool IsDragging
        {
            get { return dragging; }
        }

        //
        // -- PUBLIC --
        //

        public float GetHeight()
        {
            if (HasList)
            {
                float topHeight = doPagination ? headerHeight + paginationHeight : headerHeight;

                return list.isExpanded ? topHeight + GetElementsHeight() + footerHeight : headerHeight;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        public float GetElementHeight(int index)
        {
            return index >= 0 && index < Length ? GetElementHeight(list.GetArrayElementAtIndex(index)) : 0;
        }

        public void DoLayoutList()
        {
            Rect position = EditorGUILayout.GetControlRect(false, GetHeight(), EditorStyles.largeLabel);

            DoList(EditorGUI.IndentedRect(position), label);
        }

        public void DoList(Rect rect, GUIContent label)
        {
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect headerRect = rect;
            headerRect.height = headerHeight;

            if (!HasList)
            {
                DrawEmpty(headerRect, string.Format(ARRAY_ERROR, label.text), GUIStyle.none, EditorStyles.helpBox);
            }
            else
            {
                controlID = GUIUtility.GetControlID(selectionHash, FocusType.Keyboard, rect);
                dragDropControlID = GUIUtility.GetControlID(dragAndDropHash, FocusType.Passive, rect);

                DrawHeader(headerRect, label);

                if (list.isExpanded)
                {
                    if (doPagination)
                    {
                        Rect paginateHeaderRect = headerRect;
                        paginateHeaderRect.y += headerRect.height;
                        paginateHeaderRect.height = paginationHeight;

                        DrawPaginationHeader(paginateHeaderRect);

#if UNITY_2019_3_OR_NEWER
						headerRect.yMax = paginateHeaderRect.yMax;
#else
                        headerRect.yMax = paginateHeaderRect.yMax - 1;
#endif
                    }

                    Rect elementBackgroundRect = rect;
                    elementBackgroundRect.yMin = headerRect.yMax;
                    elementBackgroundRect.yMax = rect.yMax - footerHeight;

                    Event evt = Event.current;

                    if (selection.Length > 1)
                    {
                        if (evt.type == EventType.ContextClick && CanSelect(evt.mousePosition))
                        {
                            HandleMultipleContextClick(evt);
                        }
                    }

                    if (Length > 0)
                    {
                        //update element rects if not dragging. Dragging caches draw rects so no need to update

                        if (!dragging)
                        {
                            UpdateElementRects(elementBackgroundRect, evt);
                        }

                        if (elementRects.Length > 0)
                        {
                            int start, end;

                            pagination.GetVisibleRange(elementRects.Length, out start, out end);

                            Rect selectableRect = elementBackgroundRect;
                            selectableRect.yMin = elementRects[start].yMin;
                            selectableRect.yMax = elementRects[end - 1].yMax;

                            HandlePreSelection(selectableRect, evt);
                            DrawElements(elementBackgroundRect, evt);
                            HandlePostSelection(selectableRect, evt);
                        }
                    }
                    else
                    {
                        DrawEmpty(elementBackgroundRect, EMPTY_LABEL, Style.boxBackground, Style.verticalLabel);
                    }

                    Rect footerRect = rect;
                    footerRect.yMin = elementBackgroundRect.yMax;
                    footerRect.xMin = rect.xMax - 58;
                    footerRect.height = footerHeight;

                    DrawFooter(footerRect);
                }
            }

            EditorGUI.indentLevel = indent;
        }

        public SerializedProperty AddItem<T>(T item) where T : UnityEngine.Object
        {
            SerializedProperty property = AddItem();

            if (property != null)
            {
                property.objectReferenceValue = item;
            }

            return property;
        }

        public SerializedProperty AddItem()
        {
            if (HasList)
            {
                list.arraySize++;
                selection.Select(list.arraySize - 1);

                SetPageByIndex(list.arraySize - 1);
                DispatchChange();

                return list.GetArrayElementAtIndex(selection.Last);
            }
            else
            {
                throw new InvalidListException();
            }
        }

        public void Remove(int[] selection)
        {
            System.Array.Sort(selection);

            int i = selection.Length;

            while (--i > -1)
            {
                RemoveItem(selection[i]);
            }
        }

        public void RemoveItem(int index)
        {
            if (index >= 0 && index < Length)
            {
                SerializedProperty property = list.GetArrayElementAtIndex(index);

                if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue)
                {
                    property.objectReferenceValue = null;
                }

                list.DeleteArrayElementAtIndex(index);
                selection.Remove(index);

                if (Length > 0)
                {
                    selection.Select(Mathf.Max(0, index - 1));
                }

                DispatchChange();
            }
        }

        public SerializedProperty GetItem(int index)
        {
            if (index >= 0 && index < Length)
            {
                return list.GetArrayElementAtIndex(index);
            }
            else
            {
                return null;
            }
        }

        public int IndexOf(SerializedProperty element)
        {
            if (element != null)
            {
                int i = Length;

                while (--i > -1)
                {
                    if (SerializedProperty.EqualContents(element, list.GetArrayElementAtIndex(i)))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public void GrabKeyboardFocus()
        {
            GUIUtility.keyboardControl = id;
        }

        public bool HasKeyboardControl()
        {
            return GUIUtility.keyboardControl == id;
        }

        public void ReleaseKeyboardFocus()
        {
            if (GUIUtility.keyboardControl == id)
            {
                GUIUtility.keyboardControl = 0;
            }
        }

        public void SetPage(int page)
        {
            if (doPagination)
            {
                pagination.page = page;
            }
        }

        public void SetPageByIndex(int index)
        {
            if (doPagination)
            {
                pagination.page = pagination.GetPageForIndex(index);
            }
        }

        public int GetPage(int index)
        {
            return doPagination ? pagination.page : 0;
        }

        public int GetPageByIndex(int index)
        {
            return doPagination ? pagination.GetPageForIndex(index) : 0;
        }

        //
        // -- PRIVATE --
        //

        private float GetElementsHeight()
        {
            if (getElementsHeightCallback != null)
            {
                return getElementsHeightCallback(this);
            }

            int i, len = Length;

            if (len == 0)
            {
                return 28;
            }

            float totalHeight = 0;
            float spacing = elementSpacing;

            int start, end;

            pagination.GetVisibleRange(len, out start, out end);

            for (i = start; i < end; i++)
            {
                totalHeight += GetElementHeight(list.GetArrayElementAtIndex(i)) + spacing;
            }

            return totalHeight + 7 - spacing;
        }

        private float GetElementHeight(SerializedProperty element)
        {
            if (getElementHeightCallback != null)
            {
                return getElementHeightCallback(element) + ELEMENT_HEIGHT_OFFSET;
            }
            else
            {
                return EditorGUI.GetPropertyHeight(element, GetElementLabel(element, elementLabels),
                    IsElementExpandable(element)) + ELEMENT_HEIGHT_OFFSET;
            }
        }

        private Rect GetElementDrawRect(int index, Rect desiredRect)
        {
            if (slideEasing <= 0)
            {
                return desiredRect;
            }
            else
            {
                //lerp the drag easing toward slide easing, this creates a stronger easing at the start then slower at the end
                //when dealing with large lists, we can

                return dragging
                    ? slideGroup.GetRect(dragList[index].startIndex, desiredRect, slideEasing)
                    : slideGroup.SetRect(index, desiredRect);
            }
        }

        /*
        private Rect GetElementHeaderRect(SerializedProperty element, Rect elementRect) {

            Rect rect = elementRect;
            rect.height = EditorGUIUtility.singleLineHeight + verticalSpacing;

            return rect;
        }
        */

        private Rect GetElementRenderRect(SerializedProperty element, Rect elementRect)
        {
            float offset = draggable ? 20 : 5;

            Rect rect = elementRect;
            rect.xMin += IsElementExpandable(element) ? offset + 10 : offset;
            rect.xMax -= 5;
            rect.yMin += ELEMENT_EDGE_TOP;
            rect.yMax -= ELEMENT_EDGE_BOT;

            return rect;
        }

        private void DrawHeader(Rect rect, GUIContent label)
        {
            if (showDefaultBackground && Event.current.type == EventType.Repaint)
            {
                Style.headerBackground.Draw(rect, false, false, false, false);
            }

            HandleDragAndDrop(rect, Event.current);

            bool multiline = elementDisplayType != ElementDisplayType.SingleLine;

            Rect titleRect = rect;
            titleRect.xMin += 6f;
            titleRect.xMax -= multiline ? 95f : 55f;

            label = EditorGUI.BeginProperty(titleRect, label, list);

            if (drawHeaderCallback != null)
            {
                drawHeaderCallback(titleRect, label);
            }
            else if (expandable)
            {
                titleRect.xMin += 10;

                EditorGUI.BeginChangeCheck();

                bool isExpanded = EditorGUI.Foldout(titleRect, list.isExpanded, label, true);

                if (EditorGUI.EndChangeCheck())
                {
                    list.isExpanded = isExpanded;
                }
            }
            else
            {
                GUI.Label(titleRect, label, EditorStyles.label);
            }

            EditorGUI.EndProperty();

            if (multiline)
            {
                Rect bRect1 = rect;
                bRect1.xMin = rect.xMax - 25;
                bRect1.xMax = rect.xMax - 5;

                if (GUI.Button(bRect1, Style.expandButton, Style.preButtonStretch))
                {
                    ExpandElements(true);
                }

                Rect bRect2 = rect;
                bRect2.xMin = bRect1.xMin - 20;
                bRect2.xMax = bRect1.xMin;

                if (GUI.Button(bRect2, Style.collapseButton, Style.preButtonStretch))
                {
                    ExpandElements(false);
                }

                rect.xMax = bRect2.xMin + 5;
            }

            //draw sorting options

            if (sortable)
            {
                Rect sortRect1 = rect;
                sortRect1.xMin = rect.xMax - 25;
                sortRect1.xMax = rect.xMax;

                Rect sortRect2 = rect;
                sortRect2.xMin = sortRect1.xMin - 20;
                sortRect2.xMax = sortRect1.xMin;

                if (EditorGUI.DropdownButton(sortRect1, Style.sortAscending, FocusType.Passive, Style.preButtonStretch))
                {
                    SortElements(sortRect1, false);
                }

                if (EditorGUI.DropdownButton(sortRect2, Style.sortDescending, FocusType.Passive,
                        Style.preButtonStretch))
                {
                    SortElements(sortRect2, true);
                }
            }
        }

        private void ExpandElements(bool expand)
        {
            if (!list.isExpanded && expand)
            {
                list.isExpanded = true;
            }

            int i, len = Length;

            for (i = 0; i < len; i++)
            {
                list.GetArrayElementAtIndex(i).isExpanded = expand;
            }
        }

        private void SortElements(Rect rect, bool descending)
        {
            int total = Length;

            //no point in sorting a list with 1 element!

            if (total <= 1)
            {
                return;
            }

            //the first property tells us what type of items are in the list
            //if generic, then we give the user a list of properties to sort on

            SerializedProperty prop = list.GetArrayElementAtIndex(0);

            if (prop.propertyType == SerializedPropertyType.Generic)
            {
                GenericMenu menu = new GenericMenu();

                SerializedProperty property = prop.Copy();
                SerializedProperty end = property.GetEndProperty();

                bool enterChildren = true;

                while (property.NextVisible(enterChildren) && !SerializedProperty.EqualContents(property, end))
                {
                    menu.AddItem(new GUIContent(property.name), false, userData =>
                    {
                        //sort based on the property selected then apply the changes

                        ListSort.SortOnProperty(list, total, descending, (string)userData);

                        ApplyReorder();

                        HandleUtility.Repaint();
                    }, property.name);

                    enterChildren = false;
                }

                menu.DropDown(rect);
            }
            else
            {
                //list is not generic, so we just sort directly on the type then apply the changes

                ListSort.SortOnType(list, total, descending, prop.propertyType);

                ApplyReorder();
            }
        }

        private void DrawEmpty(Rect rect, string label, GUIStyle backgroundStyle, GUIStyle labelStyle)
        {
            if (showDefaultBackground && Event.current.type == EventType.Repaint)
            {
                backgroundStyle.Draw(rect, false, false, false, false);
            }

            EditorGUI.LabelField(rect, label, labelStyle);
        }

        private void UpdateElementRects(Rect rect, Event evt)
        {
            //resize array if elements changed

            int i, len = Length;

            if (len != elementRects.Length)
            {
                System.Array.Resize(ref elementRects, len);
            }

            if (evt.type == EventType.Repaint)
            {
                //start rect

                Rect elementRect = rect;
                elementRect.yMin = elementRect.yMax = rect.yMin + 2;

                float spacing = elementSpacing;

                int start, end;

                pagination.GetVisibleRange(len, out start, out end);

                for (i = start; i < end; i++)
                {
                    SerializedProperty element = list.GetArrayElementAtIndex(i);

                    //update the elementRects value for this object. Grab the last elementRect for startPosition

                    elementRect.y = elementRect.yMax;
                    elementRect.height = GetElementHeight(element);
                    elementRects[i] = elementRect;

                    elementRect.yMax += spacing;
                }
            }
        }

        private void DrawElements(Rect rect, Event evt)
        {
            //draw list background

            if (showDefaultBackground && evt.type == EventType.Repaint)
            {
                Style.boxBackground.Draw(rect, false, false, false, false);
            }

            //if not dragging, draw elements as usual

            if (!dragging)
            {
                int start, end;

                pagination.GetVisibleRange(Length, out start, out end);

                for (int i = start; i < end; i++)
                {
                    bool selected = selection.Contains(i);

                    DrawElement(list.GetArrayElementAtIndex(i), GetElementDrawRect(i, elementRects[i]), selected,
                        selected && GUIUtility.keyboardControl == controlID);
                }
            }
            else if (evt.type == EventType.Repaint)
            {
                //draw dragging elements only when repainting

                int i, s, len = dragList.Length;
                int sLen = selection.Length;

                //first, find the rects of the selected elements, we need to use them for overlap queries

                for (i = 0; i < sLen; i++)
                {
                    DragElement element = dragList[i];

                    //update the element desiredRect if selected. Selected elements appear first in the dragList, so other elements later in iteration will have rects to compare

                    element.desiredRect.y = dragPosition - element.dragOffset;
                    dragList[i] = element;
                }

                //draw elements, start from the bottom of the list as first elements are the ones selected, so should be drawn last

                i = len;

                while (--i > -1)
                {
                    DragElement element = dragList[i];

                    //draw dragging elements last as the loop is backwards

                    if (element.selected)
                    {
                        DrawElement(element.property, element.desiredRect, true, true);
                        continue;
                    }

                    //loop over selection and see what overlaps
                    //if dragging down we start from the bottom of the selection
                    //otherwise we start from the top. This helps to cover multiple selected objects

                    Rect elementRect = element.rect;
                    int elementIndex = element.startIndex;

                    int start = dragDirection > 0 ? sLen - 1 : 0;
                    int end = dragDirection > 0 ? -1 : sLen;

                    for (s = start; s != end; s -= dragDirection)
                    {
                        DragElement selected = dragList[s];

                        if (selected.Overlaps(elementRect, elementIndex, dragDirection))
                        {
                            elementRect.y -= selected.rect.height * dragDirection;
                            elementIndex += dragDirection;
                        }
                    }

                    //draw the element with the new rect

                    DrawElement(element.property, GetElementDrawRect(i, elementRect), false, false);

                    //reassign the element back into the dragList

                    element.desiredRect = elementRect;
                    dragList[i] = element;
                }
            }
        }

        private void DrawElement(SerializedProperty element, Rect rect, bool selected, bool focused)
        {
            Rect backgroundRect = rect;

#if UNITY_2019_3_OR_NEWER
			backgroundRect.xMin++;
			backgroundRect.xMax--;
#endif
            Event evt = Event.current;

            if (drawElementBackgroundCallback != null)
            {
                drawElementBackgroundCallback(backgroundRect, element, null, selected, focused);
            }
            else if (evt.type == EventType.Repaint)
            {
                Style.elementBackground.Draw(backgroundRect, false, selected, selected, focused);
            }

            if (evt.type == EventType.Repaint && draggable)
            {
                Style.draggingHandle.Draw(new Rect(rect.x + 5, rect.y + 6, 10, rect.height - (rect.height - 6)), false,
                    false, false, false);
            }

            GUIContent label = GetElementLabel(element, elementLabels);

            Rect renderRect = GetElementRenderRect(element, rect);

            if (drawElementCallback != null)
            {
                drawElementCallback(renderRect, element, label, selected, focused);
            }
            else
            {
                EditorGUI.PropertyField(renderRect, element, label, true);
            }

            //handle context click

            int controlId = GUIUtility.GetControlID(label, FocusType.Passive, rect);

            switch (evt.GetTypeForControl(controlId))
            {
                case EventType.ContextClick:

                    if (rect.Contains(evt.mousePosition))
                    {
                        HandleSingleContextClick(evt, element);
                    }

                    break;
            }
        }

        private GUIContent GetElementLabel(SerializedProperty element, bool allowElementLabel)
        {
            if (!allowElementLabel)
            {
                return GUIContent.none;
            }
            else if (getElementLabelCallback != null)
            {
                return getElementLabelCallback(element);
            }

            string name;

            if (getElementNameCallback != null)
            {
                name = getElementNameCallback(element);
            }
            else
            {
                name = GetElementName(element, elementNameProperty, elementNameOverride);
            }

            elementLabel.text = !string.IsNullOrEmpty(name) ? name : element.displayName;
            elementLabel.tooltip = element.tooltip;
            elementLabel.image = elementIcon;

            return elementLabel;
        }

        private static string GetElementName(SerializedProperty element, string nameProperty, string nameOverride)
        {
            if (!string.IsNullOrEmpty(nameOverride))
            {
                string path = element.propertyPath;

                const string arrayEndDelimeter = "]";
                const char arrayStartDelimeter = '[';

                if (path.EndsWith(arrayEndDelimeter))
                {
                    int startIndex = path.LastIndexOf(arrayStartDelimeter) + 1;

                    return string.Format("{0} {1}", nameOverride,
                        path.Substring(startIndex, path.Length - startIndex - 1));
                }

                return nameOverride;
            }
            else if (string.IsNullOrEmpty(nameProperty))
            {
                return null;
            }
            else if (element.propertyType == SerializedPropertyType.ObjectReference && nameProperty == "name")
            {
                return element.objectReferenceValue ? element.objectReferenceValue.name : null;
            }

            SerializedProperty prop = element.FindPropertyRelative(nameProperty);

            if (prop != null)
            {
                switch (prop.propertyType)
                {
                    case SerializedPropertyType.ObjectReference:

                        return prop.objectReferenceValue ? prop.objectReferenceValue.name : null;

                    case SerializedPropertyType.Enum:

                        return prop.enumDisplayNames[prop.enumValueIndex];

                    case SerializedPropertyType.Integer:
                    case SerializedPropertyType.Character:

                        return prop.intValue.ToString();

                    case SerializedPropertyType.LayerMask:

                        return GetLayerMaskName(prop.intValue);

                    case SerializedPropertyType.String:

                        return prop.stringValue;

                    case SerializedPropertyType.Float:

                        return prop.floatValue.ToString();
                }

                return prop.displayName;
            }

            return null;
        }

        private static string GetLayerMaskName(int mask)
        {
            if (mask == 0)
            {
                return "Nothing";
            }
            else if (mask < 0)
            {
                return "Everything";
            }

            string name = string.Empty;
            int n = 0;

            for (int i = 0; i < 32; i++)
            {
                if (((1 << i) & mask) != 0)
                {
                    if (n == 4)
                    {
                        return "Mixed ...";
                    }

                    name += (n > 0 ? ", " : string.Empty) + LayerMask.LayerToName(i);
                    n++;
                }
            }

            return name;
        }

        private void DrawFooter(Rect rect)
        {
            if (drawFooterCallback != null)
            {
                drawFooterCallback(rect);
                return;
            }

            if (Event.current.type == EventType.Repaint)
            {
                Style.footerBackground.Draw(rect, false, false, false, false);
            }

            Rect addRect = new Rect(rect.xMin + 4f, rect.y, 25f, Style.preButton.fixedHeight);
            Rect subRect = new Rect(rect.xMax - 29f, rect.y, 25f, Style.preButton.fixedHeight);

            EditorGUI.BeginDisabledGroup(!canAdd);

            if (GUI.Button(addRect, onAddDropdownCallback != null ? Style.iconToolbarPlusMore : Style.iconToolbarPlus,
                    Style.preButton))
            {
                if (onAddDropdownCallback != null)
                {
                    onAddDropdownCallback(addRect, this);
                }
                else if (onAddCallback != null)
                {
                    onAddCallback(this);
                }
                else
                {
                    AddItem();
                }
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!CanSelect(selection) || !canRemove ||
                                         (onCanRemoveCallback != null && !onCanRemoveCallback(this)));

            if (GUI.Button(subRect, Style.iconToolbarMinus, Style.preButton))
            {
                if (onRemoveCallback != null)
                {
                    onRemoveCallback(this);
                }
                else
                {
                    Remove(selection.ToArray());
                }
            }

            EditorGUI.EndDisabledGroup();
        }

        private void DrawPaginationHeader(Rect rect)
        {
            int total = Length;
            int pages = pagination.GetPageCount(total);
            int page = Mathf.Clamp(pagination.page, 0, pages - 1);

            //some actions may have reduced the page count, so we need to check the current page against the clamped one
            //if different, we need to change and repaint

            if (page != pagination.page)
            {
                pagination.page = page;

                HandleUtility.Repaint();
            }

            Rect prevRect = new Rect(rect.xMin + 4f, rect.y, 17f, rect.height - 1);
            Rect popupRect = new Rect(prevRect.xMax, rect.y, 14f, rect.height - 1);
            Rect nextRect = new Rect(popupRect.xMax, rect.y, 17f, rect.height - 1);

            if (Event.current.type == EventType.Repaint)
            {
                Style.paginationHeader.Draw(rect, false, true, true, false);
            }

            pageInfoContent.text = string.Format(Style.PAGE_INFO_FORMAT, pagination.page + 1, pages);

            Rect pageInfoRect = rect;
            pageInfoRect.xMin = rect.xMax - Style.paginationText.CalcSize(pageInfoContent).x - 7;

            //draw page info

            GUI.Label(pageInfoRect, pageInfoContent, Style.paginationText);

            //draw page buttons and page popup

            if (GUI.Button(prevRect, Style.iconPagePrev, Style.preButtonStretch))
            {
                pagination.page = Mathf.Max(0, pagination.page - 1);
            }

            if (EditorGUI.DropdownButton(popupRect, Style.iconPagePopup, FocusType.Passive, Style.preButtonStretch))
            {
                GenericMenu menu = new GenericMenu();

                for (int i = 0; i < pages; i++)
                {
                    int pageIndex = i;

                    menu.AddItem(new GUIContent(string.Format("Page {0}", i + 1)), i == pagination.page,
                        OnPageDropDownSelect, pageIndex);
                }

                menu.DropDown(popupRect);
            }

            if (GUI.Button(nextRect, Style.iconPageNext, Style.preButtonStretch))
            {
                pagination.page = Mathf.Min(pages - 1, pagination.page + 1);
            }

            //if we're allowed to control the page size manually, show an editor

            bool useFixedPageSize = pagination.fixedPageSize > 0;
            int currentPageSize = useFixedPageSize ? pagination.fixedPageSize : pagination.customPageSize;

            EditorGUI.BeginDisabledGroup(useFixedPageSize);

            pageSizeContent.text = currentPageSize.ToString();

            GUIStyle style = Style.pageSizeTextField;
            Texture icon = Style.listIcon.image;

            float labelWidth = icon.width + 2;
            float width = style.CalcSize(pageSizeContent).x + 50;

            Rect pageSizeRect = rect;
            pageSizeRect.x = rect.center.x - (width - labelWidth) / 2;
            pageSizeRect.width = width;

            EditorGUI.BeginChangeCheck();

            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUIUtility.SetIconSize(new Vector2(icon.width, icon.height));

            int newPageSize = EditorGUI.DelayedIntField(pageSizeRect, Style.listIcon, currentPageSize, style);

            EditorGUIUtility.labelWidth = 0;
            EditorGUIUtility.SetIconSize(Vector2.zero);

            if (EditorGUI.EndChangeCheck())
            {
                pagination.customPageSize = Mathf.Clamp(newPageSize, 0, total);
                pagination.page = Mathf.Min(pagination.GetPageCount(total) - 1, pagination.page);
            }

            EditorGUI.EndDisabledGroup();
        }

        private void OnPageDropDownSelect(object userData)
        {
            pagination.page = (int)userData;
        }

        private void DispatchChange()
        {
            if (onChangedCallback != null)
            {
                onChangedCallback(this);
            }
        }

        private void HandleSingleContextClick(Event evt, SerializedProperty element)
        {
            selection.Select(IndexOf(element));

            GenericMenu menu = new GenericMenu();

            if (element.isInstantiatedPrefab)
            {
                menu.AddItem(new GUIContent(string.Format("Revert {0} to Prefab", GetElementLabel(element, true).text)),
                    false, selection.RevertValues, list);
                menu.AddSeparator(string.Empty);
            }

            HandleSharedContextClick(evt, menu, "Duplicate Array Element", "Delete Array Element",
                "Move Array Element");
        }

        private void HandleMultipleContextClick(Event evt)
        {
            GenericMenu menu = new GenericMenu();

            if (selection.CanRevert(list))
            {
                menu.AddItem(new GUIContent("Revert Values to Prefab"), false, selection.RevertValues, list);
                menu.AddSeparator(string.Empty);
            }

            HandleSharedContextClick(evt, menu, "Duplicate Array Elements", "Delete Array Elements",
                "Move Array Elements");
        }

        private void HandleSharedContextClick(Event evt, GenericMenu menu, string duplicateLabel, string deleteLabel,
            string moveLabel)
        {
            menu.AddItem(new GUIContent(duplicateLabel), false, HandleDuplicate, list);
            menu.AddItem(new GUIContent(deleteLabel), false, HandleDelete, list);

            if (doPagination)
            {
                int pages = pagination.GetPageCount(Length);

                if (pages > 1)
                {
                    for (int i = 0; i < pages; i++)
                    {
                        string label = string.Format("{0}/Page {1}", moveLabel, i + 1);

                        menu.AddItem(new GUIContent(label), i == pagination.page, HandleMoveElement, i);
                    }
                }
            }

            menu.ShowAsContext();

            evt.Use();
        }

        private void HandleMoveElement(object userData)
        {
            int toPage = (int)userData;
            int fromPage = pagination.page;
            int size = pagination.pageSize;
            int offset = (toPage * size) - (fromPage * size);
            int direction = offset > 0 ? 1 : -1;
            int total = Length;

            //We need to find the actually positions things will move to and not clamp the index
            //because sometimes something wants to move to a negative index, or beyond the length
            //we need to find this overlow and adjust the move offsets based on that

            int overflow = 0;

            for (int i = 0; i < selection.Length; i++)
            {
                int desiredIndex = selection[i] + offset;

                overflow = direction < 0
                    ? Mathf.Min(overflow, desiredIndex)
                    : Mathf.Max(overflow, desiredIndex - total);
            }

            offset -= overflow;

            //copy the current list to prepare for moving

            UpdateDragList(0, 0, total);

            //create a list that will act as our new order

            List<DragElement> orderedList =
                new List<DragElement>(dragList.Elements.Where(t => !selection.Contains(t.startIndex)));

            //go through the selection and insert them into the new order based on the page offset

            selection.Sort();

            for (int i = 0; i < selection.Length; i++)
            {
                int selIndex = selection[i];
                int oldIndex = dragList.GetIndexFromSelection(selIndex);
                int newIndex = Mathf.Clamp(selIndex + offset, 0, orderedList.Count);

                orderedList.Insert(newIndex, dragList[oldIndex]);
            }

            //finally, perform the re-order

            dragList.Elements = orderedList.ToArray();

            ReorderDraggedElements(direction, 0, null);

            //assume we still want to view these items

            pagination.page = toPage;

            HandleUtility.Repaint();
        }

        private void HandleDelete(object userData)
        {
            selection.Delete(userData as SerializedProperty);

            DispatchChange();
        }

        private void HandleDuplicate(object userData)
        {
            selection.Duplicate(userData as SerializedProperty);

            DispatchChange();
        }

        private void HandleDragAndDrop(Rect rect, Event evt)
        {
            switch (evt.GetTypeForControl(dragDropControlID))
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:

                    if (GUI.enabled && rect.Contains(evt.mousePosition))
                    {
                        UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
                        UnityEngine.Object[] references = new UnityEngine.Object[1];

                        bool acceptDrag = false;

                        foreach (UnityEngine.Object object1 in objectReferences)
                        {
                            references[0] = object1;
                            UnityEngine.Object object2 = ValidateObjectDragAndDrop(references);

                            if (object2 != null)
                            {
                                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                                if (evt.type == EventType.DragPerform)
                                {
                                    AppendDragAndDropValue(object2);

                                    acceptDrag = true;
                                    DragAndDrop.activeControlID = 0;
                                }
                                else
                                {
                                    DragAndDrop.activeControlID = dragDropControlID;
                                }
                            }
                        }

                        if (acceptDrag)
                        {
                            GUI.changed = true;
                            DragAndDrop.AcceptDrag();
                        }
                    }

                    break;

                case EventType.DragExited:

                    if (GUI.enabled)
                    {
                        HandleUtility.Repaint();
                    }

                    break;
            }
        }

        private UnityEngine.Object ValidateObjectDragAndDrop(UnityEngine.Object[] references)
        {
            if (onValidateDragAndDropCallback != null)
            {
                return onValidateDragAndDropCallback(references, this);
            }
            else if (surrogate.HasType)
            {
                //if we have a surrogate type, then validate using the surrogate type rather than the list

                return Internals.ValidateObjectDragAndDrop(references, null, surrogate.type, surrogate.exactType);
            }

            return Internals.ValidateObjectDragAndDrop(references, list, null, false);
        }

        private void AppendDragAndDropValue(UnityEngine.Object obj)
        {
            if (onAppendDragDropCallback != null)
            {
                onAppendDragDropCallback(obj, this);
            }
            else
            {
                //check if we have a surrogate type. If so use that for appending

                if (surrogate.HasType)
                {
                    surrogate.Invoke(AddItem(), obj, this);
                }
                else
                {
                    Internals.AppendDragAndDropValue(obj, list);
                }
            }

            DispatchChange();
        }

        private void HandlePreSelection(Rect rect, Event evt)
        {
            if (evt.type == EventType.MouseDrag && draggable && GUIUtility.hotControl == controlID)
            {
                if (selection.Length > 0 && UpdateDragPosition(evt.mousePosition, rect, dragList))
                {
                    GUIUtility.keyboardControl = controlID;
                    dragging = true;
                }

                evt.Use();
            }
        }

        private void HandlePostSelection(Rect rect, Event evt)
        {
            switch (evt.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:

                    if (rect.Contains(evt.mousePosition) && IsSelectionButton(evt))
                    {
                        int index = GetSelectionIndex(evt.mousePosition);

                        if (CanSelect(index))
                        {
                            DoSelection(index,
                                GUIUtility.keyboardControl == 0 || GUIUtility.keyboardControl == controlID ||
                                evt.button == 2, evt);
                        }
                        else
                        {
                            selection.Clear();
                        }

                        HandleUtility.Repaint();
                    }

                    break;

                case EventType.MouseUp:

                    if (!draggable)
                    {
                        //select the single object if no selection modifier is being performed

                        selection.SelectWhenNoAction(pressIndex, evt);

                        if (onMouseUpCallback != null && IsPositionWithinElement(evt.mousePosition, selection.Last))
                        {
                            onMouseUpCallback(this);
                        }
                    }
                    else if (GUIUtility.hotControl == controlID)
                    {
                        evt.Use();

                        if (dragging)
                        {
                            dragging = false;

                            //move elements in list

                            ReorderDraggedElements(dragDirection, dragList.StartIndex, () => dragList.SortByPosition());
                        }
                        else
                        {
                            //if we didn't drag, then select the original pressed object

                            selection.SelectWhenNoAction(pressIndex, evt);

                            if (onMouseUpCallback != null)
                            {
                                onMouseUpCallback(this);
                            }
                        }

                        GUIUtility.hotControl = 0;
                    }

                    HandleUtility.Repaint();

                    break;

                case EventType.KeyDown:

                    if (GUIUtility.keyboardControl == controlID)
                    {
                        if (evt.keyCode == KeyCode.DownArrow && !dragging)
                        {
                            selection.Select(Mathf.Min(selection.Last + 1, Length - 1));
                            evt.Use();
                        }
                        else if (evt.keyCode == KeyCode.UpArrow && !dragging)
                        {
                            selection.Select(Mathf.Max(selection.Last - 1, 0));
                            evt.Use();
                        }
                        else if (evt.keyCode == KeyCode.Escape && GUIUtility.hotControl == controlID)
                        {
                            GUIUtility.hotControl = 0;

                            if (dragging)
                            {
                                dragging = false;
                                selection = beforeDragSelection;
                            }

                            evt.Use();
                        }
                    }

                    break;
            }
        }

        private bool IsSelectionButton(Event evt)
        {
            return evt.button == 0 || evt.button == 2;
        }

        private void DoSelection(int index, bool setKeyboardControl, Event evt)
        {
            //append selections based on action, this may be a additive (ctrl) or range (shift) selection

            if (multipleSelection)
            {
                selection.AppendWithAction(pressIndex = index, evt);
            }
            else
            {
                selection.Select(pressIndex = index);
            }

            if (onSelectCallback != null)
            {
                onSelectCallback(this);
            }

            if (draggable)
            {
                dragging = false;
                dragPosition = pressPosition = evt.mousePosition.y;

                int start, end;

                pagination.GetVisibleRange(Length, out start, out end);

                UpdateDragList(dragPosition, start, end);

                selection.Trim(start, end);

                beforeDragSelection = selection.Clone();

                GUIUtility.hotControl = controlID;
            }

            if (setKeyboardControl)
            {
                GUIUtility.keyboardControl = controlID;
            }

            evt.Use();
        }

        private void UpdateDragList(float dragPosition, int start, int end)
        {
            dragList.Resize(start, end - start);

            for (int i = start; i < end; i++)
            {
                SerializedProperty property = list.GetArrayElementAtIndex(i);
                Rect elementRect = elementRects[i];

                DragElement dragElement = new DragElement()
                {
                    property = property,
                    dragOffset = dragPosition - elementRect.y,
                    rect = elementRect,
                    desiredRect = elementRect,
                    selected = selection.Contains(i),
                    startIndex = i
                };

                dragList[i - start] = dragElement;
            }

            //finally, sort the dragList by selection, selected objects appear first in the list
            //selection order is preserved as well

            dragList.SortByIndex();
        }

        private bool UpdateDragPosition(Vector2 position, Rect bounds, DragList dragList)
        {
            //find new drag position

            int startIndex = 0;
            int endIndex = selection.Length - 1;

            float minOffset = dragList[startIndex].dragOffset;
            float maxOffset = dragList[endIndex].rect.height - dragList[endIndex].dragOffset;

            dragPosition = Mathf.Clamp(position.y, bounds.yMin + minOffset, bounds.yMax - maxOffset);

            if (Mathf.Abs(dragPosition - pressPosition) > 1)
            {
                dragDirection = (int)Mathf.Sign(dragPosition - pressPosition);
                return true;
            }

            return false;
        }

        private void ReorderDraggedElements(int direction, int offset, System.Action sortList)
        {
            //save the current expanded states on all elements. I don't see any other way to do this
            //MoveArrayElement does not move the foldout states, so... fun.

            dragList.RecordState();

            if (sortList != null)
            {
                sortList();
            }

            selection.Sort((a, b) =>
            {
                int d1 = dragList.GetIndexFromSelection(a);
                int d2 = dragList.GetIndexFromSelection(b);

                return direction > 0 ? d1.CompareTo(d2) : d2.CompareTo(d1);
            });

            //swap the selected elements in the List

            int s = selection.Length;

            while (--s > -1)
            {
                int newIndex = dragList.GetIndexFromSelection(selection[s]);
                int listIndex = newIndex + offset;

                selection[s] = listIndex;

                list.MoveArrayElement(dragList[newIndex].startIndex, listIndex);
            }

            //restore expanded states on items

            dragList.RestoreState(list);

            //apply and update

            ApplyReorder();
        }

        private void ApplyReorder()
        {
            list.serializedObject.ApplyModifiedProperties();
            list.serializedObject.Update();

            if (onReorderCallback != null)
            {
                onReorderCallback(this);
            }

            DispatchChange();
        }

        private int GetSelectionIndex(Vector2 position)
        {
            int start, end;

            pagination.GetVisibleRange(elementRects.Length, out start, out end);

            for (int i = start; i < end; i++)
            {
                Rect rect = elementRects[i];

                if (rect.Contains(position) || (i == 0 && position.y <= rect.yMin) ||
                    (i == end - 1 && position.y >= rect.yMax))
                {
                    return i;
                }
            }

            return -1;
        }

        private bool CanSelect(ListSelection selection)
        {
            return selection.Length > 0 ? selection.All(s => CanSelect(s)) : false;
        }

        private bool CanSelect(int index)
        {
            return index >= 0 && index < Length;
        }

        private bool CanSelect(Vector2 position)
        {
            return selection.Length > 0 ? selection.Any(s => IsPositionWithinElement(position, s)) : false;
        }

        private bool IsPositionWithinElement(Vector2 position, int index)
        {
            return CanSelect(index) ? elementRects[index].Contains(position) : false;
        }

        private bool IsElementExpandable(SerializedProperty element)
        {
            switch (elementDisplayType)
            {
                case ElementDisplayType.Auto:

                    return element.hasVisibleChildren && IsTypeExpandable(element.propertyType);

                case ElementDisplayType.Expandable: return true;
                case ElementDisplayType.SingleLine: return false;
            }

            return false;
        }

        private bool IsTypeExpandable(SerializedPropertyType type)
        {
            switch (type)
            {
                case SerializedPropertyType.Generic:
                case SerializedPropertyType.Vector4:
                case SerializedPropertyType.Quaternion:
                case SerializedPropertyType.ArraySize:
#if UNITY_2019_3_OR_NEWER
				case SerializedPropertyType.ManagedReference:
#endif
                    return true;

                default:

                    return false;
            }
        }

        //
        // -- LIST STYLE --
        //

        static class Style
        {
            internal const string PAGE_INFO_FORMAT = "{0} / {1}";

            internal static GUIContent iconToolbarPlus;
            internal static GUIContent iconToolbarPlusMore;
            internal static GUIContent iconToolbarMinus;
            internal static GUIContent iconPagePrev;
            internal static GUIContent iconPageNext;
            internal static GUIContent iconPagePopup;

            internal static GUIStyle paginationText;
            internal static GUIStyle pageSizeTextField;
            internal static GUIStyle draggingHandle;
            internal static GUIStyle headerBackground;
            internal static GUIStyle footerBackground;
            internal static GUIStyle paginationHeader;
            internal static GUIStyle boxBackground;
            internal static GUIStyle preButton;
            internal static GUIStyle preButtonStretch;
            internal static GUIStyle elementBackground;
            internal static GUIStyle verticalLabel;
            internal static GUIContent expandButton;
            internal static GUIContent collapseButton;
            internal static GUIContent sortAscending;
            internal static GUIContent sortDescending;

            internal static GUIContent listIcon;

            static Style()
            {
                iconToolbarPlus = EditorGUIUtility.IconContent("Toolbar Plus", "Add to list");
                iconToolbarPlusMore = EditorGUIUtility.IconContent("Toolbar Plus More", "Choose to add to list");
                iconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus", "Remove selection from list");
                iconPagePrev = EditorGUIUtility.IconContent("Animation.PrevKey", "Previous page");
                iconPageNext = EditorGUIUtility.IconContent("Animation.NextKey", "Next page");

#if UNITY_2018_3_OR_NEWER
                iconPagePopup = EditorGUIUtility.IconContent("PopupCurveEditorDropDown", "Select page");
#else
				iconPagePopup = EditorGUIUtility.IconContent("MiniPopupNoBg", "Select page");
#endif
                paginationText = new GUIStyle();
                paginationText.margin = new RectOffset(2, 2, 0, 0);
                paginationText.fontSize = EditorStyles.miniTextField.fontSize;
                paginationText.font = EditorStyles.miniFont;
                paginationText.normal.textColor = EditorStyles.miniTextField.normal.textColor;
                paginationText.alignment = TextAnchor.MiddleLeft;
                paginationText.clipping = TextClipping.Clip;

#if UNITY_2019_3_OR_NEWER
				pageSizeTextField = new GUIStyle("RL Background");
#else
                pageSizeTextField = new GUIStyle("RL Footer");
                pageSizeTextField.overflow = new RectOffset(0, 0, -2, -3);
                pageSizeTextField.contentOffset = new Vector2(0, -1);
#endif
                pageSizeTextField.alignment = TextAnchor.MiddleLeft;
                pageSizeTextField.clipping = TextClipping.Clip;
                pageSizeTextField.fixedHeight = 0;
                pageSizeTextField.padding = new RectOffset(3, 0, 0, 0);
                pageSizeTextField.font = EditorStyles.miniFont;
                pageSizeTextField.fontSize = EditorStyles.miniTextField.fontSize;
                pageSizeTextField.fontStyle = FontStyle.Normal;
                pageSizeTextField.wordWrap = false;

                draggingHandle = new GUIStyle("RL DragHandle");
                headerBackground = new GUIStyle("RL Header");
                footerBackground = new GUIStyle("RL Footer");

#if UNITY_2019_3_OR_NEWER
				paginationHeader = new GUIStyle("TimeRulerBackground");
				paginationHeader.fixedHeight = 0;
#else
                paginationHeader = new GUIStyle("RL Element");
                paginationHeader.border = new RectOffset(2, 3, 2, 3);
#endif
                elementBackground = new GUIStyle("RL Element");
                elementBackground.border = new RectOffset(2, 3, 2, 3);
                verticalLabel = new GUIStyle(EditorStyles.label);
                verticalLabel.alignment = TextAnchor.UpperLeft;
                verticalLabel.contentOffset = new Vector2(10, 3);
                boxBackground = new GUIStyle("RL Background");
                boxBackground.border = new RectOffset(6, 3, 3, 6);

#if UNITY_2019_3_OR_NEWER
				preButton = new GUIStyle("RL FooterButton");
#else
                preButton = new GUIStyle("RL FooterButton");
                preButton.contentOffset = new Vector2(0, -4);
#endif
                preButtonStretch = new GUIStyle("RL FooterButton");
                preButtonStretch.fixedHeight = 0;
                preButtonStretch.stretchHeight = true;

                expandButton = EditorGUIUtility.IconContent("winbtn_win_max");
                expandButton.tooltip = "Expand All Elements";

                collapseButton = EditorGUIUtility.IconContent("winbtn_win_min");
                collapseButton.tooltip = "Collapse All Elements";

                sortAscending = EditorGUIUtility.IconContent("align_vertically_bottom");
                sortAscending.tooltip = "Sort Ascending";

                sortDescending = EditorGUIUtility.IconContent("align_vertically_top");
                sortDescending.tooltip = "Sort Descending";

                listIcon = EditorGUIUtility.IconContent("align_horizontally_right");
            }
        }

        //
        // -- DRAG LIST --
        //

        struct DragList
        {
            private int startIndex;
            private DragElement[] elements;
            private int length;

            internal DragList(int length)
            {
                this.length = length;

                startIndex = 0;
                elements = new DragElement[length];
            }

            internal int StartIndex
            {
                get { return startIndex; }
            }

            internal int Length
            {
                get { return length; }
            }

            internal DragElement[] Elements
            {
                get { return elements; }
                set { elements = value; }
            }

            internal DragElement this[int index]
            {
                get { return elements[index]; }
                set { elements[index] = value; }
            }

            internal void Resize(int start, int length)
            {
                startIndex = start;

                this.length = length;

                if (elements.Length != length)
                {
                    System.Array.Resize(ref elements, length);
                }
            }

            internal void SortByIndex()
            {
                System.Array.Sort(elements, (a, b) =>
                {
                    if (b.selected)
                    {
                        return a.selected ? a.startIndex.CompareTo(b.startIndex) : 1;
                    }
                    else if (a.selected)
                    {
                        return b.selected ? b.startIndex.CompareTo(a.startIndex) : -1;
                    }

                    return a.startIndex.CompareTo(b.startIndex);
                });
            }

            internal void RecordState()
            {
                for (int i = 0; i < length; i++)
                {
                    elements[i].RecordState();
                }
            }

            internal void RestoreState(SerializedProperty list)
            {
                for (int i = 0; i < length; i++)
                {
                    elements[i].RestoreState(list.GetArrayElementAtIndex(i + startIndex));
                }
            }

            internal void SortByPosition()
            {
                System.Array.Sort(elements, (a, b) => a.desiredRect.center.y.CompareTo(b.desiredRect.center.y));
            }

            internal int GetIndexFromSelection(int index)
            {
                return System.Array.FindIndex(elements, t => t.startIndex == index);
            }
        }

        //
        // -- DRAG ELEMENT --
        //

        struct DragElement
        {
            internal SerializedProperty property;
            internal int startIndex;
            internal float dragOffset;
            internal bool selected;
            internal Rect rect;
            internal Rect desiredRect;

            private bool isExpanded;
            private Dictionary<int, bool> states;

            internal bool Overlaps(Rect value, int index, int direction)
            {
                if (direction < 0 && index < startIndex)
                {
                    return desiredRect.yMin < value.center.y;
                }
                else if (direction > 0 && index > startIndex)
                {
                    return desiredRect.yMax > value.center.y;
                }

                return false;
            }

            internal void RecordState()
            {
                states = new Dictionary<int, bool>();
                isExpanded = property.isExpanded;

                Iterate(this, property,
                    (DragElement e, SerializedProperty p, int index) => { e.states[index] = p.isExpanded; });
            }

            internal void RestoreState(SerializedProperty property)
            {
                property.isExpanded = isExpanded;

                Iterate(this, property,
                    (DragElement e, SerializedProperty p, int index) => { p.isExpanded = e.states[index]; });
            }

            private static void Iterate(DragElement element, SerializedProperty property,
                System.Action<DragElement, SerializedProperty, int> action)
            {
                SerializedProperty copy = property.Copy();
                SerializedProperty end = copy.GetEndProperty();

                int index = 0;

                while (copy.NextVisible(true) && !SerializedProperty.EqualContents(copy, end))
                {
                    if (copy.hasVisibleChildren)
                    {
                        action(element, copy, index);
                        index++;
                    }
                }
            }
        }

        //
        // -- SLIDE GROUP --
        //

        class SlideGroup
        {
            private Dictionary<int, Rect> animIDs;

            public SlideGroup()
            {
                animIDs = new Dictionary<int, Rect>();
            }

            public Rect GetRect(int id, Rect r, float easing)
            {
                if (Event.current.type != EventType.Repaint)
                {
                    return r;
                }

                if (!animIDs.ContainsKey(id))
                {
                    animIDs.Add(id, r);
                    return r;
                }
                else
                {
                    Rect rect = animIDs[id];

                    if (rect.y != r.y)
                    {
                        float delta = r.y - rect.y;
                        float absDelta = Mathf.Abs(delta);

                        //if the distance between current rect and target is too large, then move the element towards the target rect so it reaches the destination faster

                        if (absDelta > (rect.height * 2))
                        {
                            r.y = delta > 0 ? r.y - rect.height : r.y + rect.height;
                        }
                        else if (absDelta > 0.5)
                        {
                            r.y = Mathf.Lerp(rect.y, r.y, easing);
                        }

                        animIDs[id] = r;
                        HandleUtility.Repaint();
                    }

                    return r;
                }
            }

            public Rect SetRect(int id, Rect rect)
            {
                if (animIDs.ContainsKey(id))
                {
                    animIDs[id] = rect;
                }
                else
                {
                    animIDs.Add(id, rect);
                }

                return rect;
            }
        }

        //
        // -- PAGINATION --
        //

        struct Pagination
        {
            internal bool enabled;
            internal int fixedPageSize;
            internal int customPageSize;
            internal int page;

            internal bool usePagination
            {
                get { return enabled && pageSize > 0; }
            }

            internal int pageSize
            {
                get { return fixedPageSize > 0 ? fixedPageSize : customPageSize; }
            }

            internal int GetVisibleLength(int total)
            {
                int start, end;

                if (GetVisibleRange(total, out start, out end))
                {
                    return end - start;
                }

                return total;
            }

            internal int GetPageForIndex(int index)
            {
                return usePagination ? Mathf.FloorToInt(index / (float)pageSize) : 0;
            }

            internal int GetPageCount(int total)
            {
                return usePagination ? Mathf.CeilToInt(total / (float)pageSize) : 1;
            }

            internal bool GetVisibleRange(int total, out int start, out int end)
            {
                if (usePagination)
                {
                    int size = pageSize;

                    start = Mathf.Clamp(page * size, 0, total - 1);
                    end = Mathf.Min(start + size, total);
                    return true;
                }

                start = 0;
                end = total;
                return false;
            }
        }

        //
        // -- SELECTION --
        //

        class ListSelection : IEnumerable<int>
        {
            private List<int> indexes;

            internal int? firstSelected;

            public ListSelection()
            {
                indexes = new List<int>();
            }

            public ListSelection(int[] indexes)
            {
                this.indexes = new List<int>(indexes);
            }

            public int First
            {
                get { return indexes.Count > 0 ? indexes[0] : -1; }
            }

            public int Last
            {
                get { return indexes.Count > 0 ? indexes[indexes.Count - 1] : -1; }
            }

            public int Length
            {
                get { return indexes.Count; }
            }

            public int this[int index]
            {
                get { return indexes[index]; }
                set
                {
                    int oldIndex = indexes[index];

                    indexes[index] = value;

                    if (oldIndex == firstSelected)
                    {
                        firstSelected = value;
                    }
                }
            }

            public bool Contains(int index)
            {
                return indexes.Contains(index);
            }

            public void Clear()
            {
                indexes.Clear();
                firstSelected = null;
            }

            public void SelectWhenNoAction(int index, Event evt)
            {
                if (!EditorGUI.actionKey && !evt.shift)
                {
                    Select(index);
                }
            }

            public void Select(int index)
            {
                indexes.Clear();
                indexes.Add(index);

                firstSelected = index;
            }

            public void Remove(int index)
            {
                if (indexes.Contains(index))
                {
                    indexes.Remove(index);
                }
            }

            public void AppendWithAction(int index, Event evt)
            {
                if (EditorGUI.actionKey)
                {
                    if (Contains(index))
                    {
                        Remove(index);
                    }
                    else
                    {
                        Append(index);
                        firstSelected = index;
                    }
                }
                else if (evt.shift && indexes.Count > 0 && firstSelected.HasValue)
                {
                    indexes.Clear();

                    AppendRange(firstSelected.Value, index);
                }
                else if (!Contains(index))
                {
                    Select(index);
                }
            }

            public void Sort()
            {
                if (indexes.Count > 0)
                {
                    indexes.Sort();
                }
            }

            public void Sort(System.Comparison<int> comparison)
            {
                if (indexes.Count > 0)
                {
                    indexes.Sort(comparison);
                }
            }

            public int[] ToArray()
            {
                return indexes.ToArray();
            }

            public ListSelection Clone()
            {
                ListSelection clone = new ListSelection(ToArray());
                clone.firstSelected = firstSelected;

                return clone;
            }

            internal void Trim(int min, int max)
            {
                int i = indexes.Count;

                while (--i > -1)
                {
                    int index = indexes[i];

                    if (index < min || index >= max)
                    {
                        if (index == firstSelected && i > 0)
                        {
                            firstSelected = indexes[i - 1];
                        }

                        indexes.RemoveAt(i);
                    }
                }
            }

            internal bool CanRevert(SerializedProperty list)
            {
                if (list.serializedObject.targetObjects.Length == 1)
                {
                    for (int i = 0; i < Length; i++)
                    {
                        if (list.GetArrayElementAtIndex(this[i]).isInstantiatedPrefab)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            internal void RevertValues(object userData)
            {
                SerializedProperty list = userData as SerializedProperty;

                for (int i = 0; i < Length; i++)
                {
                    SerializedProperty property = list.GetArrayElementAtIndex(this[i]);

                    if (property.isInstantiatedPrefab)
                    {
                        property.prefabOverride = false;
                    }
                }

                list.serializedObject.ApplyModifiedProperties();
                list.serializedObject.Update();

                HandleUtility.Repaint();
            }

            internal void Duplicate(SerializedProperty list)
            {
                int offset = 0;

                for (int i = 0; i < Length; i++)
                {
                    this[i] += offset;

                    list.GetArrayElementAtIndex(this[i]).DuplicateCommand();
                    list.serializedObject.ApplyModifiedProperties();
                    list.serializedObject.Update();

                    offset++;
                }

                HandleUtility.Repaint();
            }

            internal void Delete(SerializedProperty list)
            {
                Sort();

                int i = Length;

                while (--i > -1)
                {
                    list.GetArrayElementAtIndex(this[i]).DeleteCommand();
                }

                Clear();

                list.serializedObject.ApplyModifiedProperties();
                list.serializedObject.Update();

                HandleUtility.Repaint();
            }

            private void Append(int index)
            {
                if (index >= 0 && !indexes.Contains(index))
                {
                    indexes.Add(index);
                }
            }

            private void AppendRange(int from, int to)
            {
                int dir = (int)Mathf.Sign(to - from);

                if (dir != 0)
                {
                    for (int i = from; i != to; i += dir)
                    {
                        Append(i);
                    }
                }

                Append(to);
            }

            public IEnumerator<int> GetEnumerator()
            {
                return ((IEnumerable<int>)indexes).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<int>)indexes).GetEnumerator();
            }
        }

        //
        // -- SORTING --
        //

        static class ListSort
        {
            private delegate int SortComparision(SerializedProperty p1, SerializedProperty p2);

            internal static void SortOnProperty(SerializedProperty list, int length, bool descending,
                string propertyName)
            {
                BubbleSort(list, length, (p1, p2) =>
                {
                    SerializedProperty a = p1.FindPropertyRelative(propertyName);
                    SerializedProperty b = p2.FindPropertyRelative(propertyName);

                    if (a != null && b != null && a.propertyType == b.propertyType)
                    {
                        int comparison = Compare(a, b, descending, a.propertyType);

                        return descending ? -comparison : comparison;
                    }

                    return 0;
                });
            }

            internal static void SortOnType(SerializedProperty list, int length, bool descending,
                SerializedPropertyType type)
            {
                BubbleSort(list, length, (p1, p2) =>
                {
                    int comparision = Compare(p1, p2, descending, type);

                    return descending ? -comparision : comparision;
                });
            }

            //
            // -- PRIVATE --
            //

            private static void BubbleSort(SerializedProperty list, int length, SortComparision comparision)
            {
                for (int i = 0; i < length; i++)
                {
                    SerializedProperty p1 = list.GetArrayElementAtIndex(i);

                    for (int j = i + 1; j < length; j++)
                    {
                        SerializedProperty p2 = list.GetArrayElementAtIndex(j);

                        if (comparision(p1, p2) > 0)
                        {
                            list.MoveArrayElement(j, i);
                        }
                    }
                }
            }

            private static int Compare(SerializedProperty p1, SerializedProperty p2, bool descending,
                SerializedPropertyType type)
            {
                if (p1 == null || p2 == null)
                {
                    return 0;
                }

                switch (type)
                {
                    case SerializedPropertyType.Boolean:

                        return p1.boolValue.CompareTo(p2.boolValue);

                    case SerializedPropertyType.Character:
                    case SerializedPropertyType.Enum:
                    case SerializedPropertyType.Integer:
                    case SerializedPropertyType.LayerMask:

                        return p1.longValue.CompareTo(p2.longValue);

                    case SerializedPropertyType.Color:

                        return p1.colorValue.grayscale.CompareTo(p2.colorValue.grayscale);

                    case SerializedPropertyType.ExposedReference:

                        return CompareObjects(p1.exposedReferenceValue, p2.exposedReferenceValue, descending);

                    case SerializedPropertyType.Float:

                        return p1.doubleValue.CompareTo(p2.doubleValue);

                    case SerializedPropertyType.ObjectReference:

                        return CompareObjects(p1.objectReferenceValue, p2.objectReferenceValue, descending);

                    case SerializedPropertyType.String:

                        return p1.stringValue.CompareTo(p2.stringValue);

                    default:

                        return 0;
                }
            }

            private static int CompareObjects(UnityEngine.Object obj1, UnityEngine.Object obj2, bool descending)
            {
                if (obj1 && obj2)
                {
                    return obj1.name.CompareTo(obj2.name);
                }
                else if (obj1)
                {
                    return descending ? 1 : -1;
                }

                return descending ? -1 : 1;
            }
        }

        //
        // -- SURROGATE --
        //

        public struct Surrogate
        {
            public System.Type type;
            public bool exactType;
            public SurrogateCallback callback;

            internal bool enabled;

            public bool HasType
            {
                get { return enabled && type != null; }
            }

            public Surrogate(System.Type type)
                : this(type, null)
            {
            }

            public Surrogate(System.Type type, SurrogateCallback callback)
            {
                this.type = type;
                this.callback = callback;

                enabled = true;
                exactType = false;
            }

            public void Invoke(SerializedProperty element, UnityEngine.Object objectReference, ReorderableList list)
            {
                if (element != null && callback != null)
                {
                    callback.Invoke(element, objectReference, list);
                }
            }
        }

        //
        // -- EXCEPTIONS --
        //

        class InvalidListException : System.InvalidOperationException
        {
            public InvalidListException() : base("ReorderableList serializedProperty must be an array")
            {
            }
        }

        class MissingListExeption : System.ArgumentNullException
        {
            public MissingListExeption() : base("ReorderableList serializedProperty is null")
            {
            }
        }

        //
        // -- INTERNAL --
        //

        static class Internals
        {
            private static MethodInfo dragDropValidation;
            private static object[] dragDropValidationParams;
            private static MethodInfo appendDragDrop;
            private static object[] appendDragDropParams;

            static Internals()
            {
                dragDropValidation = System.Type.GetType("UnityEditor.EditorGUI, UnityEditor")
                    .GetMethod("ValidateObjectFieldAssignment", BindingFlags.NonPublic | BindingFlags.Static);
                appendDragDrop = typeof(SerializedProperty).GetMethod("AppendFoldoutPPtrValue",
                    BindingFlags.NonPublic | BindingFlags.Instance);
            }

            internal static UnityEngine.Object ValidateObjectDragAndDrop(UnityEngine.Object[] references,
                SerializedProperty property, System.Type type, bool exactType)
            {
#if UNITY_2017_1_OR_NEWER
                dragDropValidationParams = GetParams(ref dragDropValidationParams, 4);
                dragDropValidationParams[0] = references;
                dragDropValidationParams[1] = type;
                dragDropValidationParams[2] = property;
                dragDropValidationParams[3] = exactType ? 1 : 0;
#else
				dragDropValidationParams = GetParams(ref dragDropValidationParams, 3);
				dragDropValidationParams[0] = references;
				dragDropValidationParams[1] = type;
				dragDropValidationParams[2] = property;
#endif
                return dragDropValidation.Invoke(null, dragDropValidationParams) as UnityEngine.Object;
            }

            internal static void AppendDragAndDropValue(UnityEngine.Object obj, SerializedProperty list)
            {
                appendDragDropParams = GetParams(ref appendDragDropParams, 1);
                appendDragDropParams[0] = obj;
                appendDragDrop.Invoke(list, appendDragDropParams);
            }

            private static object[] GetParams(ref object[] parameters, int count)
            {
                if (parameters == null)
                {
                    parameters = new object[count];
                }

                return parameters;
            }
        }
    }

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributeDrawer : PropertyDrawer
    {
        // Necessary since some properties tend to collapse smaller than their content
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        // Draw a disabled property field
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false; // Disable fields
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true; // Enable fields
        }
    }
}
#endif