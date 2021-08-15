using UnityEngine;

namespace QFramework.Example.ActionKit
{
	public class SpawnNodeExample : MonoBehaviour
	{
		void Start()
		{
			var spawnNode = new SpawnNode();
			

			spawnNode.Add(DelayAction.Allocate(1.0f, () => Debug.Log(Time.time)));
			spawnNode.Add(DelayAction.Allocate(1.0f, () => Debug.Log(Time.time)));
			spawnNode.Add(DelayAction.Allocate(1.0f, () => Debug.Log(Time.time)));
			spawnNode.Add(DelayAction.Allocate(1.0f, () => Debug.Log(Time.time)));
			spawnNode.Add(DelayAction.Allocate(1.0f, () => Debug.Log(Time.time)));

			this.ExecuteNode(spawnNode);
		}
	}
}