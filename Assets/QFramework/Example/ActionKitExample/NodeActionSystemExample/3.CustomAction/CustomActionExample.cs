using DG.Tweening;
using UnityEngine;

namespace QFramework
{
	public class CustomActionExample : MonoBehaviour
	{
		// Use this for initialization
		void Start()
		{
			this.ExecuteNode(OnlyBeginAction.Allocate(nodeAction =>
			{
				this.transform.DOLocalMove(new Vector3(5, 5), 0.5f).OnComplete(() => { nodeAction.Finish(); });
			}));

			this.Sequence()
				.Delay(1.0f)
				.OnlyBegin(action =>
				{
					this.transform.DOLocalMove(new Vector3(-5, -5), 0.5f).OnComplete(() => { action.Finish(); });
				})
				.Begin();
		}
	}
}