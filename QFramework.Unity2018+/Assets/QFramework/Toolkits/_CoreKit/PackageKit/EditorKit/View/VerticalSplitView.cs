/****************************************************************************
 * Copyright (c) IFramework UNDER MIT License
 * Copyright (c) 2022 liangxiegame UNDER MIT License
 *
 * https://github.com/OnClick9927/IFramework
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    /// <summary>
    /// 切分方式
    /// </summary>
    public enum SplitType
    {
        /// <summary>
        /// 纵向
        /// </summary>
        Vertical,

        /// <summary>
        /// 横向
        /// </summary>
        Horizontal
    }

    public enum AnchorType
    {
        UpperLeft = 0,
        UpperCenter = 1,
        UpperRight = 2,
        MiddleLeft = 3,
        MiddleCenter = 4,
        MiddleRight = 5,
        LowerLeft = 6,
        LowerCenter = 7,
        LowerRight = 8
    }

    [Serializable]
    public class VerticalSplitView
    {
        public void DrawExpandButtonLeft()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Space(20);
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
                    
            if (Expand.Value && GUILayout.Button("<"))
            {
                Expand.Value = false;
            }

            GUILayout.EndHorizontal();
        }
        public VerticalSplitView(int splitLeftSize = 200)
        {
            mIMGUIRectBox = EasyIMGUI.BoxWithRect();

            mSplit = splitLeftSize;

            float originSplit = 0;

            Expand.Register(expand =>
            {
                if (expand)
                {
                    mSplit = originSplit;
                }
                else
                {
                    originSplit = mSplit;
                    mSplit = 0;
                }
            });
        }

        private SplitType mSplitType = SplitType.Vertical;

        private float mSplit = 200;

        public BindableProperty<bool> Expand = new BindableProperty<bool>(true);

        public Action<Rect> FirstPan, SecondPan;
        public event System.Action OnBeginResize;
        public event System.Action OnEndResize;

        private IMGUIRectBox mIMGUIRectBox;

        public bool Dragging
        {
            get { return _resizing; }
            private set
            {
                if (_resizing != value)
                {
                    _resizing = value;
                    if (value)
                    {
                        if (OnBeginResize != null)
                        {
                            OnBeginResize();
                        }
                    }
                    else
                    {
                        if (OnEndResize != null)
                        {
                            OnEndResize();
                        }
                    }
                }
            }
        }

        private bool _resizing;

        public void OnGUI(Rect position)
        {
            var rs = position.Split(mSplitType, mSplit, 4);
            var mid = position.SplitRect(mSplitType, mSplit, 4);
            FirstPan?.Invoke(rs[0]);

            SecondPan?.Invoke(rs[1]);

            mIMGUIRectBox?.Rect(mid)?.DrawGUI();

            var e = Event.current;
            if (mid.Contains(e.mousePosition))
            {
                if (mSplitType == SplitType.Vertical)
                    EditorGUIUtility.AddCursorRect(mid, MouseCursor.ResizeHorizontal);
                else
                    EditorGUIUtility.AddCursorRect(mid, MouseCursor.ResizeVertical);
            }

            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (mid.Contains(Event.current.mousePosition))
                    {
                        Dragging = true;
                    }

                    break;
                case EventType.MouseDrag:
                    if (Dragging)
                    {
                        switch (mSplitType)
                        {
                            case SplitType.Vertical:
                                mSplit += Event.current.delta.x;
                                break;
                            case SplitType.Horizontal:
                                mSplit += Event.current.delta.y;
                                break;
                        }

                        mSplit = Mathf.Clamp(mSplit, 100, position.width - 100);
                    }

                    break;
                case EventType.MouseUp:
                    if (Dragging)
                    {
                        Dragging = false;
                    }

                    break;
            }
        }

        public void DrawExpandButtonRight()
        {
            GUILayout.BeginHorizontal();
            if (!Expand.Value && GUILayout.Button(">"))
            {
                Expand.Value = true;
            }

            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical();
            GUILayout.Space(20);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }
    }

    public static class RectExtension
    {
        public static Rect Zoom(this Rect rect, AnchorType type, float pixel)
        {
            return Zoom(rect, type, new Vector2(pixel, pixel));
        }

        public static Rect Zoom(this Rect rect, AnchorType type, Vector2 pixelOffset)
        {
            float tempW = rect.width + pixelOffset.x;
            float tempH = rect.height + pixelOffset.y;
            switch (type)
            {
                case AnchorType.UpperLeft:
                    break;
                case AnchorType.UpperCenter:
                    rect.x -= (tempW - rect.width) / 2;
                    break;
                case AnchorType.UpperRight:
                    rect.x -= tempW - rect.width;
                    break;
                case AnchorType.MiddleLeft:
                    rect.y -= (tempH - rect.height) / 2;
                    break;
                case AnchorType.MiddleCenter:
                    rect.x -= (tempW - rect.width) / 2;
                    rect.y -= (tempH - rect.height) / 2;
                    break;
                case AnchorType.MiddleRight:
                    rect.y -= (tempH - rect.height) / 2;
                    rect.x -= tempW - rect.width;
                    break;
                case AnchorType.LowerLeft:
                    rect.y -= tempH - rect.height;
                    break;
                case AnchorType.LowerCenter:
                    rect.y -= tempH - rect.height;
                    rect.x -= (tempW - rect.width) / 2;
                    break;
                case AnchorType.LowerRight:
                    rect.y -= tempH - rect.height;
                    rect.x -= tempW - rect.width;
                    break;
            }

            rect.width = tempW;
            rect.height = tempH;
            return rect;
        }

        public static Rect CutBottom(this Rect r, float pixels)
        {
            r.yMax -= pixels;
            return r;
        }

        public static Rect CutTop(this Rect r, float pixels)
        {
            r.yMin += pixels;
            return r;
        }

        public static Rect CutRight(this Rect r, float pixels)
        {
            r.xMax -= pixels;
            return r;
        }

        public static Rect CutLeft(this Rect r, float pixels)
        {
            r.xMin += pixels;
            return r;
        }

        public static Rect Cut(this Rect r, float pixels)
        {
            return r.Margin(-pixels);
        }

        public static Rect Margin(this Rect r, float pixels)
        {
            r.xMax += pixels;
            r.xMin -= pixels;
            r.yMax += pixels;
            r.yMin -= pixels;
            return r;
        }

        public static Rect[] Split(this Rect r, SplitType type, float offset, float padding = 0, bool justMid = true)
        {
            switch (type)
            {
                case SplitType.Vertical:
                    return r.VerticalSplit(offset, padding, justMid);
                case SplitType.Horizontal:
                    return r.HorizontalSplit(offset, padding, justMid);
                default:
                    return default(Rect[]);
            }
        }

        public static Rect SplitRect(this Rect r, SplitType type, float offset, float padding = 0)
        {
            switch (type)
            {
                case SplitType.Vertical:
                    return r.VerticalSplitRect(offset, padding);
                case SplitType.Horizontal:
                    return r.HorizontalSplitRect(offset, padding);
                default:
                    return default(Rect);
            }
        }

        public static Rect[] VerticalSplit(this Rect r, float width, float padding = 0, bool justMid = true)
        {
            if (justMid)
                return new Rect[2]
                {
                    r.CutRight((int)(r.width - width)).CutRight(padding).CutRight(-Mathf.CeilToInt(padding / 2f)),
                    r.CutLeft(width).CutLeft(padding).CutLeft(-Mathf.FloorToInt(padding / 2f))
                };
            return new Rect[2]
            {
                r.CutRight((int)(r.width - width)).Cut(padding).CutRight(-Mathf.CeilToInt(padding / 2f)),
                r.CutLeft(width).Cut(padding).CutLeft(-Mathf.FloorToInt(padding / 2f))
            };
        }

        public static Rect[] HorizontalSplit(this Rect r, float height, float padding = 0, bool justMid = true)
        {
            if (justMid)
                return new Rect[2]
                {
                    r.CutBottom((int)(r.height - height)).CutBottom(padding).CutBottom(-Mathf.CeilToInt(padding / 2f)),
                    r.CutTop(height).CutTop(padding).CutTop(-Mathf.FloorToInt(padding / 2f))
                };
            return new Rect[2]
            {
                r.CutBottom((int)(r.height - height)).Cut(padding).CutBottom(-Mathf.CeilToInt(padding / 2f)),
                r.CutTop(height).Cut(padding).CutTop(-Mathf.FloorToInt(padding / 2f))
            };
        }

        public static Rect HorizontalSplitRect(this Rect r, float height, float padding = 0)
        {
            Rect rect = r.CutBottom((int)(r.height - height)).Cut(padding).CutBottom(-Mathf.CeilToInt(padding / 2f));
            rect.y += rect.height;
            rect.height = padding;
            return rect;
        }

        public static Rect VerticalSplitRect(this Rect r, float width, float padding = 0)
        {
            Rect rect = r.CutRight((int)(r.width - width)).Cut(padding).CutRight(-Mathf.CeilToInt(padding / 2f));
            rect.x += rect.width;
            rect.width = padding;
            return rect;
        }
    }
}
#endif