// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;

#if UNITY_EDITOR
//using UnityEditor;
#endif

public class NgAnimation
{
	public static AnimationClip SetAnimation(Animation tarAnimation, int tarIndex, AnimationClip srcClip)
	{
		int nCount = 0;
		AnimationClip[]	backClip = new AnimationClip[tarAnimation.GetClipCount() - tarIndex + 1];

		foreach (AnimationState clip in tarAnimation)
		{
			if (tarIndex == nCount)
				tarAnimation.RemoveClip(clip.clip);
			if (tarIndex < nCount)
			{
				backClip[nCount - tarIndex - 1] = clip.clip;
				tarAnimation.RemoveClip(clip.clip);
			}
		}

		tarAnimation.AddClip(srcClip, srcClip.name);
		for (int n = 0; n < backClip.Length; n++)
			tarAnimation.AddClip(backClip[n], backClip[n].name);
		return srcClip;
	}

	public static AnimationState GetAnimationByIndex(Animation anim, int nIndex)
	{
		int i = 0;
		foreach (AnimationState clip in anim)
		{
			if (i == nIndex)
				return clip;
			i++;
		}
		return null;
	}
}

