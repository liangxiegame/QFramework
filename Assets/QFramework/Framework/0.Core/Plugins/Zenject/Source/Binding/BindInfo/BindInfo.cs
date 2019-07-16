using System;
using System.Collections.Generic;
using System.Diagnostics;
using Zenject.Internal;

namespace Zenject
{
    public enum ScopeTypes
    {
        Unset,
        Transient,
        Singleton
    }

    public enum ToChoices
    {
        Self,
        Concrete
    }

    public enum InvalidBindResponses
    {
        Assert,
        Skip
    }

    public enum BindingInheritanceMethods
    {
        None,
        CopyIntoAll,
        CopyDirectOnly,
        MoveIntoAll,
        MoveDirectOnly
    }

    [NoReflectionBaking]
    public class BindInfo : IDisposable
    {
        public bool MarkAsCreationBinding;
        public bool MarkAsUniqueSingleton;
        public object ConcreteIdentifier;
        public bool SaveProvider;
        public bool OnlyBindIfNotBound;
        public bool RequireExplicitScope;
        public object Identifier;
        public readonly List<Type> ContractTypes;
        public BindingInheritanceMethods BindingInheritanceMethod;
        public InvalidBindResponses InvalidBindResponse;
        public bool NonLazy;
        public BindingCondition Condition;
        public ToChoices ToChoice;
        public string ContextInfo;
        public readonly List<Type> ToTypes; // Only relevant with ToChoices.Concrete
        public ScopeTypes Scope;
        public readonly List<TypeValuePair> Arguments;
        public Action<InjectContext, object> InstantiatedCallback;

        public BindInfo()
        {
            ContractTypes = new List<Type>();
            ToTypes = new List<Type>();
            Arguments = new List<TypeValuePair>();

            Reset();
        }

        public void Dispose()
        {
            ZenPools.DespawnBindInfo(this);
        }

        [Conditional("UNITY_EDITOR")]
        public void SetContextInfo(string contextInfo)
        {
            ContextInfo = contextInfo;
        }

        public void Reset()
        {
            MarkAsCreationBinding = true;
            MarkAsUniqueSingleton = false;
            ConcreteIdentifier = null;
            SaveProvider = false;
            OnlyBindIfNotBound = false;
            RequireExplicitScope = false;
            Identifier = null;
            ContractTypes.Clear();
            BindingInheritanceMethod = BindingInheritanceMethods.None;
            InvalidBindResponse = InvalidBindResponses.Assert;
            NonLazy = false;
            Condition = null;
            ToChoice = ToChoices.Self;
            ContextInfo = null;
            ToTypes.Clear();
            Scope = ScopeTypes.Unset;
            Arguments.Clear();
            InstantiatedCallback = null;
        }
    }
}
