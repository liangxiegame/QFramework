using UnityEngine;

namespace EGO.Framework
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