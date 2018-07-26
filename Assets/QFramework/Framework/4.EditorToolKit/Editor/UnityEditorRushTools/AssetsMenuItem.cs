/****************************************************************************
 * Copyright (c) 2017 ~ 2018.7 Karsion
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
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

using UnityEditor;
using UnityEngine;

namespace QFramework
{

    /*-------------------------------------------------------------------------------------------------------------------------------------------
        To define a hot-key, use the following special characters: 
            % (Ctrl on Windows, Cmd on OS X)
            # (Shift)
            & (Alt)
            _ (no key modifiers). 
        For example, to define the hot-key Shift-Alt-g use "#&g". To define the hot-key g and no key modifiers pressed use "_g".
        Some special keys are supported, for example "#LEFT" would map to Shift-left. The keys supported like this are: 
            LEFT, RIGHT, UP, DOWN, F1 .. F12, HOME, END, PGUP, PGDN.
        eg. [MenuItem("GameObject/UI/img &g")]
    -------------------------------------------------------------------------------------------------------------------------------------------*/

    internal static class AssetsMenuItem
    {
        [MenuItem("Assets/Copy Name")]
        private static void CopyName()
        {
            TextEditor te = new TextEditor();
            te.text = Selection.activeObject.name;
            te.OnFocus();
            te.Copy();
        }
    }
}