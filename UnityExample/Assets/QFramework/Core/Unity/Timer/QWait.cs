/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
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
    using System;
    using System.Collections;

    /// <summary>
    /// TODO:找个时间测试下性能 对比QWaitForSeconds
    /// </summary>
    public class QWait
    {
        /// <summary>
        /// replace new WaitForSeconds
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static IEnumerator ForSeconds(float seconds)
        {
            yield return Observable.Timer(TimeSpan.FromSeconds(seconds)).ToYieldInstruction();
        }

        public static IEnumerator ForFrames(int frameCount)
        {
            yield return Observable.TimerFrame(frameCount).ToYieldInstruction();
        }

        /// <summary>
        /// replace new WaitUntil
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public static IEnumerator Until(Func<bool> match)
        {
            while (!match())
            {
                yield return null;
            }
        }
    }
}