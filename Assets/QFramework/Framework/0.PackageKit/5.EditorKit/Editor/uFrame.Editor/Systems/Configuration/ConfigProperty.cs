using System;

namespace QF.GraphDesigner
{
    public class ConfigProperty<TData, TType>
    {
        public TType Literal { get; set; }
        public Func<TData, TType> Selector { get; set; }

        public ConfigProperty(TType literal)
        {
            Literal = literal;
        }

        public ConfigProperty(Func<TData, TType> selector)
        {
            Selector = selector;
        }

        public TType GetValue(TData data)
        {
            if (Selector != null)
            {
                return Selector(data);
            }
            return Literal;
        }
    }
}