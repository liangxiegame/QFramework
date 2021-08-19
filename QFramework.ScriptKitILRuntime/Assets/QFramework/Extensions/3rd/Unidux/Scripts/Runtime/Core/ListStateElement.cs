using System;
using System.Collections.Generic;
using System.Linq;

namespace Unidux
{
    public class ListStateElement<TEntity> : StateElement, ICloneable where TEntity : ICloneable
    {
        public IList<TEntity> Entities = new List<TEntity>();

        public ListStateElement()
        {
        }

        public bool IsReady
        {
            get { return this.Entities.Count > 0; }
        }

        public override bool Equals(object obj)
        {
            if (obj is ListStateElement<TEntity>)
            {
                var target = (ListStateElement<TEntity>) obj;
                return this.Entities.SequenceEqual(target.Entities);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Entities.GetHashCode();
        }

        public object Clone()
        {
            var newState = (ListStateElement<TEntity>)Activator.CreateInstance(this.GetType());
            
            foreach (var element in this.Entities)
            {
                newState.Entities.Add((TEntity)element.Clone());
            }

            return newState;
        }
    }
}