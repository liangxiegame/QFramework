using System;
using System.Collections.Generic;
using EGO.Util;
using UnityEngine;

namespace EGO.Framework
{
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
            get { return mVisible;}
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
        
        protected virtual void OnShow(){}

        public void Hide()
        {
            Visible = false;
            OnHide();
        }

        protected virtual void OnHide(){}
        

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

        protected virtual void OnRefresh(){}

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
}