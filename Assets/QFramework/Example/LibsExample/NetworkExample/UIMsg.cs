﻿/****************************************************************************
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
using UnityEngine.UI;
using QFramework;
using UniRx;

namespace QFramework
{
    public enum UIEvent
    {
        Msg=QMgrID.UI,
    }

	public class UIMsgData : UIPanelData
	{
		// TODO: Query Mgr's Data
	}

	public partial class UIMsg : UIPanel
	{
		protected override void InitUI(IUIData uiData = null)
		{
		}

		protected override void ProcessMsg (int eventId,QMsg msg)
		{
            switch (eventId)
            {
                case (int)UIEvent.Msg:
                    Text.text +=(msg as SocketMsg).Msg;
                    break;
            }
		}

        protected override void RegisterUIEvent()
        {
            RegisterEvent(UIEvent.Msg);

            Button.OnClickAsObservable().Where(x => InputField.text != null).Subscribe(

                _ => PCConnectMobileManager.Instance.SendMsg(new SocketMsg() { EventID = (int)PCConnectMobileEvent.SocketEvent, ToEventID = (int)UIEvent.Msg, Msg = InputField.text}
                
                ));
        }

		protected override void OnShow()
		{
			base.OnShow();
		}

		protected override void OnHide()
		{
			base.OnHide();
		}

		protected override void OnClose()
		{
		}

		void ShowLog(string content)
		{
			Debug.Log("[ UIMsg:]" + content);
		}
	}
}