/****************************************************************************
 * Copyright (c) 2018 ~ 2021.1 liangxie
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

using System;

namespace QFramework
{
    public interface IMsg
    {
        /// <summary>
        /// EventID
        /// </summary>
        int EventID { get; set; }
		
        /// <summary>
        /// Processed or not
        /// </summary>
        bool Processed { get; set; }
		
        /// <summary>
        /// reusable or not 
        /// </summary>
        bool ReuseAble { get; set; }
		
        int ManagerID { get; }

        void Recycle2Cache();
    }
    
    /// <summary>
    /// msgbody
    /// </summary>
    public class QMsg : IMsg, IPoolable, IPoolType
    {
        /// <summary>
        /// EventID
        /// TODO: raname 2 Id
        /// </summary>
        public virtual int EventID { get; set; }

        /// <summary>
        /// Processed or not
        /// </summary>
        public bool Processed { get; set; }

        /// <summary>
        /// reusable or not 
        /// </summary>
        public bool ReuseAble { get; set; }

        public int ManagerID
        {
            get { return EventID / QMsgSpan.Count * QMsgSpan.Count; }
        }

        public QMsg()
        {
        }

        #region Object Pool

        public static QMsg Allocate<T>(T eventId) where T : IConvertible
        {
            QMsg msg = SafeObjectPool<QMsg>.Instance.Allocate();
            msg.EventID = eventId.ToInt32(null);
            msg.ReuseAble = true;
            return msg;
        }

        public virtual void Recycle2Cache()
        {
            SafeObjectPool<QMsg>.Instance.Recycle(this);
        }

        void IPoolable.OnRecycled()
        {
            Processed = false;
        }

        bool IPoolable.IsRecycled { get; set; }

        #endregion

        #region deprecated since v0.0.5
        
        //[Obsolete("deprecated,use allocate instead")]
        public QMsg(int eventID)
        {
            EventID = eventID;
        }

        #endregion
    }
}