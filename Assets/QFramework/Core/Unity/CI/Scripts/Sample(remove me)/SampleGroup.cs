/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
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
using System.Collections;
using QFramework;

// using RuntimeUnitTestToolkit;
using RuntimeUnitTestToolkit;

namespace SampleUnitTest
{
    // make unit test on plain C# class
    public class SampleGroup
    {
        // all public methods are automatically registered in test group
        public void SumTest()
        {
            var x = int.Parse("100");
            var y = int.Parse("200");

            // using RuntimeUnitTestToolkit;
            // 'Is' is Assertion method, same as Assert(actual, expected)
            (x + y).AssertIs(300);
        }

        public void TestAPI1()
        {
            for (var i = 0; i < 100; i++)
            {
                var testObject = new GameObject("Test1");
                GameObject.Destroy(testObject);
            }

        }

        public void TestAPI2()
        {
            for (var i = 0; i < 100; i++)
            {
                var testObject = new GameObject("Test2");
                testObject.DestroySelf();
            }
        }
        
        public void TestAPI3()
        {
            for (var i = 0; i < 100; i++)
            {
                var testObject = new GameObject("Test3");
                testObject.DestroySelfGracefully();
            }
        }

        // return type 'IEnumerator' is marked as async test method
        public IEnumerator AsyncTest()
        {
            var testObject = new GameObject("Test");

            // wait asynchronous coroutine(If import UniRx, you can use MainThreadDispatcher.StartCoroutine)
            var e = MoveToRight(testObject);
            while (e.MoveNext())
            {
                yield return null;
            }

            // assrtion
            testObject.transform.position.x.AssertIs(60);

            GameObject.Destroy(testObject);
        }

        IEnumerator MoveToRight(GameObject o)
        {
            for (var i = 0; i < 60; i++)
            {
                var p = o.transform.position;
                p.x += 1;
                o.transform.position = p;
                yield return null;
            }
        }
    }
}