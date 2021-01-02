using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Xml
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    class SimpleContentSizeFitter : LayoutGroup
    {        
        private DrivenRectTransformTracker m_RectTransformTracker;

        public enum eAxis
        {
            Horizontal,
            Vertical
        };

        public eAxis Axis = eAxis.Vertical;
        
        public override void CalculateLayoutInputVertical()
        {
            var childCount = rectChildren.Count;

            var width = rectTransform.sizeDelta.x;
            var height = rectTransform.sizeDelta.y;
                        
            if (childCount == 1)
            {
                var child = rectChildren[0];
                width = child.sizeDelta.x;
                height = child.sizeDelta.y;
            }
            else
            {
                if (childCount > 1)
                {
                    Debug.LogWarning("SimpleContentSizeFitter:: This layout element will only function correctly if this element has a single child.");
                }
            }

            if (Axis == eAxis.Vertical)
            {
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
            }
            else
            {
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 1);
            }

            rectTransform.sizeDelta = new Vector2(width, height);            
        }

        public override void SetLayoutHorizontal() {}
        
        public override void SetLayoutVertical() {}

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();

            XmlLayoutTimer.DelayedCall(0.05f, SetDirty, this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_RectTransformTracker.Clear();
        }        
    }
}
