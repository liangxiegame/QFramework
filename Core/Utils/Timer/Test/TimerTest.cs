/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
****************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace QFramework
{
    public class TimerTest : MonoBehaviour
    {
        //private TimeItem m_RepeatTimeItem;

        private void Start()
        {
            Log.I(DateTime.Now);
            //m_RepeatTimeItem = Timer.S.Post2Really(OnTimeTick, 1, -1);
            //DateTime time = DateTime.Now;
            //time = time.AddSeconds(5);
            //Timer.S.Post2Really(OnDateTimeTick, time);

            //Time.timeScale = 0.5f;
            //Timer.S.Post2Scale(OnScaleTimeTick, 1, -1);
        }

        private void OnTimeTick(int tick)
        {
            Log.I("TickTick:" + DateTime.Now);
        }

        private void OnDateTimeTick(int tick)
        {
            Log.I("DateTimeTick:" + tick);
        }

        private void OnScaleTimeTick(int tick)
        {
            Log.I("ScaleTickTick:" + tick);
        }
    }
}
