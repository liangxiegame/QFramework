using System.Collections.Generic;
using System.Linq;

namespace QF.GraphDesigner
{
    public static class ConnectableExtensions
    {

        public static IEnumerable<ITypedItem> References(this IClassTypeNode node)
        {
            return node.Node.Repository.AllOf<ITypedItem>().Where(p => p.RelatedType == node.Identifier);
        }
        public static IEnumerable<TType> ReferencesOf<TType>(this IClassTypeNode node)
        {
            return node.Node.Repository.AllOf<ITypedItem>().Where(p => p.RelatedType == node.Identifier).OfType<TType>();
        }
        public static TType ReferenceOf<TType>(this IClassTypeNode node) where TType : ITypedItem
        {
            return node.Node.Repository.AllOf<ITypedItem>().OfType<TType>().FirstOrDefault(p => p.RelatedType == node.Identifier);
        }

        //public static IEnumerable<TType> InputsFrom<TType>(this IConnectable connectable)
        //    where TType : IGraphItem
        //{
        //    return connectable.Inputs.Select(p => p.Output).OfType<TType>();
        //}
        //public static TType InputFrom<TType>(this IConnectable connectable)
        //    where TType : IGraphItem
        //{
        //    return connectable.Inputs.Select(p => p.Output).OfType<TType>().FirstOrDefault();
        //}


        //public static IEnumerable<TType> OutputsTo<TType>(this IConnectable connectable)
        //    where TType : IGraphItem
        //{
        //    return connectable.Outputs.Select(p => p.Input).OfType<TType>();
        //}
        //public static TType OutputTo<TType>(this IConnectable connectable)
        //    where TType : IGraphItem
        //{
        //    return connectable.Outputs.Select(p => p.Input).OfType<TType>().FirstOrDefault();
        //}



    }
}