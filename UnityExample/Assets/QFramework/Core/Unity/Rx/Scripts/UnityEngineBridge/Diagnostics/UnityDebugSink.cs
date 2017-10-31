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
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    
    
    public class UnityDebugSink : IObserver<LogEntry>
    {
        public void OnCompleted()
        {
            // do nothing
        }

        public void OnError(Exception error)
        {
            // do nothing
        }

        public void OnNext(LogEntry value)
        {
            // avoid multithread exception.
            // (value.Context == null) can only be called from the main thread.
            var ctx = (System.Object)value.Context;

            switch (value.LogType)
            {
                case LogType.Error:
                    if (ctx == null)
                    {
                        Debug.LogError(value.Message);
                    }
                    else
                    {
                        Debug.LogError(value.Message, value.Context);
                    }
                    break;
                case LogType.Exception:
                    if (ctx == null)
                    {
                        Debug.LogException(value.Exception);
                    }
                    else
                    {
                        Debug.LogException(value.Exception, value.Context);
                    }
                    break;
                case LogType.Log:
                    if (ctx == null)
                    {
                        Debug.Log(value.Message);
                    }
                    else
                    {
                        Debug.Log(value.Message, value.Context);
                    }
                    break;
                case LogType.Warning:
                    if (ctx == null)
                    {
                        Debug.LogWarning(value.Message);
                    }
                    else
                    {
                        Debug.LogWarning(value.Message, value.Context);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}