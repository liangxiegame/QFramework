/****************************************************************************
 * Copyright (c) 2017 liangxie
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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework
{
	/// <summary>
	/// 动画播放控件
	/// http://www.cnblogs.com/mrblue/p/5191183.html
	/// </summary>
	[RequireComponent(typeof(Image))]
	public class UISpriteAnimation : MonoBehaviour
	{
		private Image mImageSource;
		private int mCurFrame = 0;
		private float mDelta = 0;

		public float FPS = 5;
		public List<Sprite> SpriteFrames;
		public bool IsPlaying = false;
		public bool Forward = true;
		public bool AutoPlay = false;
		public bool Loop = false;

		public int FrameCount
		{
			get { return SpriteFrames.Count; }
		}

		void Awake()
		{
			mImageSource = GetComponent<Image>();
		}

		void Start()
		{
			if (AutoPlay)
			{
				Play();
			}
			else
			{
				IsPlaying = false;
			}
		}

		private void SetSprite(int idx)
		{
			mImageSource.sprite = SpriteFrames[idx];
			mImageSource.SetNativeSize();
		}

		public void Play()
		{
			IsPlaying = true;
			Forward = true;
		}

		public void PlayReverse()
		{
			IsPlaying = true;
			Forward = false;
		}

		void Update()
		{
			if (!IsPlaying || 0 == FrameCount)
			{
				return;
			}

			mDelta += Time.deltaTime;
			if (mDelta > 1 / FPS)
			{
				mDelta = 0;
				if (Forward)
				{
					mCurFrame++;
				}
				else
				{
					mCurFrame--;
				}

				if (mCurFrame >= FrameCount)
				{
					if (Loop)
					{
						mCurFrame = 0;
					}
					else
					{
						IsPlaying = false;
						return;
					}
				}
				else if (mCurFrame < 0)
				{
					if (Loop)
					{
						mCurFrame = FrameCount - 1;
					}
					else
					{
						IsPlaying = false;
						return;
					}
				}

				SetSprite(mCurFrame);
			}
		}

		public void Pause()
		{
			IsPlaying = false;
		}

		public void Resume()
		{
			if (!IsPlaying)
			{
				IsPlaying = true;
			}
		}

		public void Stop()
		{
			mCurFrame = 0;
			SetSprite(mCurFrame);
			IsPlaying = false;
		}

		public void Rewind()
		{
			mCurFrame = 0;
			SetSprite(mCurFrame);
			Play();
		}
	}
}