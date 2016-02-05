using UnityEngine;
using System.Collections;

public class QTween {

	public static void FadeOut(Transform trans,float duration,bool ignoreTimeScale = false)
	{
		var alphaTween = trans.GetComponent<TweenAlpha> ();
		if (alphaTween == null) {
			alphaTween = trans.gameObject.AddComponent<TweenAlpha> ();
		}
		alphaTween.ResetToBeginning ();
//		alphaTween.enabled = false;
		alphaTween.from = 1.0f;
		alphaTween.to = 0.0f;
		alphaTween.duration = duration;
		alphaTween.ignoreTimeScale = ignoreTimeScale;
		alphaTween.style = UITweener.Style.Once;

		alphaTween.PlayForward ();
	}
		

	public static void FadeIn(Transform trans,float duration,bool ignoreTimeScale = false)
	{
		var alphaTween = trans.GetComponent<TweenAlpha> ();
		if (alphaTween == null) {
			alphaTween = trans.gameObject.AddComponent<TweenAlpha> ();
		}
		alphaTween.ResetToBeginning ();

//		alphaTween.enabled = false;
		alphaTween.from = 0.0f;
		alphaTween.to = 1.0f;
		alphaTween.duration = duration;
		alphaTween.ignoreTimeScale = ignoreTimeScale;
		alphaTween.style = UITweener.Style.Once;

		alphaTween.PlayForward ();
	}

	public static void MoveTo(Transform trans,float duration,Vector3 dstPos,bool ignoreTimeScale = false)
	{
		var posTween = trans.GetComponent<TweenPosition> ();
		if (posTween == null) {
			posTween = trans.gameObject.AddComponent<TweenPosition> ();
		}

//		posTween.enabled = false;
		posTween.from = trans.localPosition;
		posTween.to = dstPos;
		posTween.duration = duration;
		posTween.ignoreTimeScale = ignoreTimeScale;
		posTween.style = UITweener.Style.Once;
		posTween.ResetToBeginning ();

		posTween.PlayForward ();
	}

	public static void MoveBy(Transform trans,float duration,Vector3 dtPos,bool ignoreTimeScale = false)
	{
		var posTween = trans.GetComponent<TweenPosition> ();
		if (posTween == null) {
			posTween = trans.gameObject.AddComponent<TweenPosition> ();
		}

		posTween.from = trans.localPosition;
		posTween.to = trans.localPosition + dtPos;
		posTween.duration = duration;
		posTween.ignoreTimeScale = ignoreTimeScale;
		posTween.style = UITweener.Style.Once;
		posTween.ResetToBeginning ();

		posTween.PlayForward ();
	}

	public static void ScaleTo(Transform trans,float duration,Vector3 dstScale,bool ignoreTimeScale = false)
	{
		var scaleTween = trans.GetComponent<TweenScale> ();
		if (scaleTween == null) {
			scaleTween = trans.gameObject.AddComponent<TweenScale> ();
		}
//		scaleTween.enabled = false;
		scaleTween.from = trans.localScale;
		scaleTween.to = dstScale;
		scaleTween.duration = duration;
		scaleTween.ignoreTimeScale = ignoreTimeScale;
		scaleTween.style = UITweener.Style.Once;
//		scaleTween.ResetToBeginning ();
		scaleTween.PlayForward ();
	}
		

	public static UITweener Blink(Transform trans,float duration,bool ignoreTimeScale = false)
	{
		var alphaTween = trans.GetComponent<TweenAlpha> ();
		if (alphaTween == null) {
			alphaTween = trans.gameObject.AddComponent<TweenAlpha> ();
		}
		alphaTween.from = 0.1f;
		alphaTween.to = 1.0f;
		alphaTween.duration = duration;
		alphaTween.ignoreTimeScale = ignoreTimeScale;
		alphaTween.style = UITweener.Style.PingPong;
		alphaTween.ResetToBeginning ();
		alphaTween.PlayForward ();

		return alphaTween;
	}

	public static void PingPong(UITweener tweener)
	{

	}

	public static void Loop(UITweener tweener)
	{

	}

	public static void Once(UITweener tweener)
	{

	}
}
