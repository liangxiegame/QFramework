using UnityEngine;
using System.Collections;

namespace DigitalRuby.RainMaker
{
    public class RainScript : BaseRainScript
    {
        [Tooltip("The height above the camera that the rain will start falling from")]
        public float RainHeight = 25.0f;

        [Tooltip("How far the rain particle system is ahead of the player")]
        public float RainForwardOffset = -7.0f;

        [Tooltip("The top y value of the mist particles")]
        public float RainMistHeight = 3.0f;

        private void UpdateRain()
        {
            // keep rain and mist above the player
            if (RainFallParticleSystem != null)
            {
                RainFallParticleSystem.transform.position = Camera.transform.position;
                RainFallParticleSystem.transform.Translate(0.0f, RainHeight, RainForwardOffset);
                RainFallParticleSystem.transform.rotation = Quaternion.Euler(0.0f, Camera.transform.rotation.eulerAngles.y, 0.0f);
            }
            if (RainMistParticleSystem != null)
            {
                Vector3 pos = Camera.transform.position;
                pos.y = RainMistHeight;
                RainMistParticleSystem.transform.position = pos;
            }
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            UpdateRain();
        }
    }
}