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
using System.Linq;
using UnityEngine;

namespace QFramework
{
    public interface IMGUIToolbar : IMGUIView
    {
        IMGUIToolbar Menus(List<string> menuNames);
        IMGUIToolbar AddMenu(string name, Action<string> onMenuSelected = null);

        BindableProperty<int> IndexProperty { get; }

        IMGUIToolbar Index(int index);
    }

    internal class IMGUIIMGUIToolbarView : IMGUIAbstractView, IMGUIToolbar
    {
        public IMGUIIMGUIToolbarView()
        {
            IndexProperty = new BindableProperty<int>(0);

            IndexProperty.Register(index =>
            {
                if (MenuSelected.Count > index)
                {
                    MenuSelected[index].Invoke(MenuNames[index]);
                }
            });

            Style = new FluentGUIStyle(() => GUI.skin.button);
        }

        public IMGUIToolbar Menus(List<string> menuNames)
        {
            this.MenuNames = menuNames;
            // empty
            this.MenuSelected = MenuNames.Select(menuName => new Action<string>((str => { }))).ToList();
            return this;
        }

        public IMGUIToolbar AddMenu(string name, Action<string> onMenuSelected = null)
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

        public IMGUIToolbar Index(int index)
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
}
#endif