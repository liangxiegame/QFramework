using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UniRx;

public class callPool : MonoBehaviour {

    List<PoolTest> m_ObjList = new List<PoolTest>();

    private void Start()
    {
        this.Repeat()
            .Until(() => { return Input.GetKeyDown(KeyCode.Space); })
            .Event(() =>
            {
                PoolTest temp = SafeObjectPool<PoolTest>.Instance.Allocate();
                temp.DebugIndex();
                m_ObjList.Add(temp);
            })
            .Begin();

        Observable.EveryUpdate()
            .Where(x => Input.GetKeyDown(KeyCode.C) && m_ObjList.Count > 0)
            .Subscribe(_ => { SafeObjectPool<PoolTest>.Instance.Recycle(m_ObjList[0]); m_ObjList.RemoveAt(0); Debug.Log("回收"); });

    }
}
