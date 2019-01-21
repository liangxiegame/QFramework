using System;
using System.Linq.Expressions;
using System.Reflection;
using RSG.Utils;

namespace UnityEditorUI
{
    /// <summary>
    /// Binds an event like a button click to a method in the view model.
    /// </summary>
    public interface IEventBinding<WidgetT>
    {
        /// <summary>
        /// Configure the event to bind to later.
        /// </summary>
        WidgetT Bind(string methodName);

        /// <summary>
        /// Configure the event to bind to later.
        /// </summary>
        WidgetT Bind(Expression<Action> methodExpression);
    }

    /// <summary>
    /// Binds an event like a button click to a method in the view model.
    /// </summary>
    internal class EventBinding<WidgetT> : IEventBinding<WidgetT>
    {
        private readonly WidgetT mParentWidget;
        private string mBoundMethodName;
        private MethodInfo mBoundMethod;
        private object viewModel;

        public void BindViewModel(object newViewModel)
        {
            viewModel = newViewModel;
            if (!string.IsNullOrEmpty(mBoundMethodName))
            {
                Type viewModelType = newViewModel.GetType();
                mBoundMethod = viewModelType.GetMethod(mBoundMethodName);
                if (mBoundMethod == null)
                {
                    throw new ApplicationException("Expected method " + mBoundMethodName + " not found on type " + viewModelType.Name + ".");
                }
            }
        }

        /// <summary>
        /// Creates an EventBinding with a reference to the widget using it (used for API fluency)
        /// </summary>
        internal EventBinding(WidgetT parentWidget)
        {
            this.mParentWidget = parentWidget;
        }

        /// <summary>
        /// Invokes the method bound to this EventBinding
        /// </summary>
        internal void Invoke()
        {
            if (mBoundMethod != null)
            {
                mBoundMethod.Invoke(viewModel, null);
            }
        }

        /// <summary>
        /// Configure the event to bind to later.
        /// </summary>
        public WidgetT Bind(string methodName)
        {
            Argument.StringNotNullOrEmpty(() => methodName);

            mBoundMethodName = methodName;

            return mParentWidget;
        }

        /// <summary>
        /// Configure the event to bind to later.
        /// </summary>
        public WidgetT Bind(Expression<Action> methodExpression)
        {
            Argument.NotNull(() => methodExpression);

            return Bind(GetMethodName(methodExpression));
        }

        private static string GetMethodName(Expression<Action> methodExpression)
        {
            var expr = (MethodCallExpression)methodExpression.Body;
            return expr.Method.Name;
        }
    }
}
