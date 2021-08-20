/****************************************************************************
 * Copyright (c) 2018.3 liangxie
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
	public enum EnemyState
	{
		Kick,
		Idle,
		Shake,
		Hited,
	}

	public class Enemy : QMonoBehaviour ,IController
	{
		public override IManager Manager
		{
			get { return UIManager.Instance; }
		}

		public EnemyState enemyState = EnemyState.Idle;

		public Rigidbody2D      rigidBd;
		public CircleCollider2D circle2D;
		public BoxCollider2D    box2D;
		public Animator         animator;
		private IPlayerModel mPlayerModel;
		

		void Awake()
		{
			rigidBd = GetComponent<Rigidbody2D>();
			circle2D = GetComponent<CircleCollider2D>();
			box2D = GetComponent<BoxCollider2D>();
			animator = GetComponent<Animator>();

			mPlayerModel = this.GetModel<IPlayerModel>();
		}

		private void Start()
		{
			InvokeRepeating("PlayAnimation", 0.0f, Random.Range(1.0f, 3.0f));
		}

		public void PlayAnimation()
		{
			// 待机状态下 播放动画
			if (enemyState == EnemyState.Idle)
			{
				animator.SetTrigger("play");
			}
		}

		/// <summary>
		/// 重置状态
		/// </summary>
		public void ResetEnemy()
		{
			enemyState = EnemyState.Idle;
			circle2D.enabled = true;
			box2D.enabled = true;
			rigidBd.isKinematic = false;
			rigidBd.angularVelocity = 0.0f;
			rigidBd.rotation = 0.0f;
			rigidBd.velocity = Vector2.zero;
//			QTween.RotateTo(transform, 0.0f, Vector3.zero);
		}

		/// <summary>
		/// 震动掉
		/// </summary>
		public void Shake()
		{
			if (enemyState == EnemyState.Kick || enemyState == EnemyState.Hited || enemyState == EnemyState.Shake)
				return;
			enemyState = EnemyState.Shake;
			rigidBd.isKinematic = false;
			circle2D.enabled = false;
			rigidBd.AddForce(Vector2.up * mPlayerModel.CurJumpPower.Value * 0.5f);
			box2D.enabled = false;
		}

		/// <summary>
		/// 踩掉
		/// </summary>
		public void StepOn()
		{
			if (enemyState == EnemyState.Kick || enemyState == EnemyState.Hited || enemyState == EnemyState.Shake)
				return;
			enemyState = EnemyState.Shake;


			rigidBd.isKinematic = false;
//		rigidBd.angularVelocity = Random.Range (0, 2) == 1 ? 100.0f : -100.0f;
//		QTween.MoveTo
			circle2D.enabled = false;
			rigidBd.AddForce(Vector2.up * mPlayerModel.CurJumpPower.Value * 0.3f);
			box2D.enabled = false;
//			QTween.RotateBy(transform, 1.0f, Vector3.forward * (Random.Range(0, 2) == 1 ? 100.0f : -100.0f));

			// 音效
//		QSoundMgr.Instance.PlayClipAsync(Random.Range(SOUND.ENEMY_DEATH1, SOUND.ENEMY_DEATH2));
		}

		/// <summary>
		/// 踢飞
		/// </summary>
		public void Kick(Vector3 direction)
		{
			if (enemyState == EnemyState.Hited || enemyState == EnemyState.Kick)
				return;

			enemyState = EnemyState.Kick;

			circle2D.enabled = false;
			rigidBd.isKinematic = true;
			box2D.enabled = false;
			transform.parent = null;
			rigidBd.velocity = new Vector2(10.0f + this.GetModel<IPlayerModel>().CurSpeed.Value,
				Random.Range(0, 10 +  this.GetModel<IPlayerModel>().CurSpeed.Value));

			// 音效
//		QSoundMgr.Instance.PlayClipAsync(Random.Range(SOUND.ENEMY_DEATH1, SOUND.ENEMY_DEATH2));
		}

		/// <summary>
		/// 达到攻击的目的
		/// </summary>
		public void Hit()
		{
			if (enemyState == EnemyState.Shake || enemyState == EnemyState.Kick || enemyState == EnemyState.Hited)
			{
				return;
			}

			enemyState = EnemyState.Hited;
			circle2D.enabled = false;
			box2D.enabled = false;
			rigidBd.isKinematic = true;

//		QSoundMgr.Instance.PlayClipAsync(SOUND.HERO_HURT);

			animator.SetTrigger("play"); // 显示攻击动作
		}

		public IArchitecture GetArchitecture()
		{
			return PlatformRunner.Interface;
		}
	}
}