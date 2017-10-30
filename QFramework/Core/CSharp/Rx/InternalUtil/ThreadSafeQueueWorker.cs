using System;

namespace QFramework.InternalUtil
{
    public class ThreadSafeQueueWorker
    {
        const int MaxArrayLength = 0X7FEFFFFF;
        const int InitialSize = 16;

        object mGate = new object();
        bool mDequing = false;

        int mActionListCount = 0;
        Action<object>[] mActionList = new Action<object>[InitialSize];
        object[] mActionStates = new object[InitialSize];

        int mWaitingListCount = 0;
        Action<object>[] mWaitingList = new Action<object>[InitialSize];
        object[] mWaitingStates = new object[InitialSize];

        public void Enqueue(Action<object> action, object state)
        {
            lock (mGate)
            {
                if (mDequing)
                {
                    // Ensure Capacity
                    if (mWaitingList.Length == mWaitingListCount)
                    {
                        var newLength = mWaitingListCount * 2;
                        if ((uint) newLength > MaxArrayLength) newLength = MaxArrayLength;

                        var newArray = new Action<object>[newLength];
                        var newArrayState = new object[newLength];
                        Array.Copy(mWaitingList, newArray, mWaitingListCount);
                        Array.Copy(mWaitingStates, newArrayState, mWaitingListCount);
                        mWaitingList = newArray;
                        mWaitingStates = newArrayState;
                    }
                    mWaitingList[mWaitingListCount] = action;
                    mWaitingStates[mWaitingListCount] = state;
                    mWaitingListCount++;
                }
                else
                {
                    // Ensure Capacity
                    if (mActionList.Length == mActionListCount)
                    {
                        var newLength = mActionListCount * 2;
                        if ((uint) newLength > MaxArrayLength) newLength = MaxArrayLength;

                        var newArray = new Action<object>[newLength];
                        var newArrayState = new object[newLength];
                        Array.Copy(mActionList, newArray, mActionListCount);
                        Array.Copy(mActionStates, newArrayState, mActionListCount);
                        mActionList = newArray;
                        mActionStates = newArrayState;
                    }
                    mActionList[mActionListCount] = action;
                    mActionStates[mActionListCount] = state;
                    mActionListCount++;
                }
            }
        }

        public void ExecuteAll(Action<Exception> unhandledExceptionCallback)
        {
            lock (mGate)
            {
                if (mActionListCount == 0) return;

                mDequing = true;
            }

            for (int i = 0; i < mActionListCount; i++)
            {
                var action = mActionList[i];
                var state = mActionStates[i];
                try
                {
                    action(state);
                }
                catch (Exception ex)
                {
                    unhandledExceptionCallback(ex);
                }
                finally
                {
                    // Clear
                    mActionList[i] = null;
                    mActionStates[i] = null;
                }
            }

            lock (mGate)
            {
                mDequing = false;

                var swapTempActionList = mActionList;
                var swapTempActionStates = mActionStates;

                mActionListCount = mWaitingListCount;
                mActionList = mWaitingList;
                mActionStates = mWaitingStates;

                mWaitingListCount = 0;
                mWaitingList = swapTempActionList;
                mWaitingStates = swapTempActionStates;
            }
        }
    }
}