/****************************************************************************
 * Copyright (c) 2018.3 ~ 7 liangxie
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
using DigitalRuby.RainMaker;

namespace QFramework.PlatformRunner
{
	/// <summary>
	/// 摄像机教程
	/// </summary>
	public class CameraCtrl : QMonoBehaviour
	{
		public override IManager Manager
		{
			get { return GameManager.Instance; }
		}
		
		public Camera    mCamera;
		public Transform BgLayer;
		public Transform RainTrans;

//		public ParticleSystem SunEmitter;

//		public  TweenPosition            shakeEffect;
		private ResLoader mResLoader = ResLoader.Allocate();

		private const float originalSize = 3.7f;
		private float         mScreenScale  = 0.0f;

		float bgScale = 1.0f; // to 2.25
		/*
		 * camera.size 4 - 6.2
		 * camera.posY 1.5 - 4
		 * player      0.4 - 7
		 */
		

		void Start()
		{
			AppLoader.Container.RegisterInstance(this);

//			shakeEffect = GetComponent<TweenPosition>();

			mScreenScale = (1136.0f / 640.0f) / (Screen.width * 1.0f / Screen.height * 1.0f);

			mCamera = GetComponent<Camera>();
			mCamera.orthographicSize = originalSize * mScreenScale;

			Debug.LogWarning(Screen.width / Screen.height + " width:" + Screen.width + " height:" + Screen.height);

			BgLayer = transform.Find("BgLayer");

//			SunEmitter = mResLoader.LoadSync<GameObject>("SUN")
//				.Instantiate()
//				.transform
//				.Parent(transform)
//				.GetComponent<ParticleSystem>()
//				.ApplySelfTo(selfScript => selfScript.Pause())
//				.Show();
//
//			RainTrans = mResLoader.LoadSync<GameObject>("RAINPREFAB2D")
//				.Instantiate()
//				.transform
//				.Parent(transform)
//				.Hide();
		}

		private float mPlayerPosY;
		private float mAspectOfPlayer;

		private float mDstCameraSize;
		private float mDstCameraPosY;
		private float mDstBgScale;

		// Update is called once per frame
		private void Update()
		{
			if (!BgLayer)
				return;
//			mPlayerPosY = GameManager.Instance.playerCtrl.transform.localPosition.y;

			if (mPlayerPosY > 0.4f)
			{
				mAspectOfPlayer = (mPlayerPosY - 0.4f) / (7.0f - 0.4f);
				mDstCameraSize = originalSize + mAspectOfPlayer * (6.2f - originalSize);
				mDstCameraPosY = 1.5f + mAspectOfPlayer * (4 - 1.5f);
				mDstBgScale = (2.25f - 1.5f) * mAspectOfPlayer + 1.0f;

				mCamera.orthographicSize += (mDstCameraSize * mScreenScale - mCamera.orthographicSize) * Time.deltaTime;

				transform.Translate(Vector3.up * (mDstCameraPosY - transform.localPosition.y) * Time.deltaTime * 2.0f);

				BgLayer.localScale +=
					new Vector3(1.0f, 1.0f, 0.0f) * (mDstBgScale - BgLayer.localScale.x) * Time.deltaTime;

				if (GameManager.Instance.bgCtrl.CurBgTrans != null)
				{
					GameManager.Instance.bgCtrl.CurBgTrans.GetComponent<IBg>()
						.LerpPosY(Mathf.Abs(BgLayer.localScale.x - 1.0f));
				}
			}
		}

		protected override void OnBeforeDestroy()
		{
			base.OnBeforeDestroy();

			mResLoader.Recycle2Cache();
			mResLoader = null;
		}

		public void Rain()
		{
			RainTrans.Show();
			RainTrans.GetComponent<RainScript2D>().enabled = true;

//			SunEmitter.Pause();
		}

		public void Sun()
		{
			RainTrans.GetComponent<RainScript2D>().enabled = false;
			RainTrans.Hide();
//			SunEmitter.Pause();
		}

		public void Idle()
		{
			RainTrans.GetComponent<RainScript2D>().enabled = false;
			RainTrans.Hide();
//			SunEmitter.Pause();
		}
	}
}