using UnityEngine;
using System.Collections;

namespace QFramework {
	/// <summary>
	/// 函数延时,计时器等方法,但是受到timeScale影响
	/// </summary>
	public class QTimer {

		public delegate void QTimerCallfunc ();
		public delegate void QTimerCallfuncInt(int i);

		/// <summary>
		/// 延时调用函数
		/// </summary>
		/// <param name="seconds">Seconds.</param>
		/// <param name="callfunc">Callfunc.</param>
		public static void ExecuteAfterSeconds(float seconds,QTimerCallfunc callfunc)
		{
			App.Instance ().StartCoroutine (Execute (seconds, callfunc));
		}

		static IEnumerator Execute(float seconds,QTimerCallfunc callfunc)
		{

			yield return new WaitForSeconds (seconds);
			callfunc ();
		}




		//Sets the active state of the go to state, after time
		public static IEnumerator ChangeEnabledState(GameObject go, bool state, float time)
		{
			float i = 0.0f;
			float rate = 1.0f / time;

			while (i < 1.0)
			{
				i += Time.deltaTime * rate;
				yield return 0;
			}

			go.SetActive(state);
		}
		//Calls the passed void function with no arguments after delay
		public static IEnumerator CallWithDelay(QTimerCallfunc del, float delay)
		{
			yield return new WaitForSeconds(delay);
			del();
		}
		//Calls the passed void function with no arguments after delay
		public static IEnumerator CallWithDelay(QTimerCallfuncInt del, int num, float delay)
		{
			yield return new WaitForSeconds(delay);
			del(num);
		}


	}






}
