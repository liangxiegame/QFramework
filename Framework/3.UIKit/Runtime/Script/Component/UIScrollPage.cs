/****************************************************************************
 * Copyright (c) 2018.8 liangxie
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

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

namespace QFramework
{
    public class UIScrollPageChangeEvent : UnityEvent<int, int> {}
    
    public class UIScrollPage : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        ScrollRect rect;

        //页面：0，1，2，3  索引从0开始
        //每页占的比列：0/3=0  1/3=0.333  2/3=0.6666 3/3=1
        //float[] pages = { 0f, 0.333f, 0.6666f, 1f };
        List<float> pages = new List<float>();

        int currentPageIndex = -1;

        //滑动速度
        public float smooting = 4;

        //滑动的起始坐标
        float targethorizontal = 0;

        //是否拖拽结束
        bool isDrag = false;

        /// <summary>
        /// 用于返回一个页码，-1说明page的数据为0
        /// </summary>
        private UIScrollPageChangeEvent mOnPageChanged;

        float startime = 0f;
        float delay = 0.1f;

        void Awake()
        {
            rect = transform.GetComponent<ScrollRect>();
            startime = Time.time;
        }

        void Update()
        {
            if (Time.time < startime + delay) return;
            UpdatePages();
            //如果不判断。当在拖拽的时候要也会执行插值，所以会出现闪烁的效果
            //这里只要在拖动结束的时候。在进行插值
            if (!isDrag && pages.Count > 0)
            {
                rect.horizontalNormalizedPosition = Mathf.Lerp(rect.horizontalNormalizedPosition, targethorizontal, Time.deltaTime * smooting);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDrag = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDrag = false;

            if (eventData != null)
            {
                bool bLeft = eventData.position.x < eventData.pressPosition.x;
                if (bLeft)
                {
                    if (currentPageIndex < pages.Count - 1)
                        currentPageIndex++;
                }
                else
                {
                    if (currentPageIndex > 0)
                        currentPageIndex--;
                }
                mOnPageChanged.Invoke(pages.Count, currentPageIndex);
                targethorizontal = pages[currentPageIndex];
            }

            //float posX = rect.horizontalNormalizedPosition;
            //int index = 0;
            ////假设离第一位最近
            //float offset = Mathf.Abs(pages[index] - posX);
            //for (int i = 1; i < pages.Count; i++)
            //{
            //    float temp = Mathf.Abs(pages[i] - posX);
            //    if (temp < offset)
            //    {
            //        index = i;
            //        //保存当前的偏移量
            //        //如果到最后一页。反翻页。所以要保存该值，如果不保存。你试试效果就知道
            //        offset = temp;
            //    }
            //}

            //if(index!=currentPageIndex)
            //{
            //    currentPageIndex = index;
            //    OnPageChanged(pages.Count, currentPageIndex);
            //}

            ///*
            // 因为这样效果不好。没有滑动效果。比较死板。所以改为插值
            // */
            ////rect.horizontalNormalizedPosition = page[index];


            //targethorizontal = pages[index];
        }

        void UpdatePages()
        {
            // 获取子对象的数量
            int count = this.rect.content.childCount;
            int temp = 0;
            for (int i = 0; i < count; i++)
            {
                if (this.rect.content.GetChild(i).gameObject.activeSelf)
                {
                    temp++;
                }
            }
            count = temp;

            if (pages.Count != count)
            {
                if (count != 0)
                {
                    pages.Clear();
                    for (int i = 0; i < count; i++)
                    {
                        float page = 0;
                        if (count != 1)
                            page = i / ((float) (count - 1));
                        pages.Add(page);
                        //Debug.Log(i.ToString() + " page:" + page.ToString());
                    }
                }
                OnEndDrag(null);
            }
        }

        /// <summary>
        /// force set page index
        /// </summary>
        /// <param name="pageIndex"></param>
        public void SetPage(int pageIndex)
        {
            isDrag = false;
            UpdatePages();

            currentPageIndex = Mathf.Clamp(pageIndex, 0, pages.Count - 1);
            mOnPageChanged.Invoke(pages.Count, currentPageIndex);
            targethorizontal = pages[currentPageIndex];
            rect.horizontalNormalizedPosition = targethorizontal;
        }

        /// <summary>
        /// get all pages' count
        /// </summary>
        public int GetPageCount()
        {
            return pages.Count;
        }

        /// <summary>
        /// get current showed page index
        /// </summary>
        public int GetCurrentPageIndex()
        {
            return currentPageIndex;
        }
        
        /// <summary>
        /// register page change event listener
        /// </summary>
        public void AddPageChangeListener(UnityAction<int, int> listener)
        {
            if (mOnPageChanged == null)
                mOnPageChanged = new UIScrollPageChangeEvent();
            mOnPageChanged.AddListener(listener);
        }

        /// <summary>
        /// remove page change event listener
        /// </summary>
        public void RemovePageChangeListener(UnityAction<int, int> listener)
        {
            if (mOnPageChanged == null)
                return;
            mOnPageChanged.RemoveListener(listener);
        }

        void OnDestroy()
        {
            if (mOnPageChanged != null)
                mOnPageChanged.RemoveAllListeners();
        }
    }
}