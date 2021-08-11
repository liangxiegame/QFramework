/****************************************************************************
 * Copyright (c) 2017 ~2021.1 liangxie
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

namespace QFramework
{
    public abstract class QMgrID
    {
        public const int Framework = 0;
        public const int UI = Framework + QMsgSpan.Count; // 3000
        public const int Audio = UI + QMsgSpan.Count; // 6000
        public const int Network = Audio + QMsgSpan.Count;
        public const int UIFilter = Network + QMsgSpan.Count;
        public const int Game = UIFilter + QMsgSpan.Count;
        public const int PCConnectMobile = Game + QMsgSpan.Count;
        public const int FrameworkEnded = PCConnectMobile + QMsgSpan.Count;
        public const int FrameworkMsgModuleCount = 7;
    }
}