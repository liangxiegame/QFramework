/****************************************************************************
 * Copyright (c) 2018.7 ~ 2021.8 liangxie
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
	public class Bg4 : QMonoBehaviour, IBg,IController
	{
		public override IManager Manager
		{
			get { return GameManager.Instance; }
		}

		private Transform bg4BackA;
		private Transform bg4ForeA;

		private Transform bg4BackB;
		private Transform bg4ForeB;

		private const float DistanceForeY = 2.0f;

		private bool  mNewObj = true; // 是不是刚创建得对象
		private float mOringinForeAPosY;
		private float mOringinForeBPosY;

		private IPlayerModel mPlayerModel;

		private const float Width = 12.8f;

		private void Awake()
		{
			bg4BackA = transform.Find("bg4_backA");
			bg4ForeA = transform.Find("bg4_foreA");

			bg4BackB = transform.Find("bg4_backB");
			bg4ForeB = transform.Find("bg4_foreB");

			mPlayerModel = this.GetModel<IPlayerModel>();
		}

		private void Start()
		{
			InitBg();
		}

		/// <summary>
		/// 初始化位置数据
		/// </summary>
		public void InitBg()
		{
			bg4BackA.PositionX(0.0f);
			bg4ForeA.PositionX(0.0f);

			bg4BackB.PositionX(Width);
			bg4ForeA.PositionX(Width);

			transform.Position(0.0f, 0.3f);

			if (mNewObj)
			{
				mOringinForeAPosY = bg4ForeA.GetPosition().y;
				mOringinForeBPosY = bg4ForeB.GetPosition().y;

				mNewObj = false;
			}

			this.LocalRotationIdentity();
			this.LocalScale(1.2f, 0.7f, 1.0f);
		}

		public void LerpPosY(float lerpValue)
		{
			bg4ForeA.PositionY(mOringinForeAPosY - DistanceForeY * lerpValue);
			bg4ForeB.PositionY(mOringinForeBPosY - DistanceForeY * lerpValue);
		}

		float Speed = 0.0f;

		private void Update()
		{
			Speed = mPlayerModel.CurSpeed.Value;

			bg4BackA.Translate(Vector3.left * Speed * 0.125f * Time.deltaTime, Space.World);
			bg4BackB.Translate(Vector3.left * Speed * 0.125f * Time.deltaTime, Space.World);

			bg4ForeA.Translate(Vector3.left * Speed * 0.5f * Time.deltaTime, Space.World);
			bg4ForeB.Translate(Vector3.left * Speed * 0.5f * Time.deltaTime, Space.World);

			Bg.Check(bg4BackA, bg4BackB, bg4ForeA, bg4ForeB, Width);
		}

		public void Despawn()
		{
//		PoolManager.Pools["Stage4"].Despawn(transform);
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