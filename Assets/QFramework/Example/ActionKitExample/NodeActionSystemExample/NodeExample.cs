/****************************************************************************
 * Copyright (c) 2018.3 布鞋 827922094@qq.com
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

using UnityEngine;

namespace QFramework
{

    public class NodeExample : MonoBehaviour
    {

        void Start()
        {
            this.Sequence()
               .Until(() => { return Input.GetKeyDown(KeyCode.Space); })
               .Delay(2.0f)
               .Event(() => { Debug.Log("延迟两秒"); })
               .Delay(1f)
               .Event(() => { Debug.Log("延迟一秒"); })
               .Until(() => { return Input.GetKeyDown(KeyCode.A); })
               .Event(() =>
               {
                   this.Repeat()
                   .Delay(0.5f)
                   .Event(() => { Debug.Log("0.5s"); })
                   .Begin()
                   .DisposeWhen(() => { return Input.GetKeyDown(KeyCode.S); })
                   .OnDisposed(() => { Debug.Log("结束"); });
               })
                .Begin();
        }

        #region Update的情况
        //private float m_CurrentTime;
        //private bool isSpace = true;
        //private bool isBegin = false;
        //private bool isCanA = false;
        //private bool isA = false;
        //private bool isRepeatS = false;

        //private void Start()
        //{
        //    m_CurrentTime = Time.time;
        //}

        //private void Update()
        //{
        //    if (isSpace && Input.GetKeyDown(KeyCode.Space))
        //    {
        //        isSpace = false;
        //        isBegin = true;
        //        m_CurrentTime = Time.time;
        //    }

        //    if (isA && Input.GetKeyDown(KeyCode.A))
        //    {
        //        isA = false;
        //        isRepeatS = true;
        //        m_CurrentTime = Time.time;
        //    }

        //    if (isRepeatS)
        //    {
        //        if (Time.time - m_CurrentTime > 0.5f)
        //        {
        //            m_CurrentTime = Time.time;

        //            Debug.Log("0.5s");

        //        }

        //        if (Input.GetKeyDown(KeyCode.S))
        //        {
        //            Debug.Log("结束");
        //            isRepeatS = false;
        //        }
        //    }

        //    if (isBegin)
        //    {
        //        if (Time.time - m_CurrentTime > 2)
        //        {
        //            Debug.Log("延迟两秒");
        //            isBegin = false;
        //            isCanA = true;
        //            m_CurrentTime = Time.time;
        //        }
        //    }

        //    if (isCanA)
        //    {
        //        if (Time.time - m_CurrentTime > 1)
        //        {
        //            Debug.Log("延迟一秒");
        //            isCanA = false;
        //            isA = true;
        //        }
        //    }

        //}
        #endregion
    }
}
