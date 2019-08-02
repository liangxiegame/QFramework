using System;
using Unidux.Util;

namespace Unidux
{
    public partial class EntityStateElement<TEntity> : StateElement, ICloneable where TEntity : class, ICloneable, new()
    {
        public TEntity Entity;

        public bool IsReady
        {
            get { return this.Entity != null; }
        }

        public override bool Equals(object obj)
        {
            if (obj is EntityStateElement<TEntity>)
            {
                var target = (EntityStateElement<TEntity>) obj;
                return this.Entity != null
                    ? this.Entity.Equals(target.Entity)
                    : this.Entity == null && target.Entity == null;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Entity != null ? this.Entity.GetHashCode() : base.GetHashCode();
        }

        public object Clone()
        {
            return CloneUtil.CopyEntity(this, Activator.CreateInstance(this.GetType()));
        }
    }
}