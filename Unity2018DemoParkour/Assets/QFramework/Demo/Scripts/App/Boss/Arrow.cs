/****************************************************************************
 * Copyright (c) 2018.7 liangxie
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
	public interface ICmd
	{
		void Execute();
	}

	/// <summary>
	/// 射箭
	/// </summary>
	public class Arrow : MonoBehaviour, ICmd
	{

		private Rigidbody2D      mRigidbd2D;
		private SpriteRenderer   mRenderer;
		private CircleCollider2D mCircleCollider2D;

//	SpawnPool bossPool;

		private bool mFading = false;

		void Awake()
		{
			mRigidbd2D = GetComponent<Rigidbody2D>();
			mRenderer = GetComponent<SpriteRenderer>();
			mCircleCollider2D = GetComponent<CircleCollider2D>();
		}

		// Use this for initialization
		void Start()
		{
//		bossPool = PoolManager.Pools ["Boss"];
		}

		void FixedUpdate()
		{
			if (mFading)
				return;
			mRigidbd2D.rotation = Mathf.Rad2Deg * Mathf.Atan2(mRigidbd2D.velocity.y, mRigidbd2D.velocity.x);
		}

		public void Execute()
		{
			mFading = false;
			mRigidbd2D.isKinematic = false;
			mCircleCollider2D.enabled = true;

			var bg = GetComponent<Rigidbody2D>();
			mRenderer.color = Color.white;

			bg.AddForce((Vector2.one - Vector2.up * Random.Range(0.15f, 0.25f)) * 200);
			Invoke("DespawnOutOfScreen", 7.0f);
		}

		void DespawnOutOfScreen()
		{
			if (mFading)
			{

			}
			else
			{
				Despawn();
			}
		}

		void OnDespawned()
		{
			CancelInvoke();
		}

		/// <summary>
		/// 消失
		/// </summary>
		public void FadeOut()
		{
			mFading = true;
			mRigidbd2D.velocity = Vector2.one;
			mRigidbd2D.isKinematic = true;
			mCircleCollider2D.enabled = false;

			Invoke("FadeOutTrans", 0.3f);
			Invoke("Despawn", 0.5f);
		}

		void FadeOutTrans()
		{
//		QTween.FadeOut (transform, 0.2f);
		}

		/// <summary>
		/// 回收
		/// </summary>
		public void Despawn()
		{
//		bossPool.Despawn (transform);
		}
	}
}