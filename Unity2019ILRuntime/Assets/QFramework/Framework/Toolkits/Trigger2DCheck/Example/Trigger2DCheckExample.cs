using UnityEngine;

namespace QFramework.Example
{
	public class Trigger2DCheckExample : MonoBehaviour
	{
		private Trigger2DCheck mGroundCheck;

		private void Awake()
		{
			mGroundCheck = transform.Find("GroundCheck").GetComponent<Trigger2DCheck>();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				GetComponent<Rigidbody2D>().velocity = Vector2.up * 5;
			}
		}

		private void OnGUI()
		{
			GUILayout.Label("GroundCheck:" + mGroundCheck.Triggered);
			
			GUILayout.Label("Press Space To Jump");
		}
	}
}