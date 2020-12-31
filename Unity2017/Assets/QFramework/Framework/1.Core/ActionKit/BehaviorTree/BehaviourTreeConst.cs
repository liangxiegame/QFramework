using System;

namespace QFramework
{
    public class BehaviourTreeRunningStatus
    {
        //-------------------------------------------------------
        //Any value which is below ZERO means error occurs 
        //-------------------------------------------------------
        //default running status
        public const int EXECUTING   = 0;
        public const int FINISHED    = 1;
        public const int TRANSITION  = 2;
        //-------------------------------------------------------
        //User running status
        //100-999, reserved user executing status
        public const int USER_EXECUTING = 100;
        //>=1000, reserved user finished status
        public const int USER_FINISHED = 1000;
        //-------------------------------------------------------
        static public bool IsOK(int runningStatus)
        {
            return runningStatus == BehaviourTreeRunningStatus.FINISHED ||
                   runningStatus >= BehaviourTreeRunningStatus.USER_FINISHED;
        }
        static public bool IsError(int runningStatus)
        {
            return runningStatus < 0;
        }
        static public bool IsFinished(int runningStatus)
        {
            return IsOK(runningStatus) || IsError(runningStatus);
        }
        static public bool IsExecuting(int runningStatus)
        {
            return !IsFinished(runningStatus);
        }
    }
}
