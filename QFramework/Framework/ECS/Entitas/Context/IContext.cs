using System;
using System.Collections.Generic;
using QFramework;

namespace Entitas
{

    public delegate void ContextEntityChanged(IContext context, IEntity entity);

    public delegate void ContextGroupChanged(IContext context, IGroup group);

    public interface IContext
    {

        event ContextEntityChanged OnEntityCreated;
        event ContextEntityChanged OnEntityWillBeDestroyed;
        event ContextEntityChanged OnEntityDestroyed;

        event ContextGroupChanged OnGroupCreated;

        int TotalComponents { get; }

        Stack<IComponent>[] ComponentPools { get; }
        ContextInfo ContextInfo { get; }

        int Count { get; }
        int ReusableEntitiesCount { get; }
        int RetainedEntitiesCount { get; }

        void DestroyAllEntities();

        void AddEntityIndex(IEntityIndex entityIndex);
        IEntityIndex GetEntityIndex(string name);

        void ResetCreationIndex();
        void ClearComponentPool(int index);
        void ClearComponentPools();
        void Reset();
    }

    public interface IContext<TEntity> : IContext where TEntity : class, IEntity
    {

        TEntity CreateEntity();

        // TODO Obsolete since 0.42.0, April 2017
        [Obsolete("Please use entity.Destroy()")]
        void DestroyEntity(TEntity entity);

        bool HasEntity(TEntity entity);
        TEntity[] GetEntities();

        IGroup<TEntity> GetGroup(IMatcher<TEntity> matcher);
    }
}
