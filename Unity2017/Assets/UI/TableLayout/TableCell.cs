using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tables
{    
    [RequireComponent(typeof(RectTransform))]    
    public class TableCell : HorizontalLayoutGroup
    {        
        [Tooltip("How many columns should this cell span?")]
        public int columnSpan = 1;

        [Tooltip("If this property is set, then this cell will ignore the TableLayout CellBackgroundColor/CellBackgroundImage values - allowing you to set specific values for this cell.")]
        public bool dontUseTableCellBackground = false;

        [Tooltip("If this property is set, then this cell will ignore the TableLayout Global Cell Padding values - allowing you to set specific values for this cell.")]
        public bool overrideGlobalPadding = false;

        [NonSerialized]
        internal float actualWidth = 0f;
        [NonSerialized]
        internal float actualHeight = 0f;
        [NonSerialized]
        internal float actualX = 0f;
        [NonSerialized]
        internal float actualY = 0f;

        protected Image _image;
        public Image image
        {
            get
            {
                if (_image == null)
                {
                    _image = this.GetComponent<Image>();
                }

                return _image;
            }
        }

        [SerializeField]
        private TableLayout m_tableLayout = null;
        [SerializeField]
        private TableRow m_tableRow = null;

        internal void Initialise(TableLayout tableLayout, TableRow row)
        {
            m_tableLayout = tableLayout;
            m_tableRow = row;
        }
           
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

        public void NotifyTableCellPropertiesChanged()
        {
            if(m_tableLayout != null && m_tableLayout.gameObject.activeInHierarchy) m_tableLayout.CalculateLayoutInputHorizontal();
        }

        public void SetCellPaddingFromTableLayout()
        {
            padding = m_tableLayout.CellPadding;
        }

        public TableRow GetRow()
        {            
            return m_tableRow;            
        }
    }    
}
