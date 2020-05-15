using UnityEngine;

namespace QFramework
{
	public abstract class GameplayComponent : MonoBehaviour
	{
		public abstract int UpdateAI(float gameTime, float deltaTime);
		public abstract int UpdateReqeust(float gameTime, float deltaTime);
		public abstract int UpdateBehavior(float gameTime, float deltaTime);
	}
}