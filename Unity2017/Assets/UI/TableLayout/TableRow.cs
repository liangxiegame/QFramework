using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tables
{
    [RequireComponent(typeof(RectTransform))]
    public class TableRow : LayoutGroup
    {
        new List<RectTransform> rectChildren
        {
            get
            {
                List<RectTransform> list = new List<RectTransform>();
                foreach (RectTransform r in this.rectTransform)
                {
                    list.Add(r);
                }
                return list;
            }
        }

        public List<TableCell> Cells
        {
            get
            {
                return GetComponentsInChildren<TableCell>()
                        .Where(tc => tc.transform.parent == this.transform)
                        .ToList();                
            }
        }

        public int CellCount
        {
            get
            {
                return Cells.Count;
            }
        }

        public new float preferredHeight = 0;
        [NonSerialized]
        internal float actualHeight = 0f;

        public bool dontUseTableRowBackground = false;

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

        private DrivenRectTransformTracker _tracker;

        [SerializeField]
        private TableLayout m_tableLayout;

        internal void Initialise(TableLayout tableLayout)
        {
            m_tableLayout = tableLayout;
        }

        protected override void Awake()
        {
            base.Awake();
        }

        public override void CalculateLayoutInputHorizontal()
        {
            _tracker.Clear();

            var cells = Cells;

            foreach (var cell in cells)
            {                
                var rectTransform = ((RectTransform)cell.transform);

                _tracker.Add(this, rectTransform, DrivenTransformProperties.All);

                rectTransform.pivot = new Vector2(0, 1);
                rectTransform.sizeDelta = new Vector2(cell.actualWidth, cell.actualHeight);
                rectTransform.anchoredPosition3D = new Vector3(cell.actualX, cell.actualY, 0);
                rectTransform.localScale = new Vector3(1, 1, 1);
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);                

                cell.Initialise(m_tableLayout, this);

                cell.CalculateLayoutInputHorizontal();
            }            
        }

        public override void CalculateLayoutInputVertical()
        {                        
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
        }

        public override void SetLayoutHorizontal()
        {                        
        }

        public override void SetLayoutVertical()
        {            
        }

        public TableCell AddCell(RectTransform cellContent = null)
        {
            var cell = TableLayoutUtilities.InstantiatePrefab("TableLayout/Cell");
            cell.transform.SetParent(this.transform);
            cell.transform.SetAsLastSibling();
            cell.name = "Cell";

            if (cellContent != null)
            {
                cellContent.SetParent(cell.transform);
                cellContent.transform.localScale = new Vector3(1, 1, 1);
                cellContent.localPosition = Vector3.zero;
            }            

            return cell.GetComponent<TableCell>();
        }

        public TableCell AddCell(TableCell cell)
        {
            cell.transform.SetParent(this.transform);
            cell.transform.SetAsLastSibling();
            cell.transform.localScale = new Vector3(1, 1, 1);

            return cell;
        }

        public void NotifyTableRowPropertiesChanged()
        {
            if(m_tableLayout != null && m_tableLayout.gameObject.activeInHierarchy) m_tableLayout.CalculateLayoutInputHorizontal();
        }

        public TableLayout GetTable()
        {
            return m_tableLayout;
        }

        public void ClearCells()
        {
            foreach (var cell in Cells)
            {
                DestroyImmediate(cell.gameObject);
            }
        }
    }    
}
