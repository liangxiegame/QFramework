using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RuntimeInitializeOnLoadMethod]
public class ParticleScaler : MonoBehaviour {


	[SerializeField]public float mCurScale = 1.0f;
	[SerializeField] float mPreviousScale = 1.0f;

	ParticleSystem mParticleSystem;

	float mSrcStartSize;
	float mSrcStartSpeed;
	float mSrcStartRotation;

	void Awake() {
		mParticleSystem = GetComponent<ParticleSystem> ();

		mSrcStartSize = mParticleSystem.main.startSizeMultiplier;
		mSrcStartSpeed = mParticleSystem.main.startSpeedMultiplier;
		mSrcStartRotation = mParticleSystem.main.startRotationMultiplier;

		ScaleParticleSystem (mCurScale);
	}

	void Update() {
		if (mCurScale != mPreviousScale) {
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
	public void ScaleParticleSystem(float scale)
	{

		ParticleSystem.MainModule mainModule = mParticleSystem.main;


		mainModule.startSizeMultiplier = scale * mSrcStartSize;
		mainModule.startSpeedMultiplier = scale * mSrcStartSpeed;
		mainModule.startRotationMultiplier = scale * mSrcStartRotation;
	}
}

