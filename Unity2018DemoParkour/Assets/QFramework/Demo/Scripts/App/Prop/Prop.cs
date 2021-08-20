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

namespace QFramework.PlatformRunner
{
	public class Prop : QMonoBehaviour, ICmd
	{
		public override IManager Manager
		{
			get { return GameManager.Instance; }
		}

//	private TweenScale tweenScale;

		void Awake()
		{
//		tweenScale = GetComponent<TweenScale>();
		}

		/// <summary>
		/// 重置道具状态
		/// </summary>
		public void ResetProp()
		{
			Show();
			Idle();
		}

		/// <summary>
		/// 待机动画
		/// </summary>
		void Idle()
		{
//		tweenScale.from = new Vector3(1.5f, 1.5f, 1.0f);
//		tweenScale.to = new Vector3(2.0f, 2.0f, 1.0f);
//		tweenScale.style = UITweener.Style.PingPong;
//		tweenScale.duration = 0.5f;
//		tweenScale.ignoreTimeScale = false;
//		tweenScale.PlayForward();

//		QTween.FadeIn(transform, 0.0f);
		}

		/// <summary>
		/// 吃完之后的特效
		/// </summary>
		void Effect()
		{
//		QTween.FadeOut(transform, 0.5f);
//		transform.LocalScale(1.5f, 1.5f);
//		QTween.ScaleTo(transform, 0.5f, new Vector3(4.0f, 4.0f, 1.0f));
		}

		/// <summary>
		/// 水果的逻辑执行
		/// </summary>
		public void Execute()
		{
//		SendMsg(new AudioSoundMsg("FRUIT"));
//
//		var particleTrans = PoolManager.Pools["Particle"].Spawn("fruit");
//		var systemCom = particleTrans.GetComponent<ParticleSystem>();
//
//		particleTrans.parent = transform.parent;
//		particleTrans.Identity();
//		particleTrans.localPosition = transform.localPosition;
//
//		systemCom.Play();
//
//		var delayTime = systemCom.duration > 0.5f ? systemCom.duration : 0.5f;
//
//		Effect();
//
//		this.Delay(delayTime, delegate
//		{
//			particleTrans.parent = null;
//			PoolManager.Pools["Particle"].Despawn(particleTrans);
//			Hide();
//		});
		}
	}
}