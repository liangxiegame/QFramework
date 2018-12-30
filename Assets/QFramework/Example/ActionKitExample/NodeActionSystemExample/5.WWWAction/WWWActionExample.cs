using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace QFramework
{
	public class WWWActionExample : MonoBehaviour
	{
		class WWWGetSikieduAction : NodeAction
		{
			protected override void OnBegin()
			{
				ObservableWWW.Get("http://sikiedu.com")
					.Subscribe(text =>
					{
						text.LogInfo();
						Finish();
					}, e =>
					{
						e.LogException();
					});
			}
		}
		
		// Use this for initialization
		void Start()
		{
//			this.ExecuteNode(new WWWGetSikieduAction());

//			this.Sequence()
//				.Append(new WWWGetSikieduAction())
//				.Begin();

//			this.Repeat()
//				.Append(new WWWGetSikieduAction())
//				.Begin();

			ActionQueue.Append(new WWWGetSikieduAction());
		}
	}
}