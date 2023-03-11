using System;
using System.Text;
using UnityEngine;

namespace PnFramework
{
    public class VernierArray<T>
    {
        protected T[] data;
        protected int first = 0;
        protected int N = 0;

        public int Count => N;
        public int Capacity => data.Length;
        /// <summary>
        /// 数组是否为空
        /// </summary>
        public bool IsEmpty => N == 0;

        public VernierArray(int capacity) => data = new T[capacity == 0 ? 2 : capacity];
        public VernierArray() : this(10) { }

        /// <summary>
        /// 数组索引器 []
        /// </summary>
        /// <param name="index">索引位置</param>
        public virtual T this[int index]
        {
            get { IsLegal(index); return data[(first + index) % N]; }
            set { IsLegal(index); data[(first + index) % N] = value; }
        }
        /// <summary>
        /// 游标指针向左移动一个位置[相当于把尾部往头部移动]
        /// </summary>
        public virtual void LoopPos() => first = (N + first - 1) % N;
        /// <summary>
        /// 游标指针向右移动一个位置[相当于把头部往尾部移动]
        /// </summary>
        public virtual void LoopNeg() => first = (++first) % N;
        /// <summary>
        /// 查看头部元素 
        /// </summary>
        public T GetFirst() { CheckIsEmpty(); return data[first]; }
        /// <summary>
        /// 查看尾部元素 
        /// </summary>
        public T GetLast() { CheckIsEmpty(); return this[N - 1]; }
        /// <summary>
        /// 在游标前添加[往末尾添加一个元素]
        /// </summary>
        public virtual void AddLast(T e)
        {
            if (N == data.Length) ResetCapacity(2 * data.Length);
            for (int i = N - 1; i >= first; i--) data[i + 1] = data[i];
            data[first] = e; N++; LoopNeg();
        }
        /// <summary>
        /// 清空数组 []
        /// </summary>
        public virtual void Clear() { Array.Clear(data, 0, data.Length); first = 0; N = 0; }

        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            res.Append($"{GetType().Name}:  Count = {N}   capacity = {data.Length} \n[");
            for (int i = first; i < first + N; i++)
            {
                res.Append(data[i % N]);
                if ((i + 1) % N != first) res.Append(",");
                Debug.Log(i);
            }
            return res.Append("]").ToString();
        }
        /// <summary>
        /// 重置数组容量 []
        /// </summary>
        /// <param name="newCapacity">新容量</param>
        protected virtual void ResetCapacity(int newCapacity)
        {
            T[] newData = new T[newCapacity];
            for (int i = 0; i < N; i++) newData[i] = data[i];
            data = newData;
        }
        /// <summary>
        /// 检车索引是否合法
        /// </summary>
        protected void IsLegal(int index) { if (index < 0 || index > N) throw new ArgumentException("数组索引越界"); }
        /// <summary>
        /// 检查数组是否为空
        /// </summary>
        protected void CheckIsEmpty() { if (N == 0) throw new InvalidOperationException("数组为空"); }
    }
}