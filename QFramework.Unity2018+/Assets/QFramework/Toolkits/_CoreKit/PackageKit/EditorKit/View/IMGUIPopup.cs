/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;

namespace QFramework
{
    public interface IPopup : IMGUIView
    {
        IPopup WithIndexAndMenus(int index, params string[] menus);

        IPopup OnIndexChanged(Action<int> indexChanged);

        IPopup ToolbarStyle();

        BindableProperty<int> IndexProperty { get; }
        IPopup Menus(List<string> value);
    }

    public class PopupView : IMGUIAbstractView, IPopup
    {
        protected PopupView()
        {
            mStyle = new FluentGUIStyle(() => EditorStyles.popup);
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
                EditorGUILayout.Popup(IndexProperty.Value, mMenus, mStyle.Value, LayoutStyles);
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
            mStyle = new FluentGUIStyle(() => EditorStyles.toolbarPopup);
            return this;
        }
    }
}
#endif