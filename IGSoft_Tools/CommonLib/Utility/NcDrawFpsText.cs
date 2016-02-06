// ----------------------------------------------------------------------------------
//
//
// ----------------------------------------------------------------------------------

using UnityEngine;

public class NcDrawFpsText : MonoBehaviour 
{
	// -------------------------------------------------------------------------------------------
	public  float updateInterval = 0.5F;
	 
	private float accum   = 0;
	private int   frames  = 0;
	private float timeleft;
	 
	// -------------------------------------------------------------------------------------------
	void Start()
	{
		if (!GetComponent<GUIText>())
		{
			Debug.Log("UtilityFramesPerSecond needs a GUIText component!");
			enabled = false;
			return;
		}
		timeleft = updateInterval;  
	}

	void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		++frames;
	    
		// Interval ended - update GUI text and start new interval
		if (timeleft <= 0.0)
		{
			// display two fractional digits (f2 format)
			float fps = accum/frames;
			string format = System.String.Format("{0:F2} FPS",fps);
			GetComponent<GUIText>().text = format;

			if (fps < 30)
				GetComponent<GUIText>().material.color = Color.yellow;
			else {
				if (fps < 10)
					 GetComponent<GUIText>().material.color = Color.red;
				else GetComponent<GUIText>().material.color = Color.green;
			}
			timeleft = updateInterval;
			accum = 0.0F;
			frames = 0;
		}
	}
}
