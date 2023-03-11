/****************************************************************************
 * Copyright (c) 2018 ~ 2022 UNDER MIT Lisence 布鞋 827922094@qq.com
 * 
 * https://qframework.cn
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


            var gameObjPool = new SimpleObjectPool<GameObject>(() =>
            {
                var gameObj = new GameObject("AGameObject");
                // init gameObj code 

                // gameObjPrefab = Resources.Load<GameObject>("somePath/someGameObj");
                
                return gameObj;
            }, (gameObj) =>
            {
                // reset code here
            });
            #endregion



            #region SafeObjectPool

            SafeObjectPool<Bullet>.Instance.SetFactoryMethod(() =>
            {
                // bullet can be mono behaviour
                return new Bullet();
            });
            
            SafeObjectPool<Bullet>.Instance.SetObjectFactory(new DefaultObjectFactory<Bullet>());
            
            
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