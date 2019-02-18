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
        [Conditional("DEBUG")]
        public static void Invariant<T>(Expression<Func<T>> parameter, Func<bool> condition)
        {
            var memberExpression = parameter.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ApplicationException("Can only use a member expression with Argument.NotNull.");
            }

            if (!condition())
            {
                var parameterName = memberExpression.Member.Name;
                var stackTrace = new StackTrace(true);
                var stackFrames = stackTrace.GetFrames();
                throw new ArgumentException(parameterName, memberExpression.Type.Name + " parameter failed invariant condition, Function: " + stackFrames[1].GetMethod().Name);
            }
        }


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

        [Conditional("DEBUG")] 
        public static void ArrayIndex(Expression<Func<int>> parameter)
        {
            var memberExpression = parameter.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ApplicationException("Can only use a member expression with Argument.ArrayIndex.");
            }

            if (memberExpression.Type != typeof(int))
            {
                throw new ApplicationException("ArrayIndex can only be used with int arguments, type was " + memberExpression.Type.Name);
            }

            var value = parameter.Compile().Invoke();
            if (value < 0)
            {
                var parameterName = memberExpression.Member.Name;
                var stackTrace = new StackTrace(true);
                var stackFrames = stackTrace.GetFrames();
                throw new ArgumentException("Negative array index is invalid. Function: " + stackFrames[1].GetMethod().Name, parameterName);
            }
        }

        [Conditional("DEBUG")] 
        public static void ArrayIndex(Expression<Func<int>> parameter, int maxArrayElements)
        {
            var memberExpression = parameter.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ApplicationException("Can only use a member expression with Argument.ArrayIndex.");
            }

            if (memberExpression.Type != typeof(int))
            {
                throw new ApplicationException("ArrayIndex can only be used with int arguments, type was " + memberExpression.Type.Name);
            }

            var value = parameter.Compile().Invoke();
            if (value < 0)
            {
                var parameterName = memberExpression.Member.Name;
                var stackTrace = new StackTrace(true);
                var stackFrames = stackTrace.GetFrames();
                throw new ArgumentException("Negative array index is invalid. Index: " + value + ", Function: " + stackFrames[1].GetMethod().Name, parameterName);
            }
            else if (value >= maxArrayElements)
            {
                var parameterName = memberExpression.Member.Name;
                var stackTrace = new StackTrace(true);
                var stackFrames = stackTrace.GetFrames();
                throw new ArgumentException("Array access out of bounds index is invalid. Max should be: " + maxArrayElements + ", Index is: " + value + ", Function: " + stackFrames[1].GetMethod().Name, parameterName);
            }
        }
    }
}

