using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFramework.Example
{
	public class CustomActionExample : MonoBehaviour
	{
		// Use this for initialization
		void Start()
		{
			this.ExecuteNode(OnlyBeginAction.Allocate(nodeAction =>
			{
				
				this.Delay(1.0f, nodeAction.Finish);
				
				// this.transform.DOLocalMove(new Vector3(5, 5), 0.5f).OnComplete(() =>
				// {
					// nodeAction.Finish();
				// });
			}));

			this.Sequence()
				.Delay(1.0f)
				.OnlyBegin(action =>
				{
					
					this.Delay(1.0f, action.Finish);
					// this.transform.DOLocalMove(new Vector3(-5, -5), 0.5f).OnComplete(() =>
					// {
						// action.Finish();
					// });
				})
				.Begin();
		}
	}
}