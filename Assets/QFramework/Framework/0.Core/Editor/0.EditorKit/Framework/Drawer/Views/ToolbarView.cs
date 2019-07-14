
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EGO.Framework
{
    public class ToolbarView : View
    {
        public ToolbarView(int defaultIndex = 0)
        {
            Index.Value = 0;
            Index.Bind(index => MenuSelected[index].Invoke(MenuNames[index]));
            //Style = new GUIStyle(GUI.skin.button);
        }

        public ToolbarView AddMenu(string name,Action<string> onMenuSelected)
        {
            MenuNames.Add(name);
            MenuSelected.Add(onMenuSelected);
            return this;
        }

        List<string> MenuNames = new List<string>();

        List<Action<string>> MenuSelected = new List<Action<string>>();

        Property<int> Index = new Property<int>(0);

        protected override void OnGUI()
        {
            Index.Value = GUILayout.Toolbar(Index.Value, MenuNames.ToArray(), GUI.skin.button, LayoutStyles);
        }
    }
}