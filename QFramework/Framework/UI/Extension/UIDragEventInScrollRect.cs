/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
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

namespace QFramework
{
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Events;
	using UnityEngine.UI;

	public class UIDragEventInScrollRect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public enum State
		{
			Idle,
			Scroll,
			Drag,
		}

		private State mCurState = State.Idle;

		private UnityAction mOnDragBeganEvent = null;
		private UnityAction mOnDragEvent = null;
		private UnityAction mOnDragEndedEvent = null;
		private UnityAction mOnScrollEvent = null;
		private UnityAction mOnScrollEndedEvent = null;

		public ScrollRect ScrollRect { set; protected get; }

		public void SetState(State state)
		{
			mCurState = state;
		}

		public void RegisterOnDragBeganEvent(UnityAction onDragBegan)
		{
			mOnDragBeganEvent = onDragBegan;
		}

		public void RegisterOnDragEvent(UnityAction onDragEvent)
		{
			mOnDragEvent = onDragEvent;
		}

		public void RegisterOnDragEndedEvent(UnityAction onDragEndedEvent)
		{
			mOnDragEndedEvent = onDragEndedEvent;
		}

		public void RegisterOnScrollEvent(UnityAction onScrollEvent)
		{
			mOnScrollEvent = onScrollEvent;
		}

		public void RegisterOnScrollEndedEvent(UnityAction onScrollEndedEvent)
		{
			mOnScrollEndedEvent = onScrollEndedEvent;
		}

		private UnityEngine.Vector2 mDragBeganMousePos;

		private bool mCurrentDragCalculated = false;


		/// <summary>
		/// check if drag valid
		/// </summary>
		private bool mDragBegan = false;
		
		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			if (!QUIEventUtil.Interactable(gameObject)) return;
			UIEventLockManager.Instance.SendMsg(new UILockObjEventMsg(gameObject));

			mCurrentDragCalculated = false;
			mDragBeganMousePos = (transform.parent as RectTransform).GetLocalPosInRect();

			mDragBegan = true;
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			if (!QUIEventUtil.Interactable(gameObject) || !mDragBegan) return;

			if (!mCurrentDragCalculated && mCurState == State.Idle)
			{
				var offsetFromBegan = (transform.parent as RectTransform).GetLocalPosInRect() - mDragBeganMousePos;
				if (offsetFromBegan.magnitude > 10)
				{
					mCurrentDragCalculated = true;

					if (offsetFromBegan.x > 0 && offsetFromBegan.x > Mathf.Abs(offsetFromBegan.y))
					{
						mCurState = State.Drag;
						mOnDragBeganEvent.InvokeGracefully();
					}
					else
					{
						mCurState = State.Scroll;
						if (null != ScrollRect)
						{
							ExecuteEvents.Execute<ScrollRect>(ScrollRect.gameObject, eventData,
								delegate(ScrollRect handler, BaseEventData data)
								{
									handler.OnBeginDrag(data as PointerEventData);
								});
						}
					}

					return;
				}
			}
			else if (!mCurrentDragCalculated && mCurState == State.Drag)
			{
				mOnDragBeganEvent.InvokeGracefully();
				mCurrentDragCalculated = true;
				return;
			}

			switch (mCurState)
			{
				case State.Drag:
					mOnDragEvent.InvokeGracefully();
					break;
				case State.Scroll:
					if (null != ScrollRect)
					{
						mOnScrollEvent.InvokeGracefully();

						ExecuteEvents.Execute<ScrollRect>(ScrollRect.gameObject, eventData,
							delegate(ScrollRect handler, BaseEventData data)
							{
								handler.OnDrag(data as PointerEventData);
							});

					}
					break;
			}
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			if (!QUIEventUtil.Interactable(gameObject) || !mDragBegan) return;
			
			UIEventLockManager.Instance.SendMsg(new UIUnlockObjEventMsg(gameObject));

			if (mCurState == State.Scroll)
			{
				if (null != ScrollRect)
				{
					mOnScrollEndedEvent.InvokeGracefully();

					ExecuteEvents.Execute<ScrollRect>(ScrollRect.gameObject, eventData,
						delegate(ScrollRect handler, BaseEventData data)
						{
							handler.OnEndDrag(data as PointerEventData);
						});

				}
				mCurState = State.Idle;
			}
			else
			{
				mOnDragEndedEvent.InvokeGracefully();
			}
			mDragBegan = false;
		}
	}
}