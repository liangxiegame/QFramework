using System;

namespace Unidux
{
    public partial class StructStateElement<TValue> : StateElement, ICloneable where TValue : struct
    {
        public TValue Value;

        public override bool Equals(object obj)
        {
            if (obj is StructStateElement<TValue>)
            {
                var target = (StructStateElement<TValue>) obj;
                return this.Value.Equals(target.Value);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public object Clone()
        {
            var newInstance = (StructStateElement<TValue>) Activator.CreateInstance(this.GetType());
            newInstance.Value = this.Value;
            return newInstance;
        }
    }
}