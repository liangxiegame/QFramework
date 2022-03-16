/****************************************************************************
 * Copyright (c) 2018 ~ 2022 UNDER MIT Lisence 布鞋 827922094@qq.com
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
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

            Debug.Log(pool.CurCount);

            var fish = pool.Allocate();

            Debug.Log(pool.CurCount);

            pool.Recycle(fish);

            Debug.Log(pool.CurCount);
            #endregion



            #region SafeObjectPool

            SafeObjectPool<Bullet>.Instance.Init(50,25);
            
            var bullet = Bullet.Allocate();

            Debug.Log(SafeObjectPool<Bullet>.Instance.CurCount);
            
            bullet.Recycle2Cache();

            Debug.Log(SafeObjectPool<Bullet>.Instance.CurCount);

            #endregion
        }
        
        
        class Bullet :IPoolable,IPoolType
        {
            public void OnRecycled()
            {
                Debug.Log("回收了");
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
// 50
// 49
// 50
// 回收了 x 25
// 24
// 回收了 24
// 回收了
// 回收了 25