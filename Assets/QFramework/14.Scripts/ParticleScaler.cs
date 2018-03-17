/****************************************************************************
 * Copyright (c) 2017 liangxie
 * Copyright (c) 2018 liangxie
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

namespace QFramework 
{
	using UnityEngine;

	//[RuntimeInitializeOnLoadMethod]
	public class ParticleScaler : MonoBehaviour 
	{
		[SerializeField] public float mCurScale = 1.0f;
		[SerializeField] float mPreviousScale = 1.0f;

		ParticleSystem mParticleSystem;

		private float mSrcStartSize;
		private float mSrcStartSpeed;
		private float mSrcStartRotation;

		private void Awake () 
		{
			mParticleSystem = GetComponent<ParticleSystem> ();

			mSrcStartSize = mParticleSystem.main.startSizeMultiplier;
			mSrcStartSpeed = mParticleSystem.main.startSpeedMultiplier;
			mSrcStartRotation = mParticleSystem.main.startRotationMultiplier;

			ScaleParticleSystem (mCurScale);
		}

		private void Update () 
		{
			if (mCurScale != mPreviousScale) 
			{
				ScaleParticleSystem (mCurScale);
				mPreviousScale = mCurScale;
			}
		}

		//作者：赵青青
		//链接：https://www.zhihu.com/question/33332381/answer/64318749
		//来源：知乎
		//著作权归作者所有。商业转载请联系作者获得授权，非商业转载请注明出处。

		/// <summary>
		/// 缩放粒子
		/// </summary>
		/// <param name="gameObj">粒子节点</param>
		/// <param name="scale">绽放系数</param>
		public void ScaleParticleSystem (float scale) 
		{
			var mainModule = mParticleSystem.main;

			mainModule.startSizeMultiplier = scale * mSrcStartSize;
			mainModule.startSpeedMultiplier = scale * mSrcStartSpeed;
			mainModule.startRotationMultiplier = scale * mSrcStartRotation;
		}
	}
}