/****************************************************************************
 * Copyright (c) 2017 liangxie
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

namespace QF
{
    using System;
    using System.Collections.Generic;
    
    public class SimpleObjectCache
    {
        private readonly Dictionary<Type, object> mObjectPools;

        public SimpleObjectCache()
        {
            mObjectPools = new Dictionary<Type, object>();
        }

        public SimpleObjectPool<T> GetObjectPool<T>() where T : new()
        {
            object objectPool;
            var type = typeof(T);
            if (!mObjectPools.TryGetValue(type, out objectPool))
            {
                objectPool = new SimpleObjectPool<T>(() => new T());
                mObjectPools.Add(type, objectPool);
            }

            return ((SimpleObjectPool<T>) objectPool);
        }

        public T Get<T>() where T : new()
        {
            return GetObjectPool<T>().Allocate();
        }

        public void Push<T>(T obj) where T : new()
        {
            GetObjectPool<T>().Recycle(obj);
        }

        public void RegisterCustomObjectPool<T>(SimpleObjectPool<T> simpleObjectPool)
        {
            mObjectPools.Add(typeof(T), simpleObjectPool);
        }

        public void Reset()
        {
            mObjectPools.Clear();
        }
    }
}