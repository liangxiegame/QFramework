using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
	public class AnimatorExample : MonoBehaviour
	{
		public Text StateText;

		public Animator Animator;
		
		private bool mIsRed;
		private bool mIsBlack;
		
		private List<string> mAnimatorParameters;
		
		void Start()
		{
			mAnimatorParameters = new List<string>();
			
			Animator.AddAnimatorParameterIfExists("IsRed",AnimatorControllerParameterType.Bool,mAnimatorParameters);
			Animator.AddAnimatorParameterIfExists("IsBlack",AnimatorControllerParameterType.Bool,mAnimatorParameters);
		}

		void Update()
		{
			Animator.UpdateAnimatorBool("IsRed",mIsRed,mAnimatorParameters);
			Animator.UpdateAnimatorBool("IsBlack", mIsBlack,mAnimatorParameters);

			if (Input.GetMouseButtonDown(0))
			{
				mIsRed = false;
				mIsBlack = true;

				StateText.text = "按下";
			}

			if (Input.GetMouseButtonUp(0))
			{
				mIsBlack = false;
				mIsRed = true;

				StateText.text = "抬起";
			}
		}
	}
}
