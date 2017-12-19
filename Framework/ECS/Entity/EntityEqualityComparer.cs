using System.Collections.Generic;


namespace QFramework
{

    public class EntityEqualityComparer<TEntity> : IEqualityComparer<TEntity> where TEntity : class, IEntity
    {
        public static readonly IEqualityComparer<TEntity> Comparer = new EntityEqualityComparer<TEntity>();

        public bool Equals(TEntity x, TEntity y)
        {
            return x == y;
        }

        public int GetHashCode(TEntity obj)
        {
            return obj.CreationIndex;
        }
    }
}