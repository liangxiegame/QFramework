// /****************************************************************************
//  * Copyright (c) 2018 Karsion(моп╛)
//  * 
//  * http://qframework.io
//  * https://github.com/liangxiegame/QFramework
//  * 
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  * 
//  * The above copyright notice and this permission notice shall be included in
//  * all copies or substantial portions of the Software.
//  * 
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  * THE SOFTWARE.
//  ****************************************************************************/
using UnityEditor;
using System.Collections;
using UnityEngine;

namespace QFramework
{
    public enum DragResultType
    {
        NoDrag,
        Dragging,
        Accepted,
        Ineffective,
        Canceled,
        Click
    }

    public enum DragDirection
    {
        Auto,
        Vertical,
        Horizontal
    }

    public struct DragResult
    {
        public DragResultType outcome;
        public int movedFromIndex, movedToIndex;

        public DragResult(DragResultType outcome, int movedFromIndex = -1, int movedToIndex = -1)
        {
            this.outcome = outcome;
            this.movedFromIndex = movedFromIndex;
            this.movedToIndex = movedToIndex;
        }
    }

    public static class MonoInspectorDrag
    {
        // Default drag color
        public static readonly Color DefaultDragColor = new Color(0.2f, 0.4f, 0.8f, 0.6f);
        private static DragData dragData;
        private static int nDragId;
        private static bool isWaitingToApplyDrag;
        private static bool isDrag;
        private static UnityEditor.Editor editor;

        #region Public Methods

        public static void StartDrag(int dragId, UnityEditor.Editor editor, IList draggableList,
            int draggedItemIndex,
            object optionalData = null)
        {
            if (dragData != null)
            {
                return;
            }

            Reset();
            MonoInspectorDrag.editor = editor;
            nDragId = dragId;
            dragData = new DragData(draggableList, draggableList[draggedItemIndex], draggedItemIndex, optionalData);
            isDrag = true;
        }

        public static DragResult Drag(int dragId, IList draggableList, int currDraggableItemIndex,
            Rect? lastGUIRect = null, DragDirection direction = DragDirection.Auto)
        {
            return Drag(dragId, draggableList, currDraggableItemIndex, DefaultDragColor, lastGUIRect, direction);
        }

        public static void FlatDivider(Rect rect, Color? color = null)
        {
            Color prevBgColor = GUI.backgroundColor;
            if (color != null)
            {
                GUI.backgroundColor = (Color) color;
            }

            GUI.Box(rect, "", EditorStyles.textArea);
            GUI.backgroundColor = prevBgColor;
        }

        public static DragResult Drag(int dragId, IList draggableList, int currDraggableItemIndex,
            Color dragEvidenceColor, Rect? lastGUIRect = null, DragDirection direction = DragDirection.Auto)
        {
            if (dragData == null || nDragId != dragId)
            {
                return new DragResult(DragResultType.NoDrag);
            }

            if (isWaitingToApplyDrag)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    Event.current.type = EventType.Used;
                }

                if (Event.current.type == EventType.Used)
                {
                    ApplyDrag();
                }

                return new DragResult(DragResultType.Dragging, dragData.nDraggedItemIndex, dragData.nCurrDragIndex);
            }

            dragData.draggableList =
                draggableList; // Reassign in case of references that change every call (like with EditorBuildSettings.scenes)
            int listCount = dragData.draggableList.Count;
            if (currDraggableItemIndex == 0 && Event.current.type == EventType.Repaint)
            {
                dragData.currDragSet = false;
            }

            if (!dragData.currDragSet)
            {
                // Find and store eventual drag position
                Rect lastRect = lastGUIRect == null ? GUILayoutUtility.GetLastRect() : (Rect) lastGUIRect;
                if (!dragData.hasHorizontalSet && Event.current.type == EventType.Repaint)
                {
                    // Set drag direction
                    switch (direction)
                    {
                        case DragDirection.Auto:
                            // Auto-determine if drag is also horizontal
                            if (currDraggableItemIndex == 0)
                            {
                                dragData.lastRect = lastRect;
                            }
                            else if (dragData.lastRect.width > 0)
                            {
                                dragData.hasHorizontalSet = true;
                                dragData.hasHorizontalEls =
                                    !Mathf.Approximately(dragData.lastRect.xMin, lastRect.xMin);
                            }

                            break;
                        case DragDirection.Vertical:
                            dragData.hasHorizontalSet = true;
                            dragData.hasHorizontalEls = false;
                            break;
                        case DragDirection.Horizontal:
                            dragData.hasHorizontalSet = true;
                            dragData.hasHorizontalEls = true;
                            break;
                    }
                }

                Vector2 lastRectMiddleP = lastRect.center;
                Vector2 mouseP = Event.current.mousePosition;
                if (
                    currDraggableItemIndex <= listCount - 1 && lastRect.Contains(mouseP)
                                                            && (dragData.hasHorizontalEls &&
                                                                mouseP.x <= lastRectMiddleP.x ||
                                                                !dragData.hasHorizontalEls &&
                                                                mouseP.y <= lastRectMiddleP.y)
                )
                {
                    if (isDrag)
                    {
                        Rect evidenceR = dragData.hasHorizontalEls
                            ? new Rect(lastRect.xMin, lastRect.yMin, 5, lastRect.height)
                            : new Rect(lastRect.xMin, lastRect.yMin, lastRect.width, 5);
                        FlatDivider(evidenceR, dragEvidenceColor);
                    }

                    dragData.nCurrDragIndex = currDraggableItemIndex;
                    dragData.currDragSet = true;
                }
                else if (currDraggableItemIndex <= listCount - 1 && lastRect.Contains(mouseP)
                                                                 && (dragData.hasHorizontalEls &&
                                                                     mouseP.x > lastRectMiddleP.x ||
                                                                     !dragData.hasHorizontalEls &&
                                                                     mouseP.y > lastRectMiddleP.y))
                {
                    if (isDrag)
                    {
                        Rect evidenceR = dragData.hasHorizontalEls
                            ? new Rect(lastRect.xMax - 5, lastRect.yMin, 5, lastRect.height)
                            : new Rect(lastRect.xMin, lastRect.yMax - 5, lastRect.width, 5);
                        FlatDivider(evidenceR, dragEvidenceColor);
                    }

                    dragData.nCurrDragIndex = currDraggableItemIndex + 1;
                    dragData.currDragSet = true;
                }
                else if (
                    currDraggableItemIndex == 0 && !lastRect.Contains(mouseP)
                                                && (dragData.hasHorizontalEls &&
                                                    (mouseP.x <= lastRect.x || mouseP.y < lastRect.y) ||
                                                    !dragData.hasHorizontalEls && mouseP.y <= lastRectMiddleP.y)
                )
                {
                    // First, with mouse above or aside drag areas
                    if (isDrag)
                    {
                        Rect evidenceR = dragData.hasHorizontalEls
                            ? new Rect(lastRect.xMin, lastRect.yMin, 5, lastRect.height)
                            : new Rect(lastRect.xMin, lastRect.yMin, lastRect.width, 5);
                        FlatDivider(evidenceR, dragEvidenceColor);
                    }

                    dragData.nCurrDragIndex = currDraggableItemIndex;
                    dragData.currDragSet = true;
                }
                else if (
                    currDraggableItemIndex >= listCount - 1
                    && (dragData.hasHorizontalEls && (mouseP.x > lastRectMiddleP.x || mouseP.y > lastRect.yMax) ||
                        !dragData.hasHorizontalEls && mouseP.y > lastRectMiddleP.y)
                )
                {
                    if (isDrag)
                    {
                        Rect evidenceR = dragData.hasHorizontalEls
                            ? new Rect(lastRect.xMax - 5, lastRect.yMin, 5, lastRect.height)
                            : new Rect(lastRect.xMin, lastRect.yMax - 5, lastRect.width, 5);
                        FlatDivider(evidenceR, dragEvidenceColor);
                    }

                    dragData.nCurrDragIndex = listCount;
                    dragData.currDragSet = true;
                }
            }

            if (dragData.nDraggedItemIndex == currDraggableItemIndex)
            {
                // Evidence dragged pool
                Color selectionColor = dragEvidenceColor;
                selectionColor.a = 0.35f;
                if (isDrag)
                {
                    FlatDivider(lastGUIRect ?? GUILayoutUtility.GetLastRect(),
                        selectionColor);
                }
            }

            if (GUIUtility.hotControl < 1)
            {
                // End drag
                if (isDrag)
                {
                    return EndDrag(true);
                }

                EndDrag(false);
                return new DragResult(DragResultType.Click);
            }

            return new DragResult(DragResultType.Dragging, dragData.nDraggedItemIndex, dragData.nCurrDragIndex);
        }

        /// <summary>
        ///     Ends the drag operations, and eventually applies the drag outcome.
        ///     Returns TRUE if the position of the dragged item actually changed.
        ///     Called automatically by Drag method. Use it only if you want to force the end of a drag operation.
        /// </summary>
        /// <param name="applyDrag">If TRUE applies the drag results, otherwise simply cancels the drag</param>
        public static DragResult EndDrag(bool applyDrag)
        {
            if (dragData == null)
            {
                return new DragResult(DragResultType.NoDrag);
            }

            int dragFrom = dragData.nDraggedItemIndex;
            int dragTo = dragData.nCurrDragIndex > dragData.nDraggedItemIndex
                ? dragData.nCurrDragIndex - 1
                : dragData.nCurrDragIndex;

            if (applyDrag)
            {
                bool changed = dragData.nCurrDragIndex < dragData.nDraggedItemIndex ||
                               dragData.nCurrDragIndex > dragData.nDraggedItemIndex + 1;
                if (Event.current.type == EventType.Repaint)
                {
                    Event.current.type = EventType.Used;
                }
                else if (Event.current.type == EventType.Used)
                {
                    ApplyDrag();
                }
                else
                {
                    isWaitingToApplyDrag = true;
                }

                DragResultType resultType = changed ? DragResultType.Accepted : DragResultType.Ineffective;
                return new DragResult(resultType, dragFrom, dragTo);
            }

            Reset();
            return new DragResult(DragResultType.Canceled, dragFrom, dragTo);
        }

        #endregion

        private static void ApplyDrag()
        {
            if (dragData == null)
            {
                return;
            }

            int fromIndex = dragData.nDraggedItemIndex;
            int toIndex = dragData.nCurrDragIndex > dragData.nDraggedItemIndex
                ? dragData.nCurrDragIndex - 1
                : dragData.nCurrDragIndex;
            if (toIndex != fromIndex)
            {
                int index = fromIndex;
                while (index > toIndex)
                {
                    index--;
                    dragData.draggableList[index + 1] = dragData.draggableList[index];
                    dragData.draggableList[index] = dragData.draggedItem;
                }

                while (index < toIndex)
                {
                    index++;
                    dragData.draggableList[index - 1] = dragData.draggableList[index];
                    dragData.draggableList[index] = dragData.draggedItem;
                }
            }

            Reset();
            Repaint();
        }

        private static void Repaint()
        {
            if (editor != null)
            {
                editor.Repaint();
            }

            //else if (_editorWindow != null) _editorWindow.Repaint();
        }

        private static void Reset()
        {
            dragData = null;
            nDragId = -1;
            isWaitingToApplyDrag = false;
            isDrag = false;
        }
    }

    internal class DragData
    {
        public readonly object draggedItem; // Dragged element
        public readonly int nDraggedItemIndex;
        public IList draggableList; // Collection within which the drag is being executed
        public int nCurrDragIndex = -1; // Index of current drag position
        public bool currDragSet;
        public Rect lastRect;
        public bool hasHorizontalSet;
        public bool hasHorizontalEls;
        public object optionalData;

        public DragData(IList draggableList, object draggedItem, int nDraggedItemIndex, object optionalData)
        {
            this.draggedItem = draggedItem;
            this.nDraggedItemIndex = nDraggedItemIndex;
            this.draggableList = draggableList;
            this.optionalData = optionalData;
        }
    }
}