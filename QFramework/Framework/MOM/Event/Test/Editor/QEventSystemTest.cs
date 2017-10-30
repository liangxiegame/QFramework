/****************************************************************************
 * Copyright (c) 2017 yuanhuibin@putao.com
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

namespace QFramework.Test.Core
{
    using NUnit.Framework;
    
    public class EventSystemTest
    {
        [Test]
        public void EventSystemTest_Register()
        {
            int key = 10000;
            int registerCount = 0;
            bool rigisters1 = QEventSystem.Instance.Register(key, delegate
            {
                registerCount++;
            });
            QEventSystem.Instance.Send(key);
            Assert.AreEqual(1, registerCount);
            bool rigisters2 = QEventSystem.Instance.Register(key, delegate
            {
                registerCount++;
            });
            QEventSystem.Instance.Send(key);
            Assert.AreEqual(3, registerCount);
        }

        [Test]
        public void EventSystemTest_UnRegister()
        {
            int key = 20000;
            int registerCount = 0;
            bool rigisters = QEventSystem.Instance.Register(key, delegate
            {
                registerCount++;
            });
            QEventSystem.Instance.Send(key);
            Assert.AreEqual(1, registerCount);
            QEventSystem.Instance.UnRegister(key, delegate
            {
                registerCount--;
            });
            Assert.AreEqual(1, registerCount);
        }

        [Test]
        public void EventSystemTest_Send()
        {
            int key = 30000;
            int registerCount = 0;
            bool rigisters = QEventSystem.Instance.Register(key, delegate
            {
                registerCount++;
            });
            bool sendValue = true;
            bool backValue = QEventSystem.Instance.Send(key);
            Assert.AreEqual(sendValue, backValue);
        }
    }
}