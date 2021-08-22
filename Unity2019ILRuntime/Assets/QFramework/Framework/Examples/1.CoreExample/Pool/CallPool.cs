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
    public class CallPool : MonoBehaviour
    {
        class Fish
        {
            
        }
        
        private void Start()
        {
            #region SimpleObjectPool
            var pool = new SimpleObjectPool<Fish>(() => new Fish(),initCount:50);
            
            pool.CurCount.LogInfo();

            var fish = pool.Allocate();
            
            pool.CurCount.LogInfo();

            pool.Recycle(fish);
         
            pool.CurCount.LogInfo();
            #endregion



            #region SafeObjectPool

            SafeObjectPool<Bullet>.Instance.Init(50,25);
            
            var bullet = Bullet.Allocate();

            SafeObjectPool<Bullet>.Instance.CurCount.LogInfo();
            
            bullet.Recycle2Cache();
            
            SafeObjectPool<Bullet>.Instance.CurCount.LogInfo();

            #endregion
        }
        
        
        class Bullet :IPoolable,IPoolType
        {
            public void OnRecycled()
            {
                "回收了".LogInfo();
            }

            public  bool IsRecycled { get; set; }

            public static Bullet Allocate()
            {
                return SafeObjectPool<Bullet>.Instance.Allocate();
            }
            
            public void Recycle2Cache()
            {
                SafeObjectPool<Bullet>.Instance.Recycle(this);
            }
        }

    }
}