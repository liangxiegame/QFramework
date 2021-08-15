/****************************************************************************
 * Copyright (c) 2021.4 liangxie
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

namespace QFramework
{
    public class EventBrowser : EasyEditorWindow
    {
        public static void Open(Action<Type> onTypeClick)
        {
            Create<EventBrowser>(true)
                .OnTypeClick(onTypeClick)
                .Show();
        }

        private Action<Type> mOnTypeClick;

        private EventBrowser OnTypeClick(Action<Type> onTypeClick)
        {
            mOnTypeClick = onTypeClick;
            return this;
        }

        public override void OnClose()
        {
        }

        public override void OnUpdate()
        {
        }


        List<ActionTuple<string, IButton>> AllEventViews = new List<ActionTuple<string, IButton>>();

        protected override void Init()
        {
            this.AddChild(EasyIMGUI.TextField().Self(t =>
            {
                t.Content.Bind(c =>
                {
                    if (c.IsNullOrEmpty())
                    {
                        AllEventViews.ForEach(a => a.Item2.Visible = true);
                    }
                    else
                    {
                        AllEventViews.ForEach(a => a.Item2.Visible = a.Item1.ToLower().Contains(c.ToLower()));
                    }
                });
            }));

            var scroll = EasyIMGUI.Scroll();

            foreach (var group in EventTypeDB.GetAll()
                .Where(t => t.GetFirstAttribute<OnlyUsedByCodeAttribute>(false) == null).GroupBy(t =>
                {
                    var attribute = t.GetFirstAttribute<ActionGroupAttribute>(false);

                    return attribute != null ? attribute.GroupName : "未分组";
                })
                .OrderBy(g => g.Key == "未分组"))
            {
                var treeNode = new TreeNode(true, group.Key);

                foreach (var type in @group.OrderBy(t => t.Name))
                {
                    var actionType = type;
                    treeNode.Add2Spread(EasyIMGUI.Button()
                        .OnClick(() =>
                        {
                            mOnTypeClick(actionType);
                            Close();
                        })
                        .Text(type.Name)
                        .Self(button => AllEventViews.Add(new ActionTuple<string, IButton>(type.Name, button))));
                }

                scroll.AddChild(treeNode);
            }
            
            this.AddChild(scroll);
        }
    }
}
