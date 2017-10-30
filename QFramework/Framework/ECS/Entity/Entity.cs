
/*
     Unity3D Entitas 谷歌的ECS Entity Component System入门学习1 http://blog.csdn.net/u012632851/article/details/75370836
     Unity3D Entitas 谷歌的ECS Entity Component System入门学习2 http://blog.csdn.net/u012632851/article/details/75675840
     
    sample project : https://github.com/sschmid/Entitas-Shmup
    ecs rx https://github.com/grofit 
    https://github.com/TORISOUP/UniRxExamples
    https://github.com/sschmid/Match-One
    https://github.com/sschmid/artemis_CSharp
    https://github.com/mzaks/Entitas-ReactiveUI
    https://github.com/nspec/NSpec
    http://blog.csdn.net/u012632851/article/list/3
*/

using System;
using System.Collections.Generic;
using System.Text;
using Entitas.Utils;
using QFramework;

namespace Entitas
{
    /// Use context.CreateEntity() to create a new entity and
    /// entity.Destroy() to destroy it.
    /// You can add, replace and remove IComponent to an entity.
    public class Entity : IEntity
    {
        /// Occurs when a component gets added.
        /// All event handlers will be removed when
        /// the entity gets destroyed by the context.
        public event EntityComponentChanged OnComponentAdded;

        /// Occurs when a component gets removed.
        /// All event handlers will be removed when
        /// the entity gets destroyed by the context.
        public event EntityComponentChanged OnComponentRemoved;

        /// Occurs when a component gets replaced.
        /// All event handlers will be removed when
        /// the entity gets destroyed by the context.
        public event EntityComponentReplaced OnComponentReplaced;

        /// Occurs when an entity gets released and is not retained anymore.
        /// All event handlers will be removed when
        /// the entity gets destroyed by the context.
        public event EntityEvent OnEntityReleased;

        /// Occurs when calling entity.Destroy().
        /// All event handlers will be removed when
        /// the entity gets destroyed by the context.
        public event EntityEvent OnDestroyEntity;

        /// The total amount of components an entity can possibly have.
        public int TotalComponents
        {
            get { return mTotalComponents; }
        }

        /// Each entity has its own unique creationIndex which will be set by
        /// the context when you create the entity.
        public int CreationIndex
        {
            get { return mCreationIndex; }
        }

        /// The context manages the state of an entity.
        /// Active entities are enabled, destroyed entities are not.
        public bool IsEnabled
        {
            get { return mIsEnabled; }
        }

        /// componentPools is set by the context which created the entity and
        /// is used to reuse removed components.
        /// Removed components will be pushed to the componentPool.
        /// Use entity.CreateComponent(index, type) to get a new or
        /// reusable component from the componentPool.
        /// Use entity.GetComponentPool(index) to get a componentPool for
        /// a specific component index.
        public Stack<IComponent>[] ComponentPools
        {
            get { return mComponentPools; }
        }

        /// The contextInfo is set by the context which created the entity and
        /// contains information about the context.
        /// It's used to provide better error messages.
        public ContextInfo ContextInfo
        {
            get { return mContextInfo; }
        }

        /// Automatic Entity Reference Counting (AERC)
        /// is used internally to prevent pooling retained entities.
        /// If you use retain manually you also have to
        /// release it manually at some point.
        public IRefCounter RefCounter
        {
            get { return mAERC; }
        }

        int mCreationIndex;
        bool mIsEnabled;

        int mTotalComponents;
        protected IComponent[] mComponents;
        Stack<IComponent>[] mComponentPools;
        ContextInfo mContextInfo;
        IRefCounter mAERC;

        IComponent[] mComponentsCache;
        int[] mComponentIndicesCache;
        string mToStringCache;
        StringBuilder mToStringBuilder;

        public void Initialize(int creationIndex, int totalComponents, Stack<IComponent>[] componentPools,
            ContextInfo contextInfo = null, IRefCounter iarc = null)
        {
            Reactivate(creationIndex);

            mTotalComponents = totalComponents;
            mComponents = new IComponent[totalComponents];
            mComponentPools = componentPools;

            mContextInfo = contextInfo ?? createDefaultContextInfo();
            mAERC = iarc ?? new SafeARC(this);
        }

        ContextInfo createDefaultContextInfo()
        {
            var componentNames = new string[TotalComponents];
            for (int i = 0; i < componentNames.Length; i++)
            {
                componentNames[i] = i.ToString();
            }

            return new ContextInfo("No Context", componentNames, null);
        }

        public void Reactivate(int creationIndex)
        {
            mCreationIndex = creationIndex;
            mIsEnabled = true;
        }

        /// Adds a component at the specified index.
        /// You can only have one component at an index.
        /// Each component type must have its own constant index.
        /// The prefered way is to use the
        /// generated methods from the code generator.
        public void AddComponent(int index, IComponent component)
        {
            if (!mIsEnabled)
            {
                throw new EntityIsNotEnabledException(
                    "Cannot add component '" +
                    mContextInfo.ComponentNames[index] + "' to " + this + "!"
                );
            }

            if (HasComponent(index))
            {
                throw new EntityAlreadyHasComponentException(
                    index, "Cannot add component '" +
                           mContextInfo.ComponentNames[index] + "' to " + this + "!",
                    "You should check if an entity already has the component " +
                    "before adding it or use entity.ReplaceComponent()."
                );
            }

            mComponents[index] = component;
            mComponentsCache = null;
            mComponentIndicesCache = null;
            mToStringCache = null;
            if (OnComponentAdded != null)
            {
                OnComponentAdded(this, index, component);
            }
        }

        /// Removes a component at the specified index.
        /// You can only remove a component at an index if it exists.
        /// The prefered way is to use the
        /// generated methods from the code generator.
        public void RemoveComponent(int index)
        {
            if (!mIsEnabled)
            {
                throw new EntityIsNotEnabledException(
                    "Cannot remove component '" +
                    mContextInfo.ComponentNames[index] + "' from " + this + "!"
                );
            }

            if (!HasComponent(index))
            {
                throw new EntityDoesNotHaveComponentException(
                    index, "Cannot remove component '" +
                           mContextInfo.ComponentNames[index] + "' from " + this + "!",
                    "You should check if an entity has the component " +
                    "before removing it."
                );
            }

            replaceComponent(index, null);
        }

        /// Replaces an existing component at the specified index
        /// or adds it if it doesn't exist yet.
        /// The prefered way is to use the
        /// generated methods from the code generator.
        public void ReplaceComponent(int index, IComponent component)
        {
            if (!mIsEnabled)
            {
                throw new EntityIsNotEnabledException(
                    "Cannot replace component '" +
                    mContextInfo.ComponentNames[index] + "' on " + this + "!"
                );
            }

            if (HasComponent(index))
            {
                replaceComponent(index, component);
            }
            else if (component != null)
            {
                AddComponent(index, component);
            }
        }

        void replaceComponent(int index, IComponent replacement)
        {
            mToStringCache = null;
            var previousComponent = mComponents[index];
            if (replacement != previousComponent)
            {
                mComponents[index] = replacement;
                mComponentsCache = null;
                if (replacement != null)
                {
                    if (OnComponentReplaced != null)
                    {
                        OnComponentReplaced(
                            this, index, previousComponent, replacement
                        );
                    }
                }
                else
                {
                    mComponentIndicesCache = null;
                    if (OnComponentRemoved != null)
                    {
                        OnComponentRemoved(this, index, previousComponent);
                    }
                }

                GetComponentPool(index).Push(previousComponent);

            }
            else
            {
                if (OnComponentReplaced != null)
                {
                    OnComponentReplaced(
                        this, index, previousComponent, replacement
                    );
                }
            }
        }

        /// Returns a component at the specified index.
        /// You can only get a component at an index if it exists.
        /// The prefered way is to use the
        /// generated methods from the code generator.
        public IComponent GetComponent(int index)
        {
            if (!HasComponent(index))
            {
                throw new EntityDoesNotHaveComponentException(
                    index, "Cannot get component '" +
                           mContextInfo.ComponentNames[index] + "' from " + this + "!",
                    "You should check if an entity has the component " +
                    "before getting it."
                );
            }

            return mComponents[index];
        }

        /// Returns all added components.
        public IComponent[] GetComponents()
        {
            if (mComponentsCache == null)
            {
                var components = EntitasCache.GetIComponentList();

                for (int i = 0; i < mComponents.Length; i++)
                {
                    IComponent component = mComponents[i];
                    if (component != null)
                    {
                        components.Add(component);
                    }
                }

                mComponentsCache = components.ToArray();

                EntitasCache.PushIComponentList(components);
            }

            return mComponentsCache;
        }

        /// Returns all indices of added components.
        public int[] GetComponentIndices()
        {
            if (mComponentIndicesCache == null)
            {
                var indices = EntitasCache.GetIntList();

                for (int i = 0; i < mComponents.Length; i++)
                {
                    if (mComponents[i] != null)
                    {
                        indices.Add(i);
                    }
                }

                mComponentIndicesCache = indices.ToArray();

                EntitasCache.PushIntList(indices);
            }

            return mComponentIndicesCache;
        }

        /// Determines whether this entity has a component
        /// at the specified index.
        public bool HasComponent(int index)
        {
            return mComponents[index] != null;
        }

        /// Determines whether this entity has components
        /// at all the specified indices.
        public bool HasComponents(int[] indices)
        {
            for (int i = 0; i < indices.Length; i++)
            {
                if (mComponents[indices[i]] == null)
                {
                    return false;
                }
            }

            return true;
        }

        /// Determines whether this entity has a component
        /// at any of the specified indices.
        public bool HasAnyComponent(int[] indices)
        {
            for (int i = 0; i < indices.Length; i++)
            {
                if (mComponents[indices[i]] != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// Removes all components.
        public void RemoveAllComponents()
        {
            mToStringCache = null;
            for (int i = 0; i < mComponents.Length; i++)
            {
                if (mComponents[i] != null)
                {
                    replaceComponent(i, null);
                }
            }
        }

        /// Returns the componentPool for the specified component index.
        /// componentPools is set by the context which created the entity and
        /// is used to reuse removed components.
        /// Removed components will be pushed to the componentPool.
        /// Use entity.CreateComponent(index, type) to get a new or
        /// reusable component from the componentPool.
        public Stack<IComponent> GetComponentPool(int index)
        {
            var componentPool = mComponentPools[index];
            if (componentPool == null)
            {
                componentPool = new Stack<IComponent>();
                mComponentPools[index] = componentPool;
            }

            return componentPool;
        }

        /// Returns a new or reusable component from the componentPool
        /// for the specified component index.
        public IComponent CreateComponent(int index, Type type)
        {
            var componentPool = GetComponentPool(index);
            return componentPool.Count > 0
                ? componentPool.Pop()
                : (IComponent) Activator.CreateInstance(type);
        }

        /// Returns a new or reusable component from the componentPool
        /// for the specified component index.
        public T CreateComponent<T>(int index) where T : new()
        {
            var componentPool = GetComponentPool(index);
            return componentPool.Count > 0 ? (T) componentPool.Pop() : new T();
        }

        /// Returns the number of objects that retain this entity.
        public int RefCount
        {
            get { return mAERC.RefCount; }
        }

        /// Retains the entity. An owner can only retain the same entity once.
        /// Retain/Release is part of AERC (Automatic Entity Reference Counting)
        /// and is used internally to prevent pooling retained entities.
        /// If you use retain manually you also have to
        /// release it manually at some point.
        public void Retain(object owner = null)
        {
            mAERC.Retain(owner);
            mToStringCache = null;
        }

        /// Releases the entity. An owner can only release an entity
        /// if it retains it.
        /// Retain/Release is part of AERC (Automatic Entity Reference Counting)
        /// and is used internally to prevent pooling retained entities.
        /// If you use retain manually you also have to
        /// release it manually at some point.
        public void Release(object owner = null)
        {
            mAERC.Release(owner);
            mToStringCache = null;

            if (mAERC.RefCount == 0)
            {
                if (OnEntityReleased != null)
                {
                    OnEntityReleased(this);
                }
            }
        }

        // Dispatches OnDestroyEntity which will start the destroy process.
        public void Destroy()
        {
            if (!mIsEnabled)
            {
                throw new EntityIsNotEnabledException("Cannot destroy " + this + "!");
            }

            if (OnDestroyEntity != null)
            {
                OnDestroyEntity(this);
            }
        }

        // This method is used internally. Don't call it yourself.
        // Use entity.Destroy();
        public void InternalDestroy()
        {
            mIsEnabled = false;
            RemoveAllComponents();
            OnComponentAdded = null;
            OnComponentReplaced = null;
            OnComponentRemoved = null;
            OnDestroyEntity = null;
        }

        // Do not call this method manually. This method is called by the context.
        public void RemoveAllOnEntityReleasedHandlers()
        {
            OnEntityReleased = null;
        }

        /// Returns a cached string to describe the entity
        /// with the following format:
        /// Entity_{creationIndex}(*{retainCount})({list of components})
        public override string ToString()
        {
            if (mToStringCache == null)
            {
                if (mToStringBuilder == null)
                {
                    mToStringBuilder = new StringBuilder();
                }
                mToStringBuilder.Length = 0;
                mToStringBuilder
                    .Append("Entity_")
                    .Append(mCreationIndex)
                    .Append("(*")
                    .Append(RefCount)
                    .Append(")")
                    .Append("(");

                const string separator = ", ";
                var components = GetComponents();
                var lastSeparator = components.Length - 1;
                for (int i = 0; i < components.Length; i++)
                {
                    var component = components[i];
                    var type = component.GetType();
                    var implementsToString = type.GetMethod("ToString")
                        .DeclaringType.ImplementsInterface<IComponent>();
                    mToStringBuilder.Append(
                        implementsToString
                            ? component.ToString()
                            : type.ToCompilableString().RemoveComponentSuffix()
                    );

                    if (i < lastSeparator)
                    {
                        mToStringBuilder.Append(separator);
                    }
                }

                mToStringBuilder.Append(")");
                mToStringCache = mToStringBuilder.ToString();
            }

            return mToStringCache;
        }
    }
}