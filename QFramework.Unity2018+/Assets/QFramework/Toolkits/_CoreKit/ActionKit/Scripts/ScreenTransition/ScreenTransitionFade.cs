using UnityEngine;

namespace QFramework
{
    public class ScreenTransitionFade : AbstractAction<ScreenTransitionFade>
    {
        private Color mColor = UnityEngine.Color.black;
        private float mFromAlpha = 0;
        private float mToAlpha = 0;
        private float mDuration = 1.0f;
        
        
        public ScreenTransitionFade Color(Color color)
        {
            mColor = color;
            return this;
        }
        
        public ScreenTransitionFade FromAlpha(float alpha)
        {
            mFromAlpha = alpha;
            return this;
        }
        
        public ScreenTransitionFade ToAlpha(float alpha)
        {
            mToAlpha = alpha;
            return this;
        }

        public ScreenTransitionFade Duration(float duration)
        {
            mDuration = duration;
            return this;
        }
        
        public override void OnStart()
        {
            ScreenTransitionCanvas.Instance.ColorImage.color = mColor;
            ScreenTransitionCanvas.Instance.ColorImage.ColorAlpha(mFromAlpha);
            mCurrentSeconds = 0;
        }

        private float mCurrentSeconds = 0;
        public override void OnExecute(float dt)
        {
            mCurrentSeconds += dt;

            var alpha = Mathf.Lerp(mFromAlpha, mToAlpha, mCurrentSeconds / mDuration);
            ScreenTransitionCanvas.Instance.ColorImage
                .ColorAlpha(alpha); 
            
            if (mCurrentSeconds >= mDuration)
            {
                this.Finish();
            }
        }

        public override void OnFinish()
        {
            ScreenTransitionCanvas.Instance.ColorImage.ColorAlpha(mToAlpha); 
        }
    }
}