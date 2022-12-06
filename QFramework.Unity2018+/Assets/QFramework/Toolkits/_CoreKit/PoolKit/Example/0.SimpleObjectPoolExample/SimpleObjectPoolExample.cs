using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class SimpleObjectPoolExample : MonoBehaviour
    {
        private SimpleObjectPool<GameObject> mObjectPool;

        void Start()
        {
            mObjectPool = new SimpleObjectPool<GameObject>(() =>
            {
                var gameObj = new GameObject();
                gameObj.SetActive(false);
                return gameObj;
            }, gameObj => { gameObj.SetActive(false); }, 5);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var obj = mObjectPool.Allocate();
                obj.SetActive(true);
            }

            if (Input.GetMouseButtonDown(1))
            {
                mObjectPool.Clear(o => { Destroy(o); });
            }
        }
    }
}