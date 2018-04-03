using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{

    public class NodeExample : MonoBehaviour
    {

        void Start()
        {
            this.Sequence()
               .Until(() => { return Input.GetKeyDown(KeyCode.Space); })
               .Delay(2.0f)
               .Event(() => { Debug.Log("延迟两秒"); })
               .Delay(1f)
               .Event(() => { Debug.Log("延迟一秒"); })
               .Until(() => { return Input.GetKeyDown(KeyCode.A); })
               .Event(() =>
               {
                   this.Repeat()
                   .Delay(0.5f)
                   .Event(() => { Debug.Log("0.5s"); })
                   .Begin()
                   .DisposeWhen(() => { return Input.GetKeyDown(KeyCode.S); })
                   .OnDisposed(() => { Debug.Log("结束"); });
               })
               .Begin()
               .DisposeWhenFinished();
        }

        #region Update的情况
        //private float m_CurrentTime;
        //private bool isSpace = true;
        //private bool isBegin = false;
        //private bool isCanA = false;
        //private bool isA = false;
        //private bool isRepeatS = false;

        //private void Start()
        //{
        //    m_CurrentTime = Time.time;
        //}

        //private void Update()
        //{
        //    if (isSpace && Input.GetKeyDown(KeyCode.Space))
        //    {
        //        isSpace = false;
        //        isBegin = true;
        //        m_CurrentTime = Time.time;
        //    }

        //    if (isA && Input.GetKeyDown(KeyCode.A))
        //    {
        //        isA = false;
        //        isRepeatS = true;
        //        m_CurrentTime = Time.time;
        //    }

        //    if (isRepeatS)
        //    {
        //        if (Time.time - m_CurrentTime > 0.5f)
        //        {
        //            m_CurrentTime = Time.time;

        //            Debug.Log("0.5s");

        //        }

        //        if (Input.GetKeyDown(KeyCode.S))
        //        {
        //            Debug.Log("结束");
        //            isRepeatS = false;
        //        }
        //    }

        //    if (isBegin)
        //    {
        //        if (Time.time - m_CurrentTime > 2)
        //        {
        //            Debug.Log("延迟两秒");
        //            isBegin = false;
        //            isCanA = true;
        //            m_CurrentTime = Time.time;
        //        }
        //    }

        //    if (isCanA)
        //    {
        //        if (Time.time - m_CurrentTime > 1)
        //        {
        //            Debug.Log("延迟一秒");
        //            isCanA = false;
        //            isA = true;
        //        }
        //    }

        //}
        #endregion
    }
}
