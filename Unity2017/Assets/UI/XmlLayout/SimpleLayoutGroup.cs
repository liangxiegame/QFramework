using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Xml
{
    /// <summary>
    /// This is a layout group element added to some prefabs (e.g. Panel) to allow control over simple things like padding
    /// </summary>
    [RequireComponent(typeof(RectTransform))]    
    public class SimpleLayoutGroup : HorizontalLayoutGroup
    {                        
        protected override void Awake()
        {
            base.Awake();            
        }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();            
        }

        public override void CalculateLayoutInputVertical()
        {
            base.CalculateLayoutInputVertical();            
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();            
        }

        public override void SetLayoutHorizontal()
        {
            base.SetLayoutHorizontal();            
        }

        public override void SetLayoutVertical()
        {
           base.SetLayoutVertical();                    
        }
    }    
}
