/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public interface IGenericMenu
    {
    }

    internal class GenericMenuView : IGenericMenu
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
}
#endif