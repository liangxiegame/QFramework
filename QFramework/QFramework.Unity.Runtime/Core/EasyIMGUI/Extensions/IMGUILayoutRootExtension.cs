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

namespace QFramework
{
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
}