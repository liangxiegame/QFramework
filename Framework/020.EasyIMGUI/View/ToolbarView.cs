/****************************************************************************
 * Copyright (c) 2018 ~ 2020.10 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
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

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QFramework
{
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
}