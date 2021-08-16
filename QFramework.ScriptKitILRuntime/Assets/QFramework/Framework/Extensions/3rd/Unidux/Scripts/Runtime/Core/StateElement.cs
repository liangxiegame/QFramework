using System;
using Unidux.Util;

namespace Unidux
{
    [Serializable]
    public class StateElement : IState, IStateChanged
    {
        public bool IsStateChanged { get; private set; }

        public void SetStateChanged(bool changed = true)
        {
            this.IsStateChanged = changed;
        }

        public override bool Equals(object obj)
        {
            // It's slow. So in case of requiring performance, override this equality method by your code.
            return EqualityUtil.EntityEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}