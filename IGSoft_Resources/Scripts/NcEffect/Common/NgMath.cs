
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NgMath
{
	// ease properfy -----------------------------------------------------
	public delegate float EasingFunction(float start, float end, float Value);
	public enum EaseType
	{
		None,
		linear,
		spring,
		punch,
		easeInQuad,
		easeInCubic,
		easeInQuart,
		easeInQuint,
		easeInSine,
		easeInExpo,
		easeInCirc,
		easeInBack,
		easeInElastic,
		easeInBounce,
		easeOutQuad,
		easeOutCubic,
		easeOutQuart,
		easeOutQuint,
		easeOutSine,
		easeOutExpo,
		easeOutCirc,
		easeOutBack,
		easeOutElastic,
		easeOutBounce,
		easeInOutQuad,
		easeInOutCubic,
		easeInOutQuart,
		easeInOutQuint,
		easeInOutSine,
		easeInOutExpo,
		easeInOutCirc,
		easeInOutBounce,
		easeInOutBack,
		easeInOutElastic
		/* GFX47 MOD START */
		//bounce,
		/* GFX47 MOD END */
		//Back
		/* GFX47 MOD START */
		//elastic,
		/* GFX47 MOD END */
	}

	// ease function -----------------------------------------------------
	public static EasingFunction GetEasingFunction(EaseType easeType)
	{
		switch (easeType)
		{
			case EaseType.easeInQuad:		return new EasingFunction(easeInQuad);
			case EaseType.easeOutQuad:		return new EasingFunction(easeOutQuad);
			case EaseType.easeInOutQuad:	return new EasingFunction(easeInOutQuad);
			case EaseType.easeInCubic:		return new EasingFunction(easeInCubic);
			case EaseType.easeOutCubic:		return new EasingFunction(easeOutCubic);
			case EaseType.easeInOutCubic:	return new EasingFunction(easeInOutCubic);
			case EaseType.easeInQuart:		return new EasingFunction(easeInQuart);
			case EaseType.easeOutQuart:		return new EasingFunction(easeOutQuart);
			case EaseType.easeInOutQuart:	return new EasingFunction(easeInOutQuart);
			case EaseType.easeInQuint:		return new EasingFunction(easeInQuint);
			case EaseType.easeOutQuint:		return new EasingFunction(easeOutQuint);
			case EaseType.easeInOutQuint:	return new EasingFunction(easeInOutQuint);
			case EaseType.easeInSine:		return new EasingFunction(easeInSine);
			case EaseType.easeOutSine:		return new EasingFunction(easeOutSine);
			case EaseType.easeInOutSine:	return new EasingFunction(easeInOutSine);
			case EaseType.easeInExpo:		return new EasingFunction(easeInExpo);
			case EaseType.easeOutExpo:		return new EasingFunction(easeOutExpo);
			case EaseType.easeInOutExpo:	return new EasingFunction(easeInOutExpo);
			case EaseType.easeInCirc:		return new EasingFunction(easeInCirc);
			case EaseType.easeOutCirc:		return new EasingFunction(easeOutCirc);
			case EaseType.easeInOutCirc:	return new EasingFunction(easeInOutCirc);
			case EaseType.linear:			return new EasingFunction(linear);
			case EaseType.spring:			return new EasingFunction(spring);
			/* GFX47 MOD START */
			/*case EaseType.bounce:
				return new EasingFunction(bounce);
				break;*/
			case EaseType.easeInBounce:		return new EasingFunction(easeInBounce);
			case EaseType.easeOutBounce:	return new EasingFunction(easeOutBounce);
			case EaseType.easeInOutBounce:	return new EasingFunction(easeInOutBounce);
			/* GFX47 MOD END */
			case EaseType.easeInBack:		return new EasingFunction(easeInBack);
			case EaseType.easeOutBack:		return new EasingFunction(easeOutBack);
			case EaseType.easeInOutBack:	return new EasingFunction(easeInOutBack);
			/* GFX47 MOD START */
			/*case EaseType.elastic:
				return new EasingFunction(elastic);
				break;*/
			case EaseType.easeInElastic:	return new EasingFunction(easeInElastic);
			case EaseType.easeOutElastic:	return new EasingFunction(easeOutElastic);
			case EaseType.easeInOutElastic:	return new EasingFunction(easeInOutElastic);
			/* GFX47 MOD END */
		}
		return null;
	}

	// ease simple function -----------------------------------------------------
	public static float linear(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, value);
	}
	
	public static float clerp(float start, float end, float value)
	{
		float min = 0.0f;
		float max = 360.0f;
		float half = Mathf.Abs((max - min) * 0.5f);
		float retval = 0.0f;
		float diff = 0.0f;
		if ((end - start) < -half){
			diff = ((max - start) + end) * value;
			retval = start + diff;
		}else if ((end - start) > half){
			diff = -((max - end) + start) * value;
			retval = start + diff;
		}else retval = start + (end - start) * value;
		return retval;
    }

	public static float spring(float start, float end, float value)
	{
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
		return start + (end - start) * value;
	}

	public static float easeInQuad(float start, float end, float value)
	{
		end -= start;
		return end * value * value + start;
	}

	public static float easeOutQuad(float start, float end, float value)
	{
		end -= start;
		return -end * value * (value - 2) + start;
	}

	public static float easeInOutQuad(float start, float end, float value)
	{
		value /= .5f;
		end -= start;
		if (value < 1) return end * 0.5f * value * value + start;
		value--;
		return -end * 0.5f * (value * (value - 2) - 1) + start;
	}

	public static float easeInCubic(float start, float end, float value)
	{
		end -= start;
		return end * value * value * value + start;
	}

	public static float easeOutCubic(float start, float end, float value)
	{
		value--;
		end -= start;
		return end * (value * value * value + 1) + start;
	}

	public static float easeInOutCubic(float start, float end, float value)
	{
		value /= .5f;
		end -= start;
		if (value < 1) return end * 0.5f * value * value * value + start;
		value -= 2;
		return end * 0.5f * (value * value * value + 2) + start;
	}

	public static float easeInQuart(float start, float end, float value)
	{
		end -= start;
		return end * value * value * value * value + start;
	}

	public static float easeOutQuart(float start, float end, float value)
	{
		value--;
		end -= start;
		return -end * (value * value * value * value - 1) + start;
	}

	public static float easeInOutQuart(float start, float end, float value)
	{
		value /= .5f;
		end -= start;
		if (value < 1) return end * 0.5f * value * value * value * value + start;
		value -= 2;
		return -end * 0.5f * (value * value * value * value - 2) + start;
	}

	public static float easeInQuint(float start, float end, float value)
	{
		end -= start;
		return end * value * value * value * value * value + start;
	}

	public static float easeOutQuint(float start, float end, float value)
	{
		value--;
		end -= start;
		return end * (value * value * value * value * value + 1) + start;
	}

	public static float easeInOutQuint(float start, float end, float value)
	{
		value /= .5f;
		end -= start;
		if (value < 1) return end * 0.5f * value * value * value * value * value + start;
		value -= 2;
		return end * 0.5f * (value * value * value * value * value + 2) + start;
	}

	public static float easeInSine(float start, float end, float value)
	{
		end -= start;
		return -end * Mathf.Cos(value * (Mathf.PI * 0.5f)) + end + start;
	}

	public static float easeOutSine(float start, float end, float value){
		end -= start;
		return end * Mathf.Sin(value * (Mathf.PI * 0.5f)) + start;
	}

	public static float easeInOutSine(float start, float end, float value)
	{
		end -= start;
		return -end * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1) + start;
	}

	public static float easeInExpo(float start, float end, float value)
	{
		end -= start;
		return end * Mathf.Pow(2, 10 * (value - 1)) + start;
	}

	public static float easeOutExpo(float start, float end, float value)
	{
		end -= start;
		return end * (-Mathf.Pow(2, -10 * value ) + 1) + start;
	}

	public static float easeInOutExpo(float start, float end, float value)
	{
		value /= .5f;
		end -= start;
		if (value < 1) return end * 0.5f * Mathf.Pow(2, 10 * (value - 1)) + start;
		value--;
		return end * 0.5f * (-Mathf.Pow(2, -10 * value) + 2) + start;
	}

	public static float easeInCirc(float start, float end, float value)
	{
		end -= start;
		return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
	}

	public static float easeOutCirc(float start, float end, float value)
	{
		value--;
		end -= start;
		return end * Mathf.Sqrt(1 - value * value) + start;
	}

	public static float easeInOutCirc(float start, float end, float value)
	{
		value /= .5f;
		end -= start;
		if (value < 1) return -end * 0.5f * (Mathf.Sqrt(1 - value * value) - 1) + start;
		value -= 2;
		return end * 0.5f * (Mathf.Sqrt(1 - value * value) + 1) + start;
	}

	/* GFX47 MOD START */
	public static float easeInBounce(float start, float end, float value)
	{
		end -= start;
		float d = 1f;
		return end - easeOutBounce(0, end, d-value) + start;
	}
	/* GFX47 MOD END */

	/* GFX47 MOD START */
	//private float bounce(float start, float end, float value){
	public static float easeOutBounce(float start, float end, float value)
	{
		value /= 1f;
		end -= start;
		if (value < (1 / 2.75f)){
			return end * (7.5625f * value * value) + start;
		}else if (value < (2 / 2.75f)){
			value -= (1.5f / 2.75f);
			return end * (7.5625f * (value) * value + .75f) + start;
		}else if (value < (2.5 / 2.75)){
			value -= (2.25f / 2.75f);
			return end * (7.5625f * (value) * value + .9375f) + start;
		}else{
			value -= (2.625f / 2.75f);
			return end * (7.5625f * (value) * value + .984375f) + start;
		}
	}
	/* GFX47 MOD END */

	/* GFX47 MOD START */
	public static float easeInOutBounce(float start, float end, float value)
	{
		end -= start;
		float d = 1f;
		if (value < d* 0.5f) return easeInBounce(0, end, value*2) * 0.5f + start;
		else return easeOutBounce(0, end, value*2-d) * 0.5f + end*0.5f + start;
	}
	/* GFX47 MOD END */

	public static float easeInBack(float start, float end, float value)
	{
		end -= start;
		value /= 1;
		float s = 1.70158f;
		return end * (value) * value * ((s + 1) * value - s) + start;
	}

	public static float easeOutBack(float start, float end, float value){
		float s = 1.70158f;
		end -= start;
		value = (value) - 1;
		return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
	}

	public static float easeInOutBack(float start, float end, float value)
	{
		float s = 1.70158f;
		end -= start;
		value /= .5f;
		if ((value) < 1){
			s *= (1.525f);
			return end * 0.5f * (value * value * (((s) + 1) * value - s)) + start;
		}
		value -= 2;
		s *= (1.525f);
		return end * 0.5f * ((value) * value * (((s) + 1) * value + s) + 2) + start;
	}

	public static float punch(float amplitude, float value)
	{
		float s = 9;
		if (value == 0){
			return 0;
		}
		else if (value == 1){
			return 0;
		}
		float period = 1 * 0.3f;
		s = period / (2 * Mathf.PI) * Mathf.Asin(0);
		return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
    }
	
	/* GFX47 MOD START */
	public static float easeInElastic(float start, float end, float value)
	{
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if (value == 0) return start;
		
		if ((value /= d) == 1) return start + end;
		
		if (a == 0f || a < Mathf.Abs(end)){
			a = end;
			s = p / 4;
			}else{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		return -(a * Mathf.Pow(2, 10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
	}		
	/* GFX47 MOD END */

	/* GFX47 MOD START */
	//private float elastic(float start, float end, float value){
	public static float easeOutElastic(float start, float end, float value)
	{
	/* GFX47 MOD END */
		//Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if (value == 0) return start;
		
		if ((value /= d) == 1) return start + end;
		
		if (a == 0f || a < Mathf.Abs(end)){
			a = end;
			s = p * 0.25f;
			}else{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
	}		
	
	/* GFX47 MOD START */
	public static float easeInOutElastic(float start, float end, float value)
	{
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if (value == 0) return start;
		
		if ((value /= d*0.5f) == 2) return start + end;
		
		if (a == 0f || a < Mathf.Abs(end)){
			a = end;
			s = p / 4;
			}else{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
		return a * Mathf.Pow(2, -10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
	}		
	/* GFX47 MOD END */
}