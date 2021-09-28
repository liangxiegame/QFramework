/****************************************************************************
 * Copyright (c) 2017 xiaojun
 * Copyright (c) 2017 ~2021.1 liangxie
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


using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace QFramework
{
	using UnityEngine;

	[MonoSingletonPath("[Event]/QMsgCenter")]
	public partial class QMsgCenter : MonoBehaviour, ISingleton
	{
		public static QMsgCenter Instance
		{
			get { return MonoSingletonProperty<QMsgCenter>.Instance; }
		}

		public void OnSingletonInit()
		{

		}

		public void Dispose()
		{
			mRegisteredManagers.Clear();
			
			MonoSingletonProperty<QMsgCenter>.Dispose();
		}

		void Awake()
		{
			DontDestroyOnLoad(this);
		}


		public void SendMsg(IMsg tmpMsg)
		{

			foreach (var manager in mRegisteredManagers)
			{
				if (manager.Key == tmpMsg.ManagerID)
				{
					manager.Value().SendMsg(tmpMsg);
					return;
				}
			}

			ForwardMsg(tmpMsg as QMsg);
		}

		private static Dictionary<int, Func<QMgrBehaviour>> mRegisteredManagers =
			new Dictionary<int, Func<QMgrBehaviour>>();
		
		public static void RegisterManagerFactory(int mgrId, Func<QMgrBehaviour> managerFactory)
		{
			if (mRegisteredManagers.ContainsKey(mgrId))
			{
				mRegisteredManagers[mgrId] = managerFactory;
			}
			else
			{
				mRegisteredManagers.Add(mgrId, managerFactory);
			}
		}


		partial void ForwardMsg(QMsg tmpMsg);
	}
}