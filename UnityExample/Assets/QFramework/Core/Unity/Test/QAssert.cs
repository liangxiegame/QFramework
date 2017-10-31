/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * https://github.com/neuecc/ChainingAssertion
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
    using UnityEngine;
    using UnityEngine.Assertions;
	using System.Collections.Generic;

    /// <summary>
    /// 断言用来单元测试的
    /// </summary>
    public class QAssert : MonoBehaviour
    {
        public static void AreStringEqual(string a, string b, object msg = null)
        {
            Assert.IsTrue(string.Equals(a, b), msg == null ? "" : msg.ToString());
        }

        public static void AreArrayEqual(List<string> a, List<string> b)
        {
            a.Equals(b);
            if (a != null && b != null && a.Count == b.Count)
            {
                bool areEqual = true;
            }
        }
        
        /// <summary>
        /// Check that two list have the same content.
        /// </summary>
        public static void IsEqualList<T>(List<T> list1, List<T> list2)
        {
            Assert.AreEqual(list1.Count, list2.Count);
            for (int i = 0; i < list1.Count; i++)
            {
                Assert.AreEqual(list1[i], list2[i]);
            }
        }
        
        /// <summary>
        /// Check that two arrays have the same content.
        /// </summary>
        public static void IsEqualArrays<T>(T[] array1, T[] array2)
        {
            Assert.AreEqual(array1.Length, array2.Length);
            for (int i = 0; i < array1.Length; i++)
            {
                Assert.AreEqual(array1[i], array2[i]);
            }
        }
    }
}