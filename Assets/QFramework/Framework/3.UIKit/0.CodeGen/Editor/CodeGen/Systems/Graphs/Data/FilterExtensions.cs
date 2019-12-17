using System;
using System.Collections.Generic;
using System.Linq;
using QF;
using QFramework;

namespace QFramework.CodeGen
{
    public static class FilterExtensions
    {
        public static void RegisterConnectable(this IQFrameworkContainer container, Type outputType, Type inputType)
        {
            container.RegisterInstance(new RegisteredConnection() { TInputType = inputType, TOutputType = outputType }, outputType.Name + inputType.Name);

        }


        public static IEnumerable<IGraphItem> AllGraphItems(this IGraphFilter filter)
        {
            foreach (var item in filter.FilterNodes)
            {
                yield return item;
                foreach (var child in item.GraphItems)
                {
                    yield return child;
                }
            }
        }

        public static bool IsAllowed(this IGraphFilter filter, object item, Type t)
        {

            if (filter == item) return true;

            if (!AllowedFilterNodes.ContainsKey(filter.GetType())) return false;

            foreach (var x in AllowedFilterNodes[filter.GetType()])
            {
                if (t.IsAssignableFrom(x)) return true;
            }
            return false;
            // return InvertGraphEditor.AllowedFilterNodes[filter.GetType()].Contains(t);
        }

        private static Dictionary<Type, List<Type>> _allowedFilterItems;

        private static Dictionary<Type, List<Type>> _allowedFilterNodes;

        public static Dictionary<Type, List<Type>> AllowedFilterNodes
        {
            get { return _allowedFilterNodes ?? (_allowedFilterNodes = new Dictionary<Type, List<Type>>()); }
            set { _allowedFilterNodes = value; }
        }
    }
}