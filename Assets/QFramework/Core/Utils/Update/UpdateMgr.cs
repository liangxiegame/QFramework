/****************************************************************************
 * Copyright (c) 2017 Feiko Joosten
 * Copyright (c) 2017 liangxie
 * https://blogs.unity3d.com/cn/2015/12/23/1k-update-calls/
 * https://github.com/thexa4/UnityScheduler
 * 
 * http://liangxiegame.com
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

/// <summary>
/// Made by Feiko Joosten
/// 
/// I have based this code on this blogpost. Decided to give it more functionality. http://blogs.unity3d.com/2015/12/23/1k-update-calls/
/// Use this to speed up your performance when you have a lot of update, fixed update and or late update calls in your scene
/// Let the object you want to give increased performance inherit from OverridableMonoBehaviour
/// Replace your void Update() for public override void UpdateMe()
/// Or replace your void FixedUpdate() for public override void FixedUpdateMe()
/// Or replace your void LateUpdate() for public override void LateUpdateMe()
/// OverridableMonoBehaviour will add the object to the update manager
/// UpdateManager will handle all of the update calls
/// </summary>

namespace QFramework
{
	using UnityEngine;

	[QMonoSingletonPath("[Framework]/UpdateManager")]
	public class UpdateMgr : QMonoSingleton<UpdateMgr>
	{
		private readonly ResizeableArray<IUpdatable> mRegularArray = new ResizeableArray<IUpdatable>(0);
		private readonly ResizeableArray<IFixedUpdatable> mFixedArray = new ResizeableArray<IFixedUpdatable>(0);
		private readonly ResizeableArray<ILateUpdatable> mLateArray = new ResizeableArray<ILateUpdatable>(0);

		public static void AddItem(object updatableObj)
		{
			Instance.AddItemToArray(updatableObj);
		}

		public static void RemoveSpecificItem(object updatableObj)
		{
			Instance.RemoveSpecificItemFromArray(updatableObj);
		}

		public static void RemoveSpecificItemAndDestroyIt(object updatableObj)
		{
			Instance.RemoveSpecificItemFromArray(updatableObj);

			if (updatableObj is MonoBehaviour)
			{
				((MonoBehaviour) updatableObj).DestroyGameObjGracefully();
			}
		}

		private void AddItemToArray(object updatableObj)
		{
			if (updatableObj is IUpdatable)
			{
				mRegularArray.Append(updatableObj as IUpdatable);
			}

			if (updatableObj is IFixedUpdatable)
			{
				mFixedArray.Append(updatableObj as IFixedUpdatable);
			}

			if (updatableObj is ILateUpdatable)
			{
				mLateArray.Append(updatableObj as ILateUpdatable);
			}
		}

		private void RemoveSpecificItemFromArray(object updatableObj)
		{
			if (updatableObj is IUpdatable && mRegularArray.Contains(updatableObj as IUpdatable))
			{
				mRegularArray.Remove(updatableObj as IUpdatable);
			}

			if (updatableObj is IFixedUpdatable && mFixedArray.Contains(updatableObj as IFixedUpdatable))
			{
				mFixedArray.Remove(updatableObj as IFixedUpdatable);
			}

			if (updatableObj is ILateUpdatable && mLateArray.Contains(updatableObj as ILateUpdatable))
			{
				mLateArray.Remove(updatableObj as ILateUpdatable);
			}
		}

		private void Update()
		{
			if (mRegularArray.Count == 0) return;

			for (int i = 0; i < mRegularArray.Count; i++)
			{
				if (mRegularArray[i] == null) continue;

				mRegularArray[i].OnUpdate();
			}
		}

		private void FixedUpdate()
		{
			if (mFixedArray.Count == 0) return;

			for (int i = 0; i < mFixedArray.Count; i++)
			{
				if (mFixedArray[i] == null) continue;

				mFixedArray[i].OnFixedUpdate();
			}
		}

		private void LateUpdate()
		{
			if (mLateArray.Count == 0) return;

			for (int i = 0; i < mLateArray.Count; i++)
			{
				if (mLateArray[i] == null) continue;

				mLateArray[i].OnLateUpdate();
			}
		}
	}
}