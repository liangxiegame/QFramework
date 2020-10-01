using UnityEngine;

namespace QFramework
{
    public class SpaceView : View
    {
        public int Pixel { get; set; }

        public SpaceView(int pixel = 10)
        {
            Pixel = pixel;
        }

        protected override void OnGUI()
        {
            GUILayout.Space(Pixel);
        }
    }
}