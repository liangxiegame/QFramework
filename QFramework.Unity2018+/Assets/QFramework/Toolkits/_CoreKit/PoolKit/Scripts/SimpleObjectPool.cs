/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;

namespace QFramework
{
#if UNITY_EDITOR
    // v1 No.170
    [ClassAPI("06.PoolKit", "SimpleObjectPool<T>", 0, "SimpleObjectPool<T>")]
    [APIDescriptionCN("面向业务的对象池")]
    [APIDescriptionEN("simple object pool")]
    [APIExampleCode(@"
class Fish
{
             
}

var pool = new SimpleObjectPool<Fish>(() => new Fish(),initCount:50);
 
Debug.Log(pool.CurCount);
// 50 
var fish = pool.Allocate();
 
Debug.Log(pool.CurCount);
// 49
pool.Recycle(fish);

Debug.Log(pool.CurCount);
// 50


// ---- GameObject ----
var gameObjPool = new SimpleObjectPool<GameObject>(() =>
{
    var gameObj = new GameObject(""AGameObject"");
    // init gameObj code 

    // gameObjPrefab = Resources.Load<GameObject>(""somePath/someGameObj"");
                
    return gameObj;
}, (gameObj) =>
{
    // reset code here
});

// ---- Clear ----
gameObjPool.Clear(gameObj=> Object.Destroy(gameObk));
")]
#endif
    public class SimpleObjectPool<T> : Pool<T>
    {
        readonly Action<T> mResetMethod;

        public SimpleObjectPool(Func<T> factoryMethod, Action<T> resetMethod = null, int initCount = 0)
        {
            mFactory = new CustomObjectFactory<T>(factoryMethod);
            mResetMethod = resetMethod;

            for (var i = 0; i < initCount; i++)
            {
                mCacheStack.Push(mFactory.Create());
            }
        }

        public override bool Recycle(T obj)
        {
            mResetMethod?.Invoke(obj);

            mCacheStack.Push(obj);

            return true;
        }
    }
}