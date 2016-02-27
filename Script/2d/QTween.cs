using UnityEngine;
using System.Collections;

namespace QFramework {
	public class QTween {

		public static UITweener FadeOut(Transform trans,float duration,bool ignoreTimeScale = false)
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

			return alphaTween;
		}


		public static UITweener FadeIn(Transform trans,float duration,bool ignoreTimeScale = false)
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

			return alphaTween;
		}

		public static UITweener MoveTo(Transform trans,float duration,Vector3 dstPos,bool ignoreTimeScale = false)
		{
			var posTween = trans.GetComponent<TweenPosition> ();
			if (posTween == null) {
				posTween = trans.gameObject.AddComponent<TweenPosition> ();
			}

			posTween.enabled = false;
			posTween.from = trans.localPosition;
			posTween.to = dstPos;
			posTween.duration = duration;
			posTween.ignoreTimeScale = ignoreTimeScale;
			posTween.style = UITweener.Style.Once;
			posTween.ResetToBeginning ();

			posTween.PlayForward ();

			return posTween;
		}

		public static UITweener MoveTo(Transform trans,float  duration,Vector3 srcPos,Vector3 dstPos,bool ignoreTimeScale = false)
		{
			var posTween = trans.GetComponent<TweenPosition> ();
			if (posTween == null) {
				posTween = trans.gameObject.AddComponent<TweenPosition> ();
			}

			posTween.from = srcPos;
			posTween.to = dstPos;
			posTween.duration = duration;
			posTween.ignoreTimeScale = ignoreTimeScale;
			posTween.ResetToBeginning ();

			posTween.PlayForward ();
			return posTween;
		}

		public static UITweener MoveBy(Transform trans,float duration,Vector3 dtPos,bool ignoreTimeScale = false)
		{
			var posTween = trans.GetComponent<TweenPosition> ();
			if (posTween == null) {
				posTween = trans.gameObject.AddComponent<TweenPosition> ();
			}
			posTween.enabled = false;

			posTween.from = trans.localPosition;
			posTween.to = trans.localPosition + dtPos;
			posTween.duration = duration;
			posTween.ignoreTimeScale = ignoreTimeScale;
			posTween.style = UITweener.Style.Once;

			posTween.ResetToBeginning ();
			posTween.PlayForward ();

			return posTween;
		}

		public static UITweener ScaleTo(Transform trans,float duration,Vector3 dstScale,bool ignoreTimeScale = false)
		{
			var scaleTween = trans.GetComponent<TweenScale> ();
			if (scaleTween == null) {
				scaleTween = trans.gameObject.AddComponent<TweenScale> ();
			}

			scaleTween.enabled = false;
			scaleTween.from = trans.localScale;
			scaleTween.to = dstScale;
			scaleTween.duration = duration;
			scaleTween.ignoreTimeScale = ignoreTimeScale;
			scaleTween.style = UITweener.Style.Once;
			scaleTween.ResetToBeginning ();
			scaleTween.PlayForward ();

			return scaleTween;
		}

		/// <summary>
		/// 旋转了
		/// </summary>
		public static UITweener RotateBy(Transform trans,float duration,Vector3 dtRotation,bool ignoreTimeScale = false)
		{
			var rotateTween = trans.GetComponent<TweenRotation> ();
			if (rotateTween == null) {
				rotateTween = trans.gameObject.AddComponent<TweenRotation> ();
			}
				

			rotateTween.enabled = false;
			rotateTween.from =   trans.localRotation.eulerAngles;
			rotateTween.to = trans.localRotation.eulerAngles + dtRotation;
			rotateTween.duration = duration;
			rotateTween.ignoreTimeScale = ignoreTimeScale;
			rotateTween.style = UITweener.Style.Once;
			rotateTween.PlayForward ();

			return rotateTween;
		}


		/// <summary>
		/// 旋转到
		/// </summary>
		public static UITweener RotateTo(Transform trans,float duration,Vector3 dstRotation,bool ignoreTimeScale = false)
		{
			var rotateTween = trans.GetComponent<TweenRotation> ();
			if (rotateTween == null) {
				rotateTween = trans.gameObject.AddComponent<TweenRotation> ();
			}

			rotateTween.enabled = false;
			rotateTween.from = trans.localRotation.eulerAngles;
			rotateTween.to = dstRotation;
			rotateTween.duration = duration;
			rotateTween.ignoreTimeScale = ignoreTimeScale;
			rotateTween.style = UITweener.Style.Once;
			rotateTween.PlayForward ();

			return rotateTween;
		}

		/// <summary>
		/// 变化颜色到
		/// </summary>
		public static UITweener TintTo(Transform trans,float duration,Color srcColor,Color dstColor,bool ignoreTimeScale = false)
		{
			var colorTween = trans.GetComponent<TweenColor> ();
			if (colorTween == null) {
				colorTween = trans.gameObject.AddComponent<TweenColor> ();
			}

			colorTween.enabled = false;
			colorTween.from = srcColor;
			colorTween.to = dstColor;
			colorTween.duration = duration;
			colorTween.ignoreTimeScale = ignoreTimeScale;
			colorTween.style = UITweener.Style.Once;
			colorTween.ResetToBeginning ();
			colorTween.PlayForward ();

			return colorTween;
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
			tweener.style = UITweener.Style.PingPong;
		}

		public static void Loop(UITweener tweener)
		{
			tweener.style = UITweener.Style.Loop;
		}

		public static UITweener Once(UITweener tweener)
		{
			tweener.style = UITweener.Style.Once;
			return tweener;
		}


		// 从其他插件Down下来的方法
		//Moves a menu element by the received ammount in time
		public static IEnumerator MoveElementBy(Transform element, Vector2 ammount, float time)
		{
			float i = 0.0f;
			float rate = 1.0f / time;

			Vector2 startPos = element.position;
			Vector2 endPos = element.position;
			endPos += ammount;

			while (i < 1.0)
			{
				i += Time.deltaTime * rate;
				element.localPosition = Vector3.Lerp(startPos, endPos, i);

				yield return 0;
			}
		}

		//Rescales the given element to the given scale in time
		public static IEnumerator ScaleTo(Transform element, Vector2 endScale, float time)
		{
			float i = 0.0f;
			float rate = 1.0f / time;

			Vector2 startScale = element.localScale;

			while (i < 1.0)
			{
				i += Time.deltaTime * rate;
				element.localScale = Vector3.Lerp(startScale, endScale, i);

				yield return 0;
			}
		}


	}

}
