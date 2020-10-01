using QFramework;
using UnityEngine;

namespace QFramework
{
    public class GameTimer
    {
        public float gameTime
        {
            private set;
            get;
        }
        public float deltaTime
        {
            private set;
            get;
        }
        public float timeScale
        {
            get
            {
                return Time.timeScale;
            }
        }
        public void Init()
        {
            gameTime        = 0f;
            deltaTime       = 0f;
        }
        public void UpdateTime()
        {
            deltaTime = Time.deltaTime * Time.timeScale;
            gameTime += deltaTime;
        }
    }
}
