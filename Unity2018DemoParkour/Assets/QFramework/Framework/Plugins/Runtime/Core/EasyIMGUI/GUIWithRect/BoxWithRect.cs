using System.Xml;
using UnityEngine;

namespace QFramework
{
    public interface IBoxWithRect : IMGUIView, IHasRect<IBoxWithRect>
    {
        IBoxWithRect Text(string text);
    }

    public class BoxWithRect : View, IBoxWithRect
    {
        public BoxWithRect()
        {
            mStyleProperty = new GUIStyleProperty(() => GUI.skin.box);
        }

        private Rect mRect = new Rect(0, 0, 200, 100);

        private string mText = string.Empty;
        
        protected override void OnGUI()
        {
            GUI.Box(mRect, mText, mStyleProperty.Value);
        }

        public IBoxWithRect Text(string labelText)
        {
            mText = labelText;

            return this;
        }

        public T Convert<T>(XmlNode node) where T : class
        {
            throw new System.NotImplementedException();
        }

        public IBoxWithRect Rect(Rect rect)
        {
            mRect = rect;
            return this;
        }

        public IBoxWithRect Position(Vector2 position)
        {
            mRect.position = position;
            return this;
        }

        public IBoxWithRect Position(float x, float y)
        {
            mRect.x = x;
            mRect.y = y;
            return this;
        }

        public IBoxWithRect Size(float width, float height)
        {
            mRect.width = width;
            mRect.height = height;
            return this;
        }

        public IBoxWithRect Size(Vector2 size)
        {
            mRect.size = size;
            return this;
        }

        public IBoxWithRect Width(float width)
        {
            mRect.width = width;
            return this;
        }

        public IBoxWithRect Height(float height)
        {
            mRect.height = height;
            return this;
        }
    }
}