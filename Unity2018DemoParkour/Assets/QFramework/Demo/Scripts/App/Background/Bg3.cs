/****************************************************************************
 * Copyright (c) 2018.3 ~ 2021.8 liangxie
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
	public class Bg3 : QMonoBehaviour, IBg,IController
	{
		public override IManager Manager
		{
			get { return GameManager.Instance; }
		}

		private Transform mBg3BackA;
		private Transform mBg3ForeA;

		private Transform bg3BackB;
		private Transform bg3ForeB;

		private float distanceForeY = 2.0f;

		private bool  m_bNewObj = true; // 是不是刚创建得对象
		private float oringinForeAPosY;
		private float oringinForeBPosY;

		private const float Width = 11.35f;

		private IPlayerModel mPlayerModel;

		private void Awake()
		{
			mBg3BackA = transform.Find("bg3_backA");
			mBg3ForeA = transform.Find("bg3_foreA");

			bg3BackB = transform.Find("bg3_backB");
			bg3ForeB = transform.Find("bg3_foreB");

			mPlayerModel = this.GetModel<IPlayerModel>();
		}

		private void Start()
		{
			InitBg();
		}

		public void InitBg()
		{
			mBg3BackA.PositionX(0.0f);
			mBg3ForeA.PositionX(0.0f);

			bg3BackB.PositionX(Width);
			bg3ForeB.PositionX(Width);

			transform.PositionX(0.0f);
			transform.PositionY(0.3f);

			if (m_bNewObj)
			{
				oringinForeAPosY = mBg3ForeA.GetPosition().y;
				oringinForeBPosY = bg3ForeB.GetPosition().y;

				m_bNewObj = false;
			}

			transform.LocalRotationIdentity();
			transform.LocalScale(1.2f, 0.7f, 1.0f);
		}

		public void LerpPosY(float lerpValue)
		{
			mBg3ForeA.PositionY(oringinForeAPosY - distanceForeY * lerpValue);
			bg3ForeB.PositionY(oringinForeBPosY - distanceForeY * lerpValue);
		}

		private float mSpeed;

		private PlayerModel mCachedPlayerModel;

		private void Update()
		{
			mSpeed = mPlayerModel.CurSpeed.Value;

			mBg3BackA.Translate(Vector3.left * mSpeed * 0.125f * Time.deltaTime, Space.World);
			bg3BackB.Translate(Vector3.left * mSpeed * 0.125f * Time.deltaTime, Space.World);

			mBg3ForeA.Translate(Vector3.left * mSpeed * 0.5f * Time.deltaTime, Space.World);
			bg3ForeB.Translate(Vector3.left * mSpeed * 0.5f * Time.deltaTime, Space.World);

			Bg.Check(mBg3BackA, bg3BackB, mBg3ForeA, bg3ForeB, Width);
		}

		public void Despawn()
		{
//		PoolManager.Pools["Stage3"].Despawn(transform);
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