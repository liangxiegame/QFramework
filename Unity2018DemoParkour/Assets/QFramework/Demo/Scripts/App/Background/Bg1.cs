/****************************************************************************
 * Copyright (c) 2018.7  ~ 2021.8 liangxie
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

namespace QFramework.PlatformRunner
{
	/// <summary>
	/// Bg1
	/// </summary>
	public class Bg1 : QMonoBehaviour, IBg,IController
	{
		private IPlayerModel mPlayerModel;
		
		public override IManager Manager
		{
			get { return GameManager.Instance; }
		}

		private Transform bg1BackA;
		private Transform bg1ForeA;

		private Transform bg1BackB;
		private Transform bg1ForeB;

		private const float DistanceForeY = 2.0f;

		private bool mBNewObj = true; // 刚创建

		private float mOringinForeAPosY = 0.0f;
		private float mOringinForeBPosY = 0.0f;

		private const float Width = 11.35f;


		/// <summary>
		/// Awake
		/// </summary>
		private void Awake()
		{
			bg1BackA = transform.Find("bg1_backA");
			bg1ForeA = transform.Find("bg1_foreA");

			bg1BackB = transform.Find("bg1_backB");
			bg1ForeB = transform.Find("bg1_foreB");

			mPlayerModel = this.GetModel<IPlayerModel>();
		}

		/// <summary>
		/// Start
		/// </summary>
		private void Start()
		{
			InitBg();
		}

		/// <summary>
		/// 初始化位置信息
		/// </summary>
		public void InitBg()
		{
			bg1BackA.PositionX(0.0f);
			bg1ForeA.PositionX(0.0f);

			bg1BackB.PositionX(Width);
			bg1ForeB.PositionX(Width);

			this.PositionX(0.0f);
			this.PositionY(0.3f);

			// 第一次创建对象,获取数据。
			if (mBNewObj)
			{
				mBNewObj = false;
				mOringinForeAPosY = bg1ForeA.position.x;
				mOringinForeBPosY = bg1ForeB.position.y;
			}

			this.LocalRotationIdentity();
			this.LocalScale(1.2f, 0.7f, 1.0f);
		}

		/// <summary>
		/// 插值，根据摄像头的放大缩小来确定
		/// </summary>
		public void LerpPosY(float lerpValue)
		{
			bg1ForeA.PositionY(mOringinForeAPosY - DistanceForeY * lerpValue);
			bg1ForeB.PositionY(mOringinForeBPosY - DistanceForeY * lerpValue);
		}

		private float mCachedSpeed = 0.0f;

		/// <summary>
		/// Update
		/// </summary>
		private void Update()
		{
			mCachedSpeed = mPlayerModel.CurSpeed.Value;

			bg1BackA.Translate(Vector3.left * mCachedSpeed * 0.125f * Time.deltaTime, Space.World);
			bg1BackB.Translate(Vector3.left * mCachedSpeed * 0.125f * Time.deltaTime, Space.World);

			bg1ForeA.Translate(Vector3.left * mCachedSpeed * 0.5f * Time.deltaTime, Space.World);
			bg1ForeB.Translate(Vector3.left * mCachedSpeed * 0.5f * Time.deltaTime, Space.World);

			Bg.Check(bg1BackA, bg1BackB, bg1ForeA, bg1ForeB, Width);
		}

		/// <summary>
		/// 背景消除
		/// </summary>
		void IBg.Despawn()
		{
//		PoolManager.Pools["Stage1"].Despawn(transform);
		}

		protected override void OnBeforeDestroy()
		{
			base.OnBeforeDestroy();

			mPlayerModel = null;
		}

		public IArchitecture GetArchitecture()
		{
			return PlatformRunner.Interface;
		}
	}
}