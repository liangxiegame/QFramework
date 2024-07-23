/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEditor;

namespace QFramework
{
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
}
#endif