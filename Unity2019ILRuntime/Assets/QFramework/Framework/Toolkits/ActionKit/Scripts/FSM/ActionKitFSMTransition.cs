/****************************************************************************
 * Copyright (c) 2021.3 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
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
using System.Collections.Generic;

namespace QFramework
{
    public class ActionKitFSMTransition
    {
        public virtual HashSet<Type> SrcStateTypes { get; set; }

        public virtual Type DstStateType { get; set; }

        public virtual void OnTransition(ActionKitFSMState srcState, ActionKitFSMState dstState)
        {
        }

        public ActionKitFSMTransition AddSrcState<T>() where T : ActionKitFSMState
        {
            if (SrcStateTypes == null)
            {
                SrcStateTypes = new HashSet<Type>()
                {
                    typeof(T)
                };
            }
            else
            {
                SrcStateTypes.Add(typeof(T));
            }

            return this;
        }

        public ActionKitFSMTransition WithDstState<T>() where T : ActionKitFSMState
        {
            DstStateType = typeof(T);
            return this;
        }
    }


    public class ActionKitFSMTransition<TSrcState, TDstState> : ActionKitFSMTransition
    {
        private Type mSrcStateType = typeof(TSrcState);
        private Type mDstStateType = typeof(TDstState);

        public override HashSet<Type> SrcStateTypes
        {
            get { return new HashSet<Type>() {mSrcStateType}; }
        }

        public override Type DstStateType
        {
            get { return mDstStateType; }
        }
    }
}