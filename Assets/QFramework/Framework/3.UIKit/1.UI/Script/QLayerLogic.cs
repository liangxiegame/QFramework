/****************************************************************************
 * Copyright (c) 2018 vin129
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

namespace QFramework
{
    using UnityEngine;
    using System.Collections.Generic;

    public enum AutoLayerTag
    {
        Bottom,
        Comm,
        Top,
        Hide,
    }

    public class QLayerLogic : MonoBehaviour
    {
        private const int mNormalPadding    = 10;
        private const int mLayerMaxPanelNum = 10000;
        private const int diffValue         = 0 - (int) UILevel.Bg;

        public const int BOTTOM_INDEX = 10;
        public const int COMM_INDEX   = BOTTOM_INDEX + mLayerMaxPanelNum * mNormalPadding;
        public const int TOP_INDEX    = COMM_INDEX + 9 * mLayerMaxPanelNum * mNormalPadding;
        public const int HIDE_INDEX   = -10;

        private Transform mAlwayBottomLayer;
        private Transform mCommLayer;
        private Transform mAlwayTopLayer;
        private Transform mHideLayer;

        private int   mBottomOrder = BOTTOM_INDEX;
        private int[] mCommOrder; // TODO 是否别分这么细
        private int   mTopOrder = TOP_INDEX;

        private List<UIPanel>                           ToHideList  = new List<UIPanel>();
        private Dictionary<AutoLayerTag, List<UIPanel>> mUILayerMap = new Dictionary<AutoLayerTag, List<UIPanel>>();

        public void InitLayer(Transform LayerParent)
        {
            mAlwayBottomLayer = new GameObject("BottomLayer").AddComponent<RectTransform>();
            mCommLayer = new GameObject("CommLayer").AddComponent<RectTransform>();
            mAlwayTopLayer = new GameObject("TopLayer").AddComponent<RectTransform>();
            mHideLayer = new GameObject("HideLayer").AddComponent<RectTransform>();

            mAlwayBottomLayer.SetParent(LayerParent);
            mCommLayer.SetParent(LayerParent);
            mAlwayTopLayer.SetParent(LayerParent);
            mHideLayer.SetParent(LayerParent);

            mHideLayer.SetSiblingIndex(0);
            mAlwayBottomLayer.SetSiblingIndex(1);
            mCommLayer.SetSiblingIndex(2);
            mAlwayTopLayer.SetSiblingIndex(3);

            InitailRectTrans(mAlwayBottomLayer as RectTransform);
            InitailRectTrans(mCommLayer as RectTransform);
            InitailRectTrans(mAlwayTopLayer as RectTransform);
            InitailRectTrans(mHideLayer as RectTransform);
            mHideLayer.Hide();

            mCommOrder = new int[8];
            for (int i = 0; i < mCommOrder.Length; i++)
                mCommOrder[i] = COMM_INDEX + i * mLayerMaxPanelNum * mNormalPadding;
        }

        public void ClearUILayerMap()
        {
            mBottomOrder = BOTTOM_INDEX;
            mTopOrder = TOP_INDEX;
            mCommOrder = new int[8];
            for (int i = 0; i < mCommOrder.Length; i++)
                mCommOrder[i] = COMM_INDEX + i * mLayerMaxPanelNum * mNormalPadding;
            mUILayerMap.Clear();
        }

        public void SetLayer(int uiLevel, UIPanel ui)
        {
            if (ui == null)
                return;
            ui.UILayerType = uiLevel;
            switch (uiLevel)
            {
                case (int) UILevel.AlwayBottom:
                    AddUIPanel(AutoLayerTag.Bottom, ui);
                    ui.Transform.SetParent(mAlwayBottomLayer);
                    break;
                case (int) UILevel.AlwayTop:
                    AddUIPanel(AutoLayerTag.Top, ui);
                    ui.Transform.SetParent(mAlwayTopLayer);
                    break;
                default:
                    AddCommUIPanel(uiLevel, ui);
                    ui.Transform.SetParent(mCommLayer);
                    break;
            }

            InitailRectTrans(ui.Transform as RectTransform);
        }

        //TODO隐藏和关闭在重排之前不得进行创建，要约束一下
        public void OnUIPanelClose(UIPanel ui)
        {
            RemoveUILayer(ui);
        }

        private void RemoveUILayer(UIPanel ui, int uiLevel = -10000)
        {
            if (uiLevel == -10000)
                uiLevel = ui.UILayerType;
            switch (uiLevel)
            {
                case (int) UILevel.AlwayBottom:
                    if (RemoveUIPanel(AutoLayerTag.Bottom, ui))
                    {
                        //mBottomOrder -= mNormalPadding;
                    }

                    break;
                case (int) UILevel.AlwayTop:
                    if (RemoveUIPanel(AutoLayerTag.Top, ui))
                    {
                        //mTopOrder -= mNormalPadding;
                    }

                    break;
                default:
                    if (RemoveUIPanel(AutoLayerTag.Comm, ui))
                    {
                        Log.I(ui.name);
                        if (uiLevel + diffValue < 0 || uiLevel - diffValue >= mCommOrder.Length)
                        {
                        }
                        else
                        {
                            //mCommOrder[uiLevel + diffValue] -= mNormalPadding;
                        }
                    }

                    break;
            }
        }

        public void OnUIPanelShow(UIPanel ui)
        {
            if (mUILayerMap.ContainsKey(AutoLayerTag.Hide) && mUILayerMap[AutoLayerTag.Hide].Contains(ui))
            {
                mUILayerMap[AutoLayerTag.Hide].Remove(ui);
                SetLayer(ui.UILayerType, ui);
            }
        }

        public void OnUIPanelHide(UIPanel ui)
        {
            RemoveUILayer(ui);
            AddUIPanel(AutoLayerTag.Hide, ui);
            ui.Transform.SetParent(mHideLayer);
            InitailRectTrans(ui.Transform as RectTransform);
        }

        private int SortCompare(UIPanel a, UIPanel b)
        {
            return a.LayerSortIndex.CompareTo(b.LayerSortIndex);
        }

        //排序
        public void SortAllUIPanel()
        {
            Log.I("Sortting");
            CheckLayerMap();
            if (mUILayerMap.ContainsKey(AutoLayerTag.Bottom))
            {
                mUILayerMap[AutoLayerTag.Bottom].Sort(SortCompare);
                mBottomOrder = BOTTOM_INDEX;
                ReSetLayerList(mUILayerMap[AutoLayerTag.Bottom], ref mBottomOrder);
            }

            if (mUILayerMap.ContainsKey(AutoLayerTag.Top))
            {
                mUILayerMap[AutoLayerTag.Top].Sort(SortCompare);
                mTopOrder = TOP_INDEX;
                ReSetLayerList(mUILayerMap[AutoLayerTag.Top], ref mTopOrder);
            }

            if (mUILayerMap.ContainsKey(AutoLayerTag.Comm))
            {
                mUILayerMap[AutoLayerTag.Comm].Sort(SortCompare);
                for (int i = 0; i < mCommOrder.Length; i++)
                    mCommOrder[i] = COMM_INDEX + i * mLayerMaxPanelNum * mNormalPadding;
                ReSetCommLaterList();
            }
        }


        private void CheckLayerMap()
        {
            if (mUILayerMap.ContainsKey(AutoLayerTag.Bottom))
            {
                var list = mUILayerMap[AutoLayerTag.Bottom];
                CheckLayerList(ref list);
            }

            if (mUILayerMap.ContainsKey(AutoLayerTag.Top))
            {
                var list = mUILayerMap[AutoLayerTag.Top];
                CheckLayerList(ref list);
            }

            if (mUILayerMap.ContainsKey(AutoLayerTag.Comm))
            {
                var list = mUILayerMap[AutoLayerTag.Comm];
                CheckLayerList(ref list);
            }
        }

        private void CheckLayerList(ref List<UIPanel> list)
        {
            ToHideList.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].LayerSortIndex == HIDE_INDEX)
                {
                    ToHideList.Add(list[i]);
                }
            }

            for (int i = 0; i < ToHideList.Count; i++)
                QUIManager.Instance.HideUI(ToHideList[i].Transform.gameObject.name);
        }

        private void ReSetLayerList(List<UIPanel> list, ref int order)
        {
            for (int i = 0; i < list.Count; i++)
            {
                order += mNormalPadding;
                list[i].SetSiblingIndexAndNewLayerIndex(i, order);
            }
        }

        private void ReSetCommLaterList()
        {
            if (!mUILayerMap.ContainsKey(AutoLayerTag.Comm))
                return;
            var list = mUILayerMap[AutoLayerTag.Comm];
            for (int i = 0; i < list.Count; i++)
            {
                int uiLevel = list[i].UILayerType;
                if (uiLevel + diffValue < 0 || uiLevel - diffValue >= mCommOrder.Length)
                    continue;
                else
                {
                    mCommOrder[uiLevel + diffValue] += mNormalPadding;
                    list[i].SetSiblingIndexAndNewLayerIndex(i, mCommOrder[uiLevel + diffValue]);
                }
            }
        }



        private void AddCommUIPanel(int uiLevel, UIPanel ui)
        {
            if (!mUILayerMap.ContainsKey(AutoLayerTag.Comm))
                mUILayerMap.Add(AutoLayerTag.Comm, new List<UIPanel>());
            mUILayerMap[AutoLayerTag.Comm].Add(ui);

            mCommOrder[uiLevel + diffValue] += mNormalPadding;

            var order = HIDE_INDEX;
            if (uiLevel + diffValue < 0 || uiLevel - diffValue >= mCommOrder.Length)
            {
                ui.LayerSortIndex = order;
                return;
            }

            order = mCommOrder[uiLevel + diffValue];
            ui.LayerSortIndex = order;
        }


        private void AddUIPanel(AutoLayerTag tag, UIPanel ui)
        {
            if (!mUILayerMap.ContainsKey(tag))
                mUILayerMap.Add(tag, new List<UIPanel>());
            mUILayerMap[tag].Add(ui);
            var order = HIDE_INDEX;
            switch (tag)
            {
                case AutoLayerTag.Bottom:
                    mBottomOrder += mNormalPadding;
                    order = mBottomOrder;
                    break;
                case AutoLayerTag.Top:
                    mTopOrder += mNormalPadding;
                    order = mTopOrder;
                    break;
                case AutoLayerTag.Hide:
                    break;
            }

            ui.LayerSortIndex = order;
        }


        private bool RemoveUIPanel(AutoLayerTag tag, UIPanel ui)
        {
            if (mUILayerMap.ContainsKey(tag) && mUILayerMap[tag].Contains(ui))
                return mUILayerMap[tag].Remove(ui);
            return false;
        }

        public static void InitailRectTrans(RectTransform RectTrans)
        {
            RectTrans.offsetMin = Vector2.zero;
            RectTrans.offsetMax = Vector2.zero;
            RectTrans.anchoredPosition3D = Vector3.zero;
            RectTrans.anchorMin = Vector2.zero;
            RectTrans.anchorMax = Vector2.one;

            RectTrans.LocalScaleIdentity();
        }
    }
}