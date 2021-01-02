#define TableLayoutPresent

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tables
{
    [RequireComponent(typeof(RectTransform))]
    public class TableLayout : LayoutGroup, ILayoutSelfController
    {
        public Sprite RowBackgroundImage;
        public Color RowBackgroundColor = new Color(0, 0, 0, 0);

        public Sprite CellBackgroundImage;
        public Color CellBackgroundColor = new Color(0,0,0,0);

        [Tooltip("If this is set, then this TableLayout will automatically add columns if there are more cells than columns on any row (this includes ColumnSpan checks)")]
        public bool AutomaticallyAddColumns = true;
        [Tooltip("If this is set, then this TableLayout will automatically remove any columns with no cells in them in any row (at the END of the row)")]
        public bool AutomaticallyRemoveEmptyColumns = true;
        public List<float> ColumnWidths = new List<float>();

        [Tooltip("If this is set, then the cellpadding set here will override any padding settings set on individual cells")]
        public bool UseGlobalCellPadding = true;
        public RectOffset CellPadding = new RectOffset();

        public float CellSpacing = 0f;

        public bool AutoCalculateHeight = false;        

        public List<TableRow> Rows
        {
            get
            {
                return GetComponentsInChildren<TableRow>()
                        .Where(tr => tr.transform.parent == this.transform)
                        .ToList();                
            }
        }

        private DrivenRectTransformTracker _tracker;

        private LayoutElement _layoutElement;

        protected override void Awake()
        {
            base.Awake();

            _layoutElement = this.GetComponent<LayoutElement>();
        }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            UpdateLayout();
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

        public void UpdateLayout()
        {
            _tracker.Clear();

            var tableRect = this.rectTransform.rect;
            var tableHeight = tableRect.height;
            var tableWidth = tableRect.width;                        

            var rows = this.Rows.ToList();

            if (!rows.Any()) return;

            var rowHeights = new List<float>();
            var availableHeight = Mathf.Max(0, tableHeight - rows.Sum(r => r.preferredHeight) - (this.padding.top + this.padding.bottom) - (this.CellSpacing * (rows.Count - 1)));
            var numberOfAutoSizeRows = rows.Count(r => r.preferredHeight == 0);
            float calculatedAutoSizeRowHeight = 0;
            if (numberOfAutoSizeRows > 0)
            {
                calculatedAutoSizeRowHeight = availableHeight / numberOfAutoSizeRows;
            }

            foreach (var row in rows)
            {
                rowHeights.Add(row.preferredHeight > 0 ? row.preferredHeight : calculatedAutoSizeRowHeight);
            }            

#if !UNITY_IOS
            var columnCount = rows.Max(r => r.Cells.Sum(c => c.columnSpan));
#else
            var columnCount = 0;
            foreach (var row in rows)
            {
                var rowColumnCount = 0;
                foreach (var cell in row.Cells)
                {
                    rowColumnCount += cell.columnSpan;
                }

                columnCount = Math.Max(columnCount, rowColumnCount);
            }
#endif

            if (AutomaticallyRemoveEmptyColumns && columnCount < ColumnWidths.Count)
            {
                ColumnWidths.RemoveRange(columnCount, ColumnWidths.Count - columnCount);
            }

            // 0 == auto-size            
            var availableWidth = tableWidth 
                                - (this.padding.left + this.padding.right) 
                                - (this.CellSpacing * (columnCount - 1));

            foreach (var columnWidth in ColumnWidths)
            {
                availableWidth -= columnWidth;

                if (availableWidth < 0)
                {
                    availableWidth = 0;
                    break;
                }
            }

            for (var c = 0; c < columnCount; c++)
            {
                // If we don't have enough column width entries, then
                // a) if we should automatically add column widths, then add a new zero value (which is translated into AutoSize)
                // c) if we don't want to automatically add column widths, then additional cells over the column count will likely have a width of zero (which is only a problem if that happens)
                if (AutomaticallyAddColumns && ColumnWidths.Count <= c)
                {
                    ColumnWidths.Add(0);
                }
            }

            var numberOfAutoSizeColumns = ColumnWidths.Count(c => c == 0);
            float calculatedAutoSizeColumnWidth = 0f;
            if (numberOfAutoSizeColumns > 0)
            {
                calculatedAutoSizeColumnWidth = availableWidth / numberOfAutoSizeColumns;
            }

            var columnWidths = new List<float>();
            for (var c = 0; c < columnCount; c++)
            {                
                columnWidths.Add(ColumnWidths.Count > c && ColumnWidths[c] != 0 ? ColumnWidths[c] : calculatedAutoSizeColumnWidth);                
            }

            float y = -this.padding.top;
            for (var r = 0; r < rows.Count; r++)
            {
                var row = rows[r];

                row.Initialise(this);
                
                if (!row.dontUseTableRowBackground)
                {
                    row.image.sprite = RowBackgroundImage;
                    row.image.color = RowBackgroundColor;
                }

                var rowHeight = rowHeights[r];
                row.actualHeight = rowHeight;

                var rowRectTransform = row.transform as RectTransform;

                _tracker.Add(this, rowRectTransform, DrivenTransformProperties.All);                

                rowRectTransform.pivot = new Vector2(0, 1);               
                rowRectTransform.anchorMin = new Vector2(0, 1);
                rowRectTransform.anchorMax = new Vector2(0, 1);
                rowRectTransform.localScale = new Vector3(1, 1, 1);
                rowRectTransform.localPosition = Vector3.zero;
                rowRectTransform.localRotation = new Quaternion();

                rowRectTransform.sizeDelta = new Vector2(tableWidth - (this.padding.left + this.padding.right), rowHeight);
                rowRectTransform.anchoredPosition = new Vector2(this.padding.left, y);                
                y -= rowHeight;
                y -= CellSpacing;

                float x = 0f;
                int c = 0;
                var cells = row.Cells.ToList();                
                foreach (var cell in cells)
                {
                    float columnWidth = 0f;
                    var endColumn = c + cell.columnSpan;                    

                    for (var c2 = c; c2 < endColumn; c2++)
                    {                        
                        columnWidth += columnWidths[c2];                        
                    }

                    columnWidth += (cell.columnSpan - 1) * CellSpacing;
                    
                    var difference = tableWidth - x;
                    columnWidth = Mathf.Min(columnWidth, difference);

                    cell.actualWidth = columnWidth;
                    cell.actualHeight = rowHeight;
                    cell.actualX = x;

                    if (UseGlobalCellPadding && !cell.overrideGlobalPadding)
                    {
                        cell.padding = new RectOffset(CellPadding.left, CellPadding.right, CellPadding.top, CellPadding.bottom);                        
                    }

                    if (!cell.dontUseTableCellBackground)
                    {
                        cell.image.sprite = this.CellBackgroundImage;
                        cell.image.color = this.CellBackgroundColor;
                    }
                    
                    x += columnWidth + CellSpacing;
                    c = endColumn;
                }

                // Apply the changes to the cells
                row.CalculateLayoutInputHorizontal();
            }

            if (AutoCalculateHeight)
            {
                rectTransform.pivot = new Vector2(rectTransform.pivot.x, 1);                
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, -y);

                rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 1);
                rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 1);

                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0);

                if (_layoutElement != null)
                {
                    _layoutElement.preferredHeight = rectTransform.rect.height;
                }
            }
        }

        public TableRow AddRow()
        {
            return AddRow(ColumnWidths.Count);
        }

        public TableRow AddRow(int cells)
        {
            var row = TableLayoutUtilities.InstantiatePrefab("TableLayout/Row");
            row.name = "Row";            

            row.transform.SetParent(this.transform);
            row.transform.SetAsLastSibling();

            var rowComponent = row.GetComponent<TableRow>();

            for (var x = 0; x < cells; x++)
            {
                rowComponent.AddCell();
            }

            return rowComponent;
        }

        public TableRow AddRow(TableRow row)
        {
            row.transform.SetParent(this.transform);
            row.transform.SetAsLastSibling();

            return row;
        }

        public void ClearRows()
        {
            foreach (var row in Rows)
            {
                if (Application.isPlaying)
                    Destroy(row.gameObject);
                else
                    DestroyImmediate(row.gameObject);
            }            
        }
    }    
}
