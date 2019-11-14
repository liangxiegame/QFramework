using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace RSG.Utils
{
    public class Argument
    {
        // Todo: google debug break C#, only if running in debugger
        [Conditional("DEBUG")] 
        public static void NotNull<T>(Expression<Func<T>> parameter)
        {
            var memberExpression = parameter.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ApplicationException("Can only use a member expression with Argument.NotNull.");
            }

            var value = parameter.Compile().Invoke();
            if (value == null)
            {
                var parameterName = memberExpression.Member.Name;
                var stackTrace = new StackTrace(true);
                var stackFrames = stackTrace.GetFrames();
                var msg = "Parameter type: " + memberExpression.Type.Name + ", Function: " + stackFrames[1].GetMethod().Name;
                //if (Debugger.IsAttached)
                //{
                //    Debugger.Break();
                //}
                throw new ArgumentNullException(parameterName, msg);
            }
        }

        [Conditional("DEBUG")] 
        public static void StringNotNullOrEmpty(Expression<Func<Object>> parameter)
        {
            var memberExpression = parameter.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ApplicationException("Can only use a member expression with Argument.StringNotNullOrEmpty.");
            }

            if (memberExpression.Type != typeof(String))
            {
                throw new ApplicationException("StringNotNullOrEmpty can only be used with string arguments, type was " + memberExpression.Type.Name);
            }

            var value = parameter.Compile().Invoke() as string;
            if (value == null)
            {
                var parameterName = memberExpression.Member.Name;
                var stackTrace = new StackTrace(true);
                var stackFrames = stackTrace.GetFrames();
                throw new ArgumentNullException(parameterName, "Parameter type: " + memberExpression.Type.Name + ", Function: " + stackFrames[1].GetMethod().Name);
            }

            if (value == string.Empty)
            {
                var parameterName = memberExpression.Member.Name;
                var stackTrace = new StackTrace(true);
                var stackFrames = stackTrace.GetFrames();
                throw new ArgumentException("Empty string parameter. Function: " + stackFrames[1].GetMethod().Name, parameterName);
            }
        }
    }
}

