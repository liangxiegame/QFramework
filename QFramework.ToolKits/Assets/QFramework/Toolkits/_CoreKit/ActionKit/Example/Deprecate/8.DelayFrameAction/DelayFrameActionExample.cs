using UnityEngine;

namespace QFramework
{
	public class DelayFrameActionExample : MonoBehaviour 
	{
		// Use this for initialization
		void Start ()
		{
			Debug.Log(Time.frameCount);
			
			var delayFrameAction = DelayFrameAction.Allocate(1, () =>
			{
				Debug.Log(Time.frameCount);	
			});

			this.ExecuteNode(delayFrameAction);

			this.DelayFrame(2,(() =>
			{
				Debug.Log(Time.frameCount);
			}));

			this.Sequence()
				.Event(() => Debug.Log(Time.frameCount))
				.DelayFrame(2)
				.Event(() => Debug.Log(Time.frameCount))
				.Begin();
			
			this.NextFrame(() =>
			{
				Debug.Log(Time.frameCount);
			});
		}
	}
}