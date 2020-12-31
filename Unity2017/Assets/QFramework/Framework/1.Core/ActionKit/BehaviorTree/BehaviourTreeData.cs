using System;
using System.Collections.Generic;

namespace QFramework
{
    public class BehaviourTreeData 
    {
        //------------------------------------------------------
        internal Dictionary<int, TBTActionContext> mContext;
        internal Dictionary<int, TBTActionContext> context 
        {
            get 
            {
                return mContext;
            }
        }
        //------------------------------------------------------
        public BehaviourTreeData()
        {
            mContext = new Dictionary<int, TBTActionContext>();
        }
        ~BehaviourTreeData()
        {
            mContext = null;
        }
    }
}
