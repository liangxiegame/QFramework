using UnityEditor;
using UnityEngine;

namespace QFramework.PackageKit
{
    public class LabelViewWithRect : View
    {
        public LabelViewWithRect(string content = "", float x = 100, float y = 200, float width = 200,
            float height = 200)
        {
            Content = new Property<string>(content);

            Rect = new Rect(x, y, width, height);
        }

        public Rect Rect { get; set; }

        public Property<string> Content { get; private set; }

        protected override void OnGUI()
        {
            EditorGUI.LabelField(Rect, Content.Value, EditorStyles.boldLabel);
        }
    }
}