using System;
using System.Diagnostics;

namespace QF.GraphDesigner
{
    public class Problem
    {
        private StackTrace _stackTrace;
        public Exception Exception { get; set; }
        public IItem Source { get; set; }

        public StackTrace StackTrace
        {
            get { return _stackTrace ?? (Exception == null ? null : _stackTrace = new StackTrace(Exception)); }
            set { _stackTrace = value; }
        }
    }
}