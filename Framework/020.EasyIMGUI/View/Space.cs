using UnityEngine;

namespace QFramework
{
    public interface ISpace : IMGUIView
    {
        ISpace Pixel(int pixel);
    }
    
    internal class Space : View,ISpace
    {
        private int mPixel = 10;
        

        protected override void OnGUI()
        {
            GUILayout.Space(mPixel);
        }

        public ISpace Pixel(int pixel)
        {
            mPixel = pixel;
            return this;
        }
    }
}