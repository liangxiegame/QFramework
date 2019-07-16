using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;
using Zenject.Internal;
#if !NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    public delegate bool BindingCondition(InjectContext c);

    // Responsibilities:
    // - Expose methods to configure object graph via BindX() methods
    // - Look up bound values via Resolve() method
    // - Instantiate new values via InstantiateX() methods
    [NoReflectionBaking]
    public class DiContainer : IInstantiator
    {
        readonly Dictionary<Type, IDecoratorProvider> _decorators = new Dictionary<Type, IDecoratorProvider>();
        readonly Dictionary<BindingId, List<ProviderInfo>> _providers = new Dictionary<BindingId, List<ProviderInfo>>();

        readonly DiContainer[][] _containerLookups = new DiContainer[4][];

        readonly HashSet<LookupId> _resolvesInProgress = new HashSet<LookupId>();
        readonly HashSet<LookupId> _resolvesTwiceInProgress = new HashSet<LookupId>();

        readonly LazyInstanceInjector _lazyInjector;

        readonly SingletonMarkRegistry _singletonMarkRegistry = new SingletonMarkRegistry();
        readonly Queue<BindStatement> _currentBindings = new Queue<BindStatement>();
        readonly List<BindStatement> _childBindings = new List<BindStatement>();

        readonly HashSet<Type> _validatedTypes = new HashSet<Type>();
        readonly List<IValidatable> _validationQueue = new List<IValidatable>();

#if !NOT_UNITY3D
        Transform _contextTransform;
        bool _hasLookedUpContextTransform;
        Transform _inheritedDefaultParent;
        Transform _explicitDefaultParent;
        bool _hasExplicitDefaultParent;
#endif

        ZenjectSettings _settings;

        bool _hasResolvedRoots;
        bool _isFinalizingBinding;
        bool _isValidating;
        bool _isInstalling;
#if DEBUG || UNITY_EDITOR
        bool _hasDisplayedInstallWarning;
#endif

        public DiContainer(
            IEnumerable<DiContainer> parentContainersEnumerable, bool isValidating)
        {
            _isValidating = isValidating;

            _lazyInjector = new LazyInstanceInjector(this);

            InstallDefaultBindings();
            FlushBindings();
            Assert.That(_currentBindings.Count == 0);

            _settings = ZenjectSettings.Default;

            var selfLookup = new[] { this };
            _containerLookups[(int)InjectSources.Local] = selfLookup;

            var parentContainers = parentContainersEnumerable.ToArray();
            _containerLookups[(int)InjectSources.Parent] = parentContainers;

            var ancestorContainers = FlattenInheritanceChain().ToArray();

            _containerLookups[(int)InjectSources.AnyParent] = ancestorContainers;
            _containerLookups[(int)InjectSources.Any] = selfLookup.Concat(ancestorContainers).ToArray();

            if (!parentContainers.IsEmpty())
            {
                for (int i = 0; i < parentContainers.Length; i++)
                {
                    parentContainers[i].FlushBindings();
                }

#if !NOT_UNITY3D
                _inheritedDefaultParent = parentContainers.First().DefaultParent;
#endif

                // Make sure to avoid duplicates which could happen if a parent container
                // appears multiple times in the inheritance chain
                foreach (var ancestorContainer in ancestorContainers.Distinct())
                {
                    foreach (var binding in ancestorContainer._childBindings)
                    {
                        if (ShouldInheritBinding(binding, ancestorContainer))
                        {
                            FinalizeBinding(binding);
                        }
                    }
                }

                Assert.That(_currentBindings.Count == 0);
                Assert.That(_childBindings.Count == 0);
            }

            // Assumed to be configured in a parent container
            var settings = TryResolve<ZenjectSettings>();

            if (settings != null)
            {
                _settings = settings;
            }
        }

        public DiContainer(bool isValidating)
            : this(Enumerable.Empty<DiContainer>(), isValidating)
        {
        }

        public DiContainer()
            : this(Enumerable.Empty<DiContainer>(), false)
        {
        }

        public DiContainer(DiContainer parentContainer, bool isValidating)
            : this(new [] { parentContainer }, isValidating)
        {
        }

        public DiContainer(DiContainer parentContainer)
            : this(new [] { parentContainer }, false)
        {
        }

        public DiContainer(IEnumerable<DiContainer> parentContainers)
            : this(parentContainers, false)
        {
        }

        // By default the settings will be inherited from parent containers, but can be
        // set explicitly here as well which is useful in particular in unit tests
        // Note however that if you want child containers to use this same value you have
        // to bind it as well
        public ZenjectSettings Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
                Rebind<ZenjectSettings>().FromInstance(value);
            }
        }

        internal SingletonMarkRegistry SingletonMarkRegistry
        {
            get { return _singletonMarkRegistry; }
        }

        public IEnumerable<IProvider> AllProviders
        {
            // Distinct is necessary since the same providers can be used with multiple contracts
            get { return _providers.Values.SelectMany(x => x).Select(x => x.Provider).Distinct(); }
        }

        void InstallDefaultBindings()
        {
            Bind(typeof(DiContainer), typeof(IInstantiator)).FromInstance(this);
            Bind(typeof(LazyInject<>)).FromMethodUntyped(CreateLazyBinding).Lazy();
        }

        object CreateLazyBinding(InjectContext context)
        {
            // By cloning it this also means that Ids, optional, etc. are forwarded properly
            var newContext = context.Clone();
            newContext.MemberType = context.MemberType.GenericArguments().Single();

            var result = Activator.CreateInstance(
                typeof(LazyInject<>)
                .MakeGenericType(newContext.MemberType), this, newContext);

            if (_isValidating)
            {
                QueueForValidate((IValidatable)result);
            }

            return result;
        }

        public void QueueForValidate(IValidatable validatable)
        {
            // Don't bother adding to queue if the initial resolve is already completed
            if (!_hasResolvedRoots)
            {
                var concreteType = validatable.GetType();

                if (!_validatedTypes.Contains(concreteType))
                {
                    _validatedTypes.Add(concreteType);
                    _validationQueue.Add(validatable);
                }
            }
        }

        bool ShouldInheritBinding(BindStatement binding, DiContainer ancestorContainer)
        {
            if (binding.BindingInheritanceMethod == BindingInheritanceMethods.CopyIntoAll
                || binding.BindingInheritanceMethod == BindingInheritanceMethods.MoveIntoAll)
            {
                return true;
            }

            if ((binding.BindingInheritanceMethod == BindingInheritanceMethods.CopyDirectOnly
                    || binding.BindingInheritanceMethod == BindingInheritanceMethods.MoveDirectOnly)
                && ParentContainers.Contains(ancestorContainer))
            {
                return true;
            }

            return false;
        }

#if !NOT_UNITY3D
        // This might be null in some rare cases like when used in ZenjectUnitTestFixture
        Transform ContextTransform
        {
            get
            {
                if (!_hasLookedUpContextTransform)
                {
                    _hasLookedUpContextTransform = true;

                    var context = TryResolve<Context>();

                    if (context != null)
                    {
                        _contextTransform = context.transform;
                    }
                }

                return _contextTransform;
            }
        }
#endif

        // When true, this will throw exceptions whenever we create new game objects
        // This is helpful when used in places like EditorWindowKernel where we can't
        // assume that there is a "scene" to place objects
        public bool AssertOnNewGameObjects
        {
            get;
            set;
        }

#if !NOT_UNITY3D

        public Transform InheritedDefaultParent
        {
            get { return _inheritedDefaultParent; }
        }

        public Transform DefaultParent
        {
            get { return _explicitDefaultParent; }
            set
            {
                _explicitDefaultParent = value;
                // Need to use a flag because null is a valid explicit default parent
                _hasExplicitDefaultParent = true;
            }
        }
#endif

        public DiContainer[] ParentContainers
        {
            get { return _containerLookups[(int)InjectSources.Parent]; }
        }

        public DiContainer[] AncestorContainers
        {
            get { return _containerLookups[(int)InjectSources.AnyParent]; }
        }

        public bool ChecksForCircularDependencies
        {
            get
            {
#if ZEN_MULTITHREADING
                // When multithreading is supported we can't use a static field to track the lookup
                // TODO: We could look at the inject context though
                return false;
#else
                return true;
#endif
            }
        }

        public bool IsValidating
        {
            get { return _isValidating; }
        }

        // When this is true, it will log warnings when Resolve or Instantiate
        // methods are called
        // Used to ensure that Resolve and Instantiate methods are not called
        // during bind phase.  This is important since Resolve and Instantiate
        // make use of the bindings, so if the bindings are not complete then
        // unexpected behaviour can occur
        public bool IsInstalling
        {
            get { return _isInstalling; }
            set { _isInstalling = value; }
        }

        public IEnumerable<BindingId> AllContracts
        {
            get
            {
                FlushBindings();
                return _providers.Keys;
            }
        }

        public void ResolveRoots()
        {
            Assert.That(!_hasResolvedRoots);

            FlushBindings();

            ResolveDependencyRoots();
#if DEBUG
            if (IsValidating && _settings.ValidationRootResolveMethod == RootResolveMethods.All)
            {
                ValidateFullResolve();
            }
#endif

            _lazyInjector.LazyInjectAll();

            if (IsValidating)
            {
                FlushValidationQueue();
            }

            Assert.That(!_hasResolvedRoots);
            _hasResolvedRoots = true;
        }

        void ResolveDependencyRoots()
        {
            var rootBindings = new List<BindingId>();
            var rootProviders = new List<ProviderInfo>();

            foreach (var bindingPair in _providers)
            {
                foreach (var provider in bindingPair.Value)
                {
                    if (provider.NonLazy)
                    {
                        // Save them to a list instead of resolving for them here to account
                        // for the rare case where one of the resolves does another binding
                        // and therefore changes _providers, causing an exception.
                        rootBindings.Add(bindingPair.Key);
                        rootProviders.Add(provider);
                    }
                }
            }

            Assert.IsEqual(rootProviders.Count, rootBindings.Count);

            var instances = ZenPools.SpawnList<object>();

            try
            {
                for (int i = 0; i < rootProviders.Count; i++)
                {
                    var bindId = rootBindings[i];
                    var providerInfo = rootProviders[i];

                    using (var context = ZenPools.SpawnInjectContext(this, bindId.Type))
                    {
                        context.Identifier = bindId.Identifier;
                        context.SourceType = InjectSources.Local;

                        // Should this be true?  Are there cases where you are ok that NonLazy matches
                        // zero providers?
                        // Probably better to be false to catch mistakes
                        context.Optional = false;

                        instances.Clear();

#if ZEN_INTERNAL_PROFILING
                        using (ProfileTimers.CreateTimedBlock("DiContainer.Resolve"))
#endif
                        {
                            SafeGetInstances(providerInfo, context, instances);
                        }

                        // Zero matches might actually be valid in some cases
                        //Assert.That(matches.Any());
                    }
                }
            }
            finally
            {
                ZenPools.DespawnList(instances);
            }
        }

        void ValidateFullResolve()
        {
            Assert.That(!_hasResolvedRoots);
            Assert.That(IsValidating);

            foreach (var bindingId in _providers.Keys.ToList())
            {
                if (!bindingId.Type.IsOpenGenericType())
                {
                    using (var context = ZenPools.SpawnInjectContext(this, bindingId.Type))
                    {
                        context.Identifier = bindingId.Identifier;
                        context.SourceType = InjectSources.Local;
                        context.Optional = true;

                        ResolveAll(context);
                    }
                }
            }
        }

        void FlushValidationQueue()
        {
            Assert.That(!_hasResolvedRoots);
            Assert.That(IsValidating);

            var validatables = new List<IValidatable>();

            // Repeatedly flush the validation queue until it's empty, to account for
            // cases where calls to Validate() add more objects to the queue
            while (_validationQueue.Any())
            {
                validatables.Clear();
                validatables.AllocFreeAddRange(_validationQueue);
                _validationQueue.Clear();

                for (int i = 0; i < validatables.Count; i++)
                {
                    validatables[i].Validate();
                }
            }
        }

        public DiContainer CreateSubContainer()
        {
            return CreateSubContainer(_isValidating);
        }

        public void QueueForInject(object instance)
        {
            _lazyInjector.AddInstance(instance);
        }

        // Note: this only does anything useful during the injection phase
        // It will inject on the given instance if it hasn't already been injected, but only
        // if the given instance has been queued for inject already by calling QueueForInject
        // In some rare cases this can be useful - for example if you want to add a binding in a
        // a higher level container to a resolve inside a lower level game object context container
        // since in this case you need the game object context to be injected so you can access its
        // Container property
        public T LazyInject<T>(T instance)
        {
            _lazyInjector.LazyInject(instance);
            return instance;
        }

        DiContainer CreateSubContainer(bool isValidating)
        {
            return new DiContainer(new[] { this }, isValidating);
        }

        public void RegisterProvider(
            BindingId bindingId, BindingCondition condition, IProvider provider, bool nonLazy)
        {
            var info = new ProviderInfo(provider, condition, nonLazy, this);

            List<ProviderInfo> providerInfos;

            if (!_providers.TryGetValue(bindingId, out providerInfos))
            {
                providerInfos = new List<ProviderInfo>();
                _providers.Add(bindingId, providerInfos);
            }

            providerInfos.Add(info);
        }

        void GetProviderMatches(
            InjectContext context, List<ProviderInfo> buffer)
        {
            Assert.IsNotNull(context);
            Assert.That(buffer.Count == 0);

            var allMatches = ZenPools.SpawnList<ProviderInfo>();

            try
            {
                GetProvidersForContract(
                    context.BindingId, context.SourceType, allMatches);

                for (int i = 0; i < allMatches.Count; i++)
                {
                    var match = allMatches[i];

                    if (match.Condition == null || match.Condition(context))
                    {
                        buffer.Add(match);
                    }
                }
            }
            finally
            {
                ZenPools.DespawnList(allMatches);
            }
        }

        ProviderInfo TryGetUniqueProvider(InjectContext context)
        {
            Assert.IsNotNull(context);

            var bindingId = context.BindingId;
            var sourceType = context.SourceType;

            var containerLookups = _containerLookups[(int)sourceType];

            for (int i = 0; i < containerLookups.Length; i++)
            {
                containerLookups[i].FlushBindings();
            }

            var localProviders = ZenPools.SpawnList<ProviderInfo>();

            try
            {
                ProviderInfo selected = null;
                int selectedDistance = Int32.MaxValue;
                bool selectedHasCondition = false;
                bool ambiguousSelection = false;

                for (int i = 0; i < containerLookups.Length; i++)
                {
                    var container = containerLookups[i];

                    int curDistance = GetContainerHeirarchyDistance(container);

                    if (curDistance > selectedDistance)
                    {
                        // If matching provider was already found lower in the hierarchy => don't search for a new one,
                        // because there can't be a better or equal provider in this container.
                        continue;
                    }

                    localProviders.Clear();
                    container.GetLocalProviders(bindingId, localProviders);

                    for (int k = 0; k < localProviders.Count; k++)
                    {
                        var provider = localProviders[k];

                        bool curHasCondition = provider.Condition != null;

                        if (curHasCondition && !provider.Condition(context))
                        {
                            // The condition is not satisfied.
                            continue;
                        }

                        // The distance can't decrease becuase we are iterating over the containers with increasing distance.
                        // The distance can't increase because  we skip the container if the distance is greater than selected.
                        // So the distances are equal and only the condition can help resolving the amiguity.
                        Assert.That(selected == null || selectedDistance == curDistance);

                        if (curHasCondition)
                        {
                            if (selectedHasCondition)
                            {
                                // Both providers have condition and are on equal depth.
                                ambiguousSelection = true;
                            }
                            else
                            {
                                // Ambiguity is resolved because a provider with condition was found.
                                ambiguousSelection = false;
                            }
                        }
                        else
                        {
                            if (selectedHasCondition)
                            {
                                // Selected provider is better because it has condition.
                                continue;
                            }
                            if (selected != null)
                            {
                                // Both providers don't have a condition and are on equal depth.
                                ambiguousSelection = true;
                            }
                        }

                        if (ambiguousSelection)
                        {
                            continue;
                        }

                        selectedDistance = curDistance;
                        selectedHasCondition = curHasCondition;
                        selected = provider;
                    }
                }

                if (ambiguousSelection)
                {
                    throw Assert.CreateException(
                        "Found multiple matches when only one was expected for type '{0}'{1}. Object graph:\n {2}",
                        context.MemberType,
                        (context.ObjectType == null
                            ? ""
                            : " while building object with type '{0}'".Fmt(context.ObjectType)),
                        context.GetObjectGraphString());
                }

                return selected;
            }
            finally
            {
                ZenPools.DespawnList(localProviders);
            }
        }

        // Get the full list of ancestor Di Containers, making sure to avoid
        // duplicates and also order them in a breadth-first way
        List<DiContainer> FlattenInheritanceChain()
        {
            var processed = new List<DiContainer>();

            var containerQueue = new Queue<DiContainer>();
            containerQueue.Enqueue(this);

            while (containerQueue.Count > 0)
            {
                var current = containerQueue.Dequeue();

                foreach (var parent in current.ParentContainers)
                {
                    if (!processed.Contains(parent))
                    {
                        processed.Add(parent);
                        containerQueue.Enqueue(parent);
                    }
                }
            }

            return processed;
        }

        void GetLocalProviders(BindingId bindingId, List<ProviderInfo> buffer)
        {
            List<ProviderInfo> localProviders;

            if (_providers.TryGetValue(bindingId, out localProviders))
            {
                buffer.AllocFreeAddRange(localProviders);
                return;
            }

            // If we are asking for a List<int>, we should also match for any localProviders that are bound to the open generic type List<>
            // Currently it only matches one and not the other - not totally sure if this is better than returning both
            if (bindingId.Type.IsGenericType() && _providers.TryGetValue(new BindingId(bindingId.Type.GetGenericTypeDefinition(), bindingId.Identifier), out localProviders))
            {
                buffer.AllocFreeAddRange(localProviders);
            }

            // None found
        }

        void GetProvidersForContract(
            BindingId bindingId, InjectSources sourceType, List<ProviderInfo> buffer)
        {
            var containerLookups = _containerLookups[(int)sourceType];

            for (int i = 0; i < containerLookups.Length; i++)
            {
                containerLookups[i].FlushBindings();
            }

            for (int i = 0; i < containerLookups.Length; i++)
            {
                containerLookups[i].GetLocalProviders(bindingId, buffer);
            }
        }

        public void Install<TInstaller>()
            where TInstaller : Installer
        {
            Instantiate<TInstaller>().InstallBindings();
        }

        // Note: You might want to use Installer<> as your base class instead to allow
        // for strongly typed parameters
        public void Install<TInstaller>(object[] extraArgs)
            where TInstaller : Installer
        {
            Instantiate<TInstaller>(extraArgs).InstallBindings();
        }

        public IList ResolveAll(InjectContext context)
        {
            var buffer = ZenPools.SpawnList<object>();

            try
            {
                ResolveAll(context, buffer);
                return ReflectionUtil.CreateGenericList(context.MemberType, buffer);
            }
            finally
            {
                ZenPools.DespawnList(buffer);
            }
        }

        public void ResolveAll(InjectContext context, List<object> buffer)
        {
#if ZEN_INTERNAL_PROFILING
            using (ProfileTimers.CreateTimedBlock("DiContainer.Resolve"))
#endif
            {
                Assert.IsNotNull(context);
                // Note that different types can map to the same provider (eg. a base type to a concrete class and a concrete class to itself)

                FlushBindings();
                CheckForInstallWarning(context);

                var matches = ZenPools.SpawnList<ProviderInfo>();

                try
                {
                    GetProviderMatches(context, matches);

                    if (matches.Count == 0)
                    {
                        if (!context.Optional)
                        {
                            throw Assert.CreateException(
                                "Could not find required dependency with type '{0}' Object graph:\n {1}", context.MemberType, context.GetObjectGraphString());
                        }

                        return;
                    }

                    var instances = ZenPools.SpawnList<object>();
                    var allInstances = ZenPools.SpawnList<object>();

                    try
                    {
                        for (int i = 0; i < matches.Count; i++)
                        {
                            var match = matches[i];

                            instances.Clear();
                            SafeGetInstances(match, context, instances);

                            for (int k = 0; k < instances.Count; k++)
                            {
                                allInstances.Add(instances[k]);
                            }
                        }

                        if (allInstances.Count == 0 && !context.Optional)
                        {
                            throw Assert.CreateException(
                                "Could not find required dependency with type '{0}'.  Found providers but they returned zero results!", context.MemberType);
                        }

                        if (IsValidating)
                        {
                            for (int i = 0; i < allInstances.Count; i++)
                            {
                                var instance = allInstances[i];

                                if (instance is ValidationMarker)
                                {
                                    allInstances[i] = context.MemberType.GetDefaultValue();
                                }
                            }
                        }

                        buffer.AllocFreeAddRange(allInstances);
                    }
                    finally
                    {
                        ZenPools.DespawnList(instances);
                        ZenPools.DespawnList(allInstances);
                    }
                }
                finally
                {
                    ZenPools.DespawnList(matches);
                }
            }
        }

        void CheckForInstallWarning(InjectContext context)
        {
            if (!_settings.DisplayWarningWhenResolvingDuringInstall)
            {
                return;
            }

            Assert.IsNotNull(context);

#if DEBUG || UNITY_EDITOR
            if (!_isInstalling)
            {
                return;
            }

            if (_hasDisplayedInstallWarning)
            {
                return;
            }

            if (context == null)
            {
                // No way to tell whether this is ok or not so just assume ok
                return;
            }

#if UNITY_EDITOR
            if (context.MemberType.DerivesFrom<Context>())
            {
                // This happens when getting default transform parent so ok
                return;
            }
#endif
            if (IsValidating && TypeAnalyzer.ShouldAllowDuringValidation(context.MemberType))
            {
                return;
            }

            var rootContext = context.ParentContextsAndSelf.Last();

            if (rootContext.MemberType.DerivesFrom<IInstaller>())
            {
                // Resolving/instantiating/injecting installers is valid during install phase
                return;
            }

            _hasDisplayedInstallWarning = true;

            // Feel free to comment this out if you are comfortable with this practice
            Log.Warn("Zenject Warning: It is bad practice to call Inject/Resolve/Instantiate before all the Installers have completed!  This is important to ensure that all bindings have properly been installed in case they are needed when injecting/instantiating/resolving.  Detected when operating on type '{0}'.  If you don't care about this, you can disable this warning by setting flag 'ZenjectSettings.DisplayWarningWhenResolvingDuringInstall' to false (see docs for details on ZenjectSettings).", rootContext.MemberType);
#endif
        }

        // Returns the concrete type that would be returned with Resolve<T>
        // without actually instantiating it
        // This is safe to use within installers
        public Type ResolveType<T>()
        {
            return ResolveType(typeof(T));
        }

        // Returns the concrete type that would be returned with Resolve(type)
        // without actually instantiating it
        // This is safe to use within installers
        public Type ResolveType(Type type)
        {
            using (var context = ZenPools.SpawnInjectContext(this, type))
            {
                return ResolveType(context);
            }
        }

        // Returns the concrete type that would be returned with Resolve(context)
        // without actually instantiating it
        // This is safe to use within installers
        public Type ResolveType(InjectContext context)
        {
            Assert.IsNotNull(context);

            FlushBindings();

            var providerInfo = TryGetUniqueProvider(context);

            if (providerInfo == null)
            {
                throw Assert.CreateException(
                    "Unable to resolve {0}{1}. Object graph:\n{2}", context.BindingId,
                    (context.ObjectType == null ? "" : " while building object with type '{0}'".Fmt(context.ObjectType)),
                    context.GetObjectGraphString());
            }

            return providerInfo.Provider.GetInstanceType(context);
        }

        public List<Type> ResolveTypeAll(Type type)
        {
            return ResolveTypeAll(type, null);
        }

        public List<Type> ResolveTypeAll(Type type, object identifier)
        {
            using (var context = ZenPools.SpawnInjectContext(this, type))
            {
                context.Identifier = identifier;
                return ResolveTypeAll(context);
            }
        }

        // Returns all the types that would be returned if ResolveAll was called with the given values
        public List<Type> ResolveTypeAll(InjectContext context)
        {
            Assert.IsNotNull(context);

            FlushBindings();

            var matches = ZenPools.SpawnList<ProviderInfo>();

            try
            {
                GetProviderMatches(context, matches);

                if (matches.Count > 0 )
                {
                    return matches.Select(
                        x => x.Provider.GetInstanceType(context))
                        .Where(x => x != null).ToList();
                }

                return new List<Type>();
            }
            finally
            {
                ZenPools.DespawnList(matches);
            }
        }

        public object Resolve(BindingId id)
        {
            using (var context = ZenPools.SpawnInjectContext(this, id.Type))
            {
                context.Identifier = id.Identifier;
                return Resolve(context);
            }
        }

        public object Resolve(InjectContext context)
        {
#if ZEN_INTERNAL_PROFILING
            using (ProfileTimers.CreateTimedBlock("DiContainer.Resolve"))
#endif
            {
                // Note: context.Container is not necessarily equal to this, since
                // you can have some lookups recurse to parent containers
                Assert.IsNotNull(context);

                var memberType = context.MemberType;

                FlushBindings();
                CheckForInstallWarning(context);

                var lookupContext = context;

                // The context used for lookups is always the same as the given context EXCEPT for LazyInject<>
                // In CreateLazyBinding above, we forward the context to a new instance of LazyInject<>
                // The problem is, we want the binding for Bind(typeof(LazyInject<>)) to always match even
                // for members that are marked for a specific ID, so we need to discard the identifier
                // for this one particular case
                if (memberType.IsGenericType() && memberType.GetGenericTypeDefinition() == typeof(LazyInject<>))
                {
                    lookupContext = context.Clone();
                    lookupContext.Identifier = null;
                    lookupContext.SourceType = InjectSources.Local;
                    lookupContext.Optional = false;
                }

                var providerInfo = TryGetUniqueProvider(lookupContext);

                if (providerInfo == null)
                {
                    // If it's an array try matching to multiple values using its array type
                    if (memberType.IsArray && memberType.GetArrayRank() == 1)
                    {
                        var subType = memberType.GetElementType();

                        var subContext = context.Clone();
                        subContext.MemberType = subType;
                        // By making this optional this means that all injected fields of type T[]
                        // will pass validation, which could be error prone, but I think this is better
                        // than always requiring that they explicitly mark their array types as optional
                        subContext.Optional = true;

                        var results = ZenPools.SpawnList<object>();

                        try
                        {
                            ResolveAll(subContext, results);
                            return ReflectionUtil.CreateArray(subContext.MemberType, results);
                        }
                        finally
                        {
                            ZenPools.DespawnList(results);
                        }
                    }

                    // If it's a generic list then try matching multiple instances to its generic type
                    if (memberType.IsGenericType()
                        && (memberType.GetGenericTypeDefinition() == typeof(List<>)
                            || memberType.GetGenericTypeDefinition() == typeof(IList<>)
#if NET_4_6
                            || memberType.GetGenericTypeDefinition() == typeof(IReadOnlyList<>)
#endif
                            || memberType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                    {
                        var subType = memberType.GenericArguments().Single();

                        var subContext = context.Clone();
                        subContext.MemberType = subType;
                        // By making this optional this means that all injected fields of type List<>
                        // will pass validation, which could be error prone, but I think this is better
                        // than always requiring that they explicitly mark their list types as optional
                        subContext.Optional = true;

                        return ResolveAll(subContext);
                    }

                    if (context.Optional)
                    {
                        return context.FallBackValue;
                    }

                    throw Assert.CreateException("Unable to resolve '{0}'{1}. Object graph:\n{2}", context.BindingId,
                        (context.ObjectType == null ? "" : " while building object with type '{0}'".Fmt(context.ObjectType)),
                        context.GetObjectGraphString());
                }

                var instances = ZenPools.SpawnList<object>();

                try
                {
                    SafeGetInstances(providerInfo, context, instances);

                    if (instances.Count == 0)
                    {
                        if (context.Optional)
                        {
                            return context.FallBackValue;
                        }

                        throw Assert.CreateException(
                            "Unable to resolve '{0}'{1}. Object graph:\n{2}", context.BindingId,
                            (context.ObjectType == null
                             ? ""
                             : " while building object with type '{0}'".Fmt(context.ObjectType)),
                             context.GetObjectGraphString());
                    }

                    if (instances.Count() > 1)
                    {
                        throw Assert.CreateException(
                            "Provider returned multiple instances when only one was expected!  While resolving '{0}'{1}. Object graph:\n{2}", context.BindingId,
                            (context.ObjectType == null
                             ? ""
                             : " while building object with type '{0}'".Fmt(context.ObjectType)),
                             context.GetObjectGraphString());
                    }

                    return instances.First();
                }
                finally
                {
                    ZenPools.DespawnList(instances);
                }
            }
        }

        void SafeGetInstances(ProviderInfo providerInfo, InjectContext context, List<object> instances)
        {
            Assert.IsNotNull(context);

            var provider = providerInfo.Provider;

            if (ChecksForCircularDependencies)
            {
                var lookupId = ZenPools.SpawnLookupId(provider, context.BindingId);

                try
                {
                    // Use the container associated with the provider to address some rare cases
                    // which would otherwise result in an infinite loop.  Like this:
                    // Container.Bind<ICharacter>().FromComponentInNewPrefab(Prefab).AsTransient()
                    // With the prefab being a GameObjectContext containing a script that has a
                    // ICharacter dependency.  In this case, we would otherwise use the _resolvesInProgress
                    // associated with the GameObjectContext container, which will allow the recursive
                    // lookup, which will trigger another GameObjectContext and container (since it is
                    // transient) and the process continues indefinitely
                    var providerContainer = providerInfo.Container;

                    if (providerContainer._resolvesTwiceInProgress.Contains(lookupId))
                    {
                        // Allow one before giving up so that you can do circular dependencies via postinject or fields
                        throw Assert.CreateException(
                            "Circular dependency detected! Object graph:\n {0}", context.GetObjectGraphString());
                    }

                    bool twice = false;
                    if (!providerContainer._resolvesInProgress.Add(lookupId))
                    {
                        bool added = providerContainer._resolvesTwiceInProgress.Add(lookupId);
                        Assert.That(added);
                        twice = true;
                    }

                    try
                    {
                        GetDecoratedInstances(provider, context, instances);
                    }
                    finally
                    {
                        if (twice)
                        {
                            bool removed = providerContainer._resolvesTwiceInProgress.Remove(lookupId);
                            Assert.That(removed);
                        }
                        else
                        {
                            bool removed = providerContainer._resolvesInProgress.Remove(lookupId);
                            Assert.That(removed);
                        }
                    }
                }
                finally
                {
                    ZenPools.DespawnLookupId(lookupId);
                }
            }
            else
            {
                GetDecoratedInstances(provider, context, instances);
            }
        }

        public DecoratorToChoiceFromBinder<TContract> Decorate<TContract>()
        {
            var bindStatement = StartBinding();
            var bindInfo = bindStatement.SpawnBindInfo();

            bindInfo.ContractTypes.Add(typeof(IFactory<TContract, TContract>));

            var factoryBindInfo = new FactoryBindInfo(
                typeof(PlaceholderFactory<TContract, TContract>));

            bindStatement.SetFinalizer(
                new PlaceholderFactoryBindingFinalizer<TContract>(
                    bindInfo, factoryBindInfo));

            var bindId = Guid.NewGuid();

            bindInfo.Identifier = bindId;

            IDecoratorProvider decoratorProvider;

            if (!_decorators.TryGetValue(typeof(TContract), out decoratorProvider))
            {
                decoratorProvider = new DecoratorProvider<TContract>(this);
                _decorators.Add(typeof(TContract), decoratorProvider);
            }

            ((DecoratorProvider<TContract>)decoratorProvider).AddFactoryId(bindId);

            return new DecoratorToChoiceFromBinder<TContract>(
                this, bindInfo, factoryBindInfo);
        }

        void GetDecoratedInstances(
            IProvider provider, InjectContext context, List<object> buffer)
        {
            // TODO:  This is flawed since it doesn't allow binding new decorators in subcontainers
            var decoratorProvider = TryGetDecoratorProvider(context.BindingId.Type);

            if (decoratorProvider != null)
            {
                decoratorProvider.GetAllInstances(provider, context, buffer);
                return;
            }

            provider.GetAllInstances(context, buffer);
        }

        IDecoratorProvider TryGetDecoratorProvider(Type contractType)
        {
            IDecoratorProvider decoratorProvider;

            if (_decorators.TryGetValue(contractType, out decoratorProvider))
            {
                return decoratorProvider;
            }

            var ancestorContainers = AncestorContainers;

            for (int i = 0; i < ancestorContainers.Length; i++)
            {
                if (ancestorContainers[i]._decorators.TryGetValue(contractType, out decoratorProvider))
                {
                    return decoratorProvider;
                }
            }

            return null;
        }

        int GetContainerHeirarchyDistance(DiContainer container)
        {
            return GetContainerHeirarchyDistance(container, 0).Value;
        }

        int? GetContainerHeirarchyDistance(DiContainer container, int depth)
        {
            if (container == this)
            {
                return depth;
            }

            int? result = null;

            var parentContainers = ParentContainers;

            for (int i = 0; i < parentContainers.Length; i++)
            {
                var parent = parentContainers[i];

                var distance = parent.GetContainerHeirarchyDistance(container, depth + 1);

                if (distance.HasValue && (!result.HasValue || distance.Value < result.Value))
                {
                    result = distance;
                }
            }

            return result;
        }

        public IEnumerable<Type> GetDependencyContracts<TContract>()
        {
            return GetDependencyContracts(typeof(TContract));
        }

        public IEnumerable<Type> GetDependencyContracts(Type contract)
        {
            FlushBindings();

            var info = TypeAnalyzer.TryGetInfo(contract);

            if (info != null)
            {
                foreach (var injectMember in info.AllInjectables)
                {
                    yield return injectMember.MemberType;
                }
            }
        }

        object InstantiateInternal(
            Type concreteType, bool autoInject, List<TypeValuePair> extraArgs, InjectContext context, object concreteIdentifier)
        {
#if !NOT_UNITY3D
            Assert.That(!concreteType.DerivesFrom<Component>(),
                "Error occurred while instantiating object of type '{0}'. Instantiator should not be used to create new mono behaviours.  Must use InstantiatePrefabForComponent, InstantiatePrefab, or InstantiateComponent.", concreteType);
#endif

            Assert.That(!concreteType.IsAbstract(), "Expected type '{0}' to be non-abstract", concreteType);

            FlushBindings();
            CheckForInstallWarning(context);

            var typeInfo = TypeAnalyzer.TryGetInfo(concreteType);

            Assert.IsNotNull(typeInfo, "Tried to create type '{0}' but could not find type information", concreteType);

            bool allowDuringValidation = IsValidating && TypeAnalyzer.ShouldAllowDuringValidation(concreteType);

            object newObj;

#if !NOT_UNITY3D
            if (concreteType.DerivesFrom<ScriptableObject>())
            {
                Assert.That(typeInfo.InjectConstructor.Parameters.Length == 0,
                    "Found constructor parameters on ScriptableObject type '{0}'.  This is not allowed.  Use an [Inject] method or fields instead.");

                if (!IsValidating || allowDuringValidation)
                {
                    newObj = ScriptableObject.CreateInstance(concreteType);
                }
                else
                {
                    newObj = new ValidationMarker(concreteType);
                }
            }
            else
#endif
            {
                Assert.IsNotNull(typeInfo.InjectConstructor.Factory,
                    "More than one (or zero) constructors found for type '{0}' when creating dependencies.  Use one [Inject] attribute to specify which to use.", concreteType);

                // Make a copy since we remove from it below
                var paramValues = ZenPools.SpawnArray<object>(typeInfo.InjectConstructor.Parameters.Length);

                try
                {
                    for (int i = 0; i < typeInfo.InjectConstructor.Parameters.Length; i++)
                    {
                        var injectInfo = typeInfo.InjectConstructor.Parameters[i];

                        object value;

                        if (!InjectUtil.PopValueWithType(
                            extraArgs, injectInfo.MemberType, out value))
                        {
                            using (var subContext = ZenPools.SpawnInjectContext(
                                this, injectInfo, context, null, concreteType, concreteIdentifier))
                            {
                                value = Resolve(subContext);
                            }
                        }

                        if (value == null || value is ValidationMarker)
                        {
                            paramValues[i] = injectInfo.MemberType.GetDefaultValue();
                        }
                        else
                        {
                            paramValues[i] = value;
                        }
                    }

                    if (!IsValidating || allowDuringValidation)
                    {
                        //ModestTree.Log.Debug("Zenject: Instantiating type '{0}'", concreteType);
                        try
                        {
#if ZEN_INTERNAL_PROFILING
                            using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
#if UNITY_EDITOR
                            using (ProfileBlock.Start("{0}.{1}()", concreteType, concreteType.Name))
#endif
                            {
                                newObj = typeInfo.InjectConstructor.Factory(paramValues);
                            }
                        }
                        catch (Exception e)
                        {
                            throw Assert.CreateException(
                                e, "Error occurred while instantiating object with type '{0}'", concreteType);
                        }
                    }
                    else
                    {
                        newObj = new ValidationMarker(concreteType);
                    }
                }
                finally
                {
                    ZenPools.DespawnArray(paramValues);
                }
            }

            if (autoInject)
            {
                InjectExplicit(newObj, concreteType, extraArgs, context, concreteIdentifier);

                if (extraArgs.Count > 0 && !(newObj is ValidationMarker))
                {
                    throw Assert.CreateException(
                        "Passed unnecessary parameters when injecting into type '{0}'. \nExtra Parameters: {1}\nObject graph:\n{2}",
                        newObj.GetType(), String.Join(",", extraArgs.Select(x => x.Type.PrettyName()).ToArray()), context.GetObjectGraphString());
                }
            }

#if DEBUG
            if (IsValidating && newObj is IValidatable)
            {
                QueueForValidate((IValidatable)newObj);
            }
#endif

            return newObj;
        }

        // InjectExplicit is only necessary when you want to inject null values into your object
        // otherwise you can just use Inject()
        // Note: Any arguments that are used will be removed from extraArgMap
        public void InjectExplicit(object injectable, List<TypeValuePair> extraArgs)
        {
            Type injectableType;

            if (injectable is ValidationMarker)
            {
                injectableType = ((ValidationMarker)injectable).MarkedType;
            }
            else
            {
                injectableType = injectable.GetType();
            }

            InjectExplicit(
                injectable,
                injectableType,
                extraArgs,
                new InjectContext(this, injectableType, null),
                null);
        }

        public void InjectExplicit(
            object injectable, Type injectableType,
            List<TypeValuePair> extraArgs, InjectContext context, object concreteIdentifier)
        {
#if ZEN_INTERNAL_PROFILING
            using (ProfileTimers.CreateTimedBlock("DiContainer.Inject"))
#endif
            {
                if (IsValidating)
                {
                    var marker = injectable as ValidationMarker;

                    if (marker != null && marker.InstantiateFailed)
                    {
                        // Do nothing in this case because it already failed and so there
                        // could be many knock-on errors that aren't related to the user
                        return;
                    }

                    if (_settings.ValidationErrorResponse == ValidationErrorResponses.Throw)
                    {
                        InjectExplicitInternal(
                            injectable, injectableType, extraArgs, context, concreteIdentifier);
                    }
                    else
                    {
                        // In this case, just log it and continue to print out multiple validation errors
                        // at once
                        try
                        {
                            InjectExplicitInternal(injectable, injectableType, extraArgs, context, concreteIdentifier);
                        }
                        catch (Exception e)
                        {
                            Log.ErrorException(e);
                        }
                    }
                }
                else
                {
                    InjectExplicitInternal(injectable, injectableType, extraArgs, context, concreteIdentifier);
                }
            }
        }

        void CallInjectMethodsTopDown(
            object injectable, Type injectableType,
            InjectTypeInfo typeInfo, List<TypeValuePair> extraArgs,
            InjectContext context, object concreteIdentifier, bool isDryRun)
        {
            if (typeInfo.BaseTypeInfo != null)
            {
                CallInjectMethodsTopDown(
                    injectable, injectableType, typeInfo.BaseTypeInfo, extraArgs,
                    context, concreteIdentifier, isDryRun);
            }

            for (int i = 0; i < typeInfo.InjectMethods.Length; i++)
            {
                var method = typeInfo.InjectMethods[i];
                var paramValues = ZenPools.SpawnArray<object>(method.Parameters.Length);

                try
                {
                    for (int k = 0; k < method.Parameters.Length; k++)
                    {
                        var injectInfo = method.Parameters[k];

                        object value;

                        if (!InjectUtil.PopValueWithType(extraArgs, injectInfo.MemberType, out value))
                        {
                            using (var subContext = ZenPools.SpawnInjectContext(
                                this, injectInfo, context, injectable, injectableType, concreteIdentifier))
                            {
                                value = Resolve(subContext);
                            }
                        }

                        if (value is ValidationMarker)
                        {
                            Assert.That(IsValidating);

                            paramValues[k] = injectInfo.MemberType.GetDefaultValue();
                        }
                        else
                        {
                            paramValues[k] = value;
                        }
                    }

                    if (!isDryRun)
                    {
#if ZEN_INTERNAL_PROFILING
                        using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
#if UNITY_EDITOR
                        using (ProfileBlock.Start("{0}.{1}()", typeInfo.Type, method.Name))
#endif
                        {
                            method.Action(injectable, paramValues);
                        }
                    }
                }
                finally
                {
                    ZenPools.DespawnArray(paramValues);
                }
            }
        }

        void InjectMembersTopDown(
            object injectable, Type injectableType,
            InjectTypeInfo typeInfo, List<TypeValuePair> extraArgs,
            InjectContext context, object concreteIdentifier, bool isDryRun)
        {
            if (typeInfo.BaseTypeInfo != null)
            {
                InjectMembersTopDown(
                    injectable, injectableType, typeInfo.BaseTypeInfo, extraArgs,
                    context, concreteIdentifier, isDryRun);
            }

            for (int i = 0; i < typeInfo.InjectMembers.Length; i++)
            {
                var injectInfo = typeInfo.InjectMembers[i].Info;
                var setterMethod = typeInfo.InjectMembers[i].Setter;

                object value;

                if (InjectUtil.PopValueWithType(extraArgs, injectInfo.MemberType, out value))
                {
                    if (!isDryRun)
                    {
                        if (value is ValidationMarker)
                        {
                            Assert.That(IsValidating);
                        }
                        else
                        {
                            setterMethod(injectable, value);
                        }
                    }
                }
                else
                {
                    using (var subContext = ZenPools.SpawnInjectContext(
                        this, injectInfo, context, injectable, injectableType, concreteIdentifier))
                    {
                        value = Resolve(subContext);
                    }

                    if (injectInfo.Optional && value == null)
                    {
                        // Do not override in this case so it retains the hard-coded value
                    }
                    else
                    {
                        if (!isDryRun)
                        {
                            if (value is ValidationMarker)
                            {
                                Assert.That(IsValidating);
                            }
                            else
                            {
                                setterMethod(injectable, value);
                            }
                        }
                    }
                }
            }
        }

        void InjectExplicitInternal(
            object injectable, Type injectableType, List<TypeValuePair> extraArgs,
            InjectContext context, object concreteIdentifier)
        {
            Assert.That(injectable != null);

            var typeInfo = TypeAnalyzer.TryGetInfo(injectableType);

            if (typeInfo == null)
            {
                Assert.That(extraArgs.IsEmpty());
                return;
            }

            var allowDuringValidation = IsValidating && TypeAnalyzer.ShouldAllowDuringValidation(injectableType);

            // Installers are the only things that we instantiate/inject on during validation
            var isDryRun = IsValidating && !allowDuringValidation;

            if (!isDryRun)
            {
                Assert.IsEqual(injectable.GetType(), injectableType);
            }

#if !NOT_UNITY3D
            if (injectableType == typeof(GameObject))
            {
                Assert.CreateException(
                    "Use InjectGameObject to Inject game objects instead of Inject method. Object graph: {0}", context.GetObjectGraphString());
            }
#endif

            FlushBindings();
            CheckForInstallWarning(context);

            InjectMembersTopDown(
                injectable, injectableType, typeInfo, extraArgs, context, concreteIdentifier, isDryRun);

            CallInjectMethodsTopDown(
                injectable, injectableType, typeInfo, extraArgs, context, concreteIdentifier, isDryRun);

            if (extraArgs.Count > 0)
            {
                throw Assert.CreateException(
                    "Passed unnecessary parameters when injecting into type '{0}'. \nExtra Parameters: {1}\nObject graph:\n{2}",
                    injectableType, String.Join(",", extraArgs.Select(x => x.Type.PrettyName()).ToArray()), context.GetObjectGraphString());
            }
        }

#if !NOT_UNITY3D

        // Don't use this unless you know what you're doing
        // You probably want to use InstantiatePrefab instead
        // This one will only create the prefab and will not inject into it
        // Also, this will always return the new game object as disabled, so that injection can occur before Awake / OnEnable / Start
        internal GameObject CreateAndParentPrefabResource(
            string resourcePath, GameObjectCreationParameters gameObjectBindInfo, InjectContext context, out bool shouldMakeActive)
        {
            var prefab = (GameObject)Resources.Load(resourcePath);

            Assert.IsNotNull(prefab,
                "Could not find prefab at resource location '{0}'".Fmt(resourcePath));

            return CreateAndParentPrefab(prefab, gameObjectBindInfo, context, out shouldMakeActive);
        }

        GameObject GetPrefabAsGameObject(UnityEngine.Object prefab)
        {
            if (prefab is GameObject)
            {
                return (GameObject)prefab;
            }

            Assert.That(prefab is Component, "Invalid type given for prefab. Given object name: '{0}'", prefab.name);
            return ((Component)prefab).gameObject;
        }

        // Don't use this unless you know what you're doing
        // You probably want to use InstantiatePrefab instead
        // This one will only create the prefab and will not inject into it
        internal GameObject CreateAndParentPrefab(
            UnityEngine.Object prefab, GameObjectCreationParameters gameObjectBindInfo,
            InjectContext context, out bool shouldMakeActive)
        {
            Assert.That(prefab != null, "Null prefab found when instantiating game object");

            Assert.That(!AssertOnNewGameObjects,
                "Given DiContainer does not support creating new game objects");

            FlushBindings();

            var prefabAsGameObject = GetPrefabAsGameObject(prefab);

            var prefabWasActive = prefabAsGameObject.activeSelf;

            shouldMakeActive = prefabWasActive;

            var parent = GetTransformGroup(gameObjectBindInfo, context);

            Transform initialParent;
#if !UNITY_EDITOR
            if (prefabWasActive)
            {
                prefabAsGameObject.SetActive(false);
            }
#else
            if (prefabWasActive)
            {
                initialParent = ZenUtilInternal.GetOrCreateInactivePrefabParent();
            }
            else
#endif
            {
                if (parent != null)
                {
                    initialParent = parent;
                }
                else
                {
                    // This ensures it gets added to the right scene instead of just the active scene
                    initialParent = ContextTransform;
                }
            }

            bool positionAndRotationWereSet;
            GameObject gameObj;

#if ZEN_INTERNAL_PROFILING
            using (ProfileTimers.CreateTimedBlock("GameObject.Instantiate"))
#endif
            {
                if (gameObjectBindInfo.Position.HasValue && gameObjectBindInfo.Rotation.HasValue)
                {
                    gameObj = GameObject.Instantiate(
                        prefabAsGameObject, gameObjectBindInfo.Position.Value, gameObjectBindInfo.Rotation.Value, initialParent);
                    positionAndRotationWereSet = true;
                }
                else if (gameObjectBindInfo.Position.HasValue)
                {
                    gameObj = GameObject.Instantiate(
                        prefabAsGameObject, gameObjectBindInfo.Position.Value, prefabAsGameObject.transform.rotation, initialParent);
                    positionAndRotationWereSet = true;
                }
                else if (gameObjectBindInfo.Rotation.HasValue)
                {
                    gameObj = GameObject.Instantiate(
                        prefabAsGameObject, prefabAsGameObject.transform.position, gameObjectBindInfo.Rotation.Value, initialParent);
                    positionAndRotationWereSet = true;
                }
                else
                {
                    gameObj = GameObject.Instantiate(prefabAsGameObject, initialParent);
                    positionAndRotationWereSet = false;
                }
            }

#if !UNITY_EDITOR
            if (prefabWasActive)
            {
                prefabAsGameObject.SetActive(true);
            }
#else
            if (prefabWasActive)
            {
                gameObj.SetActive(false);

                if (parent == null)
                {
                    gameObj.transform.SetParent(ContextTransform, positionAndRotationWereSet);
                }
            }
#endif

            if (gameObj.transform.parent != parent)
            {
                gameObj.transform.SetParent(parent, positionAndRotationWereSet);
            }

            if (gameObjectBindInfo.Name != null)
            {
                gameObj.name = gameObjectBindInfo.Name;
            }

            return gameObj;
        }

        public GameObject CreateEmptyGameObject(string name)
        {
            return CreateEmptyGameObject(new GameObjectCreationParameters { Name = name }, null);
        }

        public GameObject CreateEmptyGameObject(
            GameObjectCreationParameters gameObjectBindInfo, InjectContext context)
        {
            Assert.That(!AssertOnNewGameObjects,
                "Given DiContainer does not support creating new game objects");

            FlushBindings();

            var gameObj = new GameObject(gameObjectBindInfo.Name ?? "GameObject");
            var parent = GetTransformGroup(gameObjectBindInfo, context);

            if (parent == null)
            {
                // This ensures it gets added to the right scene instead of just the active scene
                gameObj.transform.SetParent(ContextTransform, false);
                gameObj.transform.SetParent(null, false);
            }
            else
            {
                gameObj.transform.SetParent(parent, false);
            }

            return gameObj;
        }

        Transform GetTransformGroup(
            GameObjectCreationParameters gameObjectBindInfo, InjectContext context)
        {
            Assert.That(!AssertOnNewGameObjects,
                "Given DiContainer does not support creating new game objects");

            if (gameObjectBindInfo.ParentTransform != null)
            {
                Assert.IsNull(gameObjectBindInfo.GroupName);
                Assert.IsNull(gameObjectBindInfo.ParentTransformGetter);

                return gameObjectBindInfo.ParentTransform;
            }

            // Don't execute the ParentTransformGetter method during validation
            // since it might do a resolve etc.
            if (gameObjectBindInfo.ParentTransformGetter != null && !IsValidating)
            {
                Assert.IsNull(gameObjectBindInfo.GroupName);

                if (context == null)
                {
                    context = new InjectContext
                    {
                        // This is the only information we can supply in this case
                        Container = this
                    };
                }

                // NOTE: Null is fine here, will just be a root game object in that case
                return gameObjectBindInfo.ParentTransformGetter(context);
            }

            var groupName = gameObjectBindInfo.GroupName;

            // Only use the inherited parent if is not set locally
            var defaultParent = _hasExplicitDefaultParent ? _explicitDefaultParent : _inheritedDefaultParent;

            if (defaultParent == null)
            {
                if (groupName == null)
                {
                    return null;
                }

                return (GameObject.Find("/" + groupName) ?? CreateTransformGroup(groupName)).transform;
            }

            if (groupName == null)
            {
                return defaultParent;
            }

            foreach (Transform child in defaultParent)
            {
                if (child.name == groupName)
                {
                    return child;
                }
            }

            var group = new GameObject(groupName).transform;
            group.SetParent(defaultParent, false);
            return group;
        }

        GameObject CreateTransformGroup(string groupName)
        {
            var gameObj = new GameObject(groupName);
            gameObj.transform.SetParent(ContextTransform, false);
            gameObj.transform.SetParent(null, false);
            return gameObj;
        }

#endif

        // Use this method to create any non-monobehaviour
        // Any fields marked [Inject] will be set using the bindings on the container
        // Any methods marked with a [Inject] will be called
        // Any constructor parameters will be filled in with values from the container
        public T Instantiate<T>()
        {
            return Instantiate<T>(new object[0]);
        }

        // Note: For IL2CPP platforms make sure to use new object[] instead of new [] when creating
        // the argument list to avoid errors converting to IEnumerable<object>
        public T Instantiate<T>(IEnumerable<object> extraArgs)
        {
            var result = Instantiate(typeof(T), extraArgs);

            if (IsValidating && !(result is T))
            {
                Assert.That(result is ValidationMarker);
                return default(T);
            }

            return (T)result;
        }

        public object Instantiate(Type concreteType)
        {
            return Instantiate(concreteType, new object[0]);
        }

        // Note: For IL2CPP platforms make sure to use new object[] instead of new [] when creating
        // the argument list to avoid errors converting to IEnumerable<object>
        public object Instantiate(
            Type concreteType, IEnumerable<object> extraArgs)
        {
            Assert.That(!extraArgs.ContainsItem(null),
                "Null value given to factory constructor arguments when instantiating object with type '{0}'. In order to use null use InstantiateExplicit", concreteType);

            return InstantiateExplicit(
                concreteType, InjectUtil.CreateArgList(extraArgs));
        }

#if !NOT_UNITY3D
        // Add new component to existing game object and fill in its dependencies
        // This is the same as AddComponent except the [Inject] fields will be filled in
        // NOTE: Gameobject here is not a prefab prototype, it is an instance
        public TContract InstantiateComponent<TContract>(GameObject gameObject)
            where TContract : Component
        {
            return InstantiateComponent<TContract>(gameObject, new object[0]);
        }

        // Add new component to existing game object and fill in its dependencies
        // This is the same as AddComponent except the [Inject] fields will be filled in
        // NOTE: Gameobject here is not a prefab prototype, it is an instance
        // Note: For IL2CPP platforms make sure to use new object[] instead of new [] when creating
        // the argument list to avoid errors converting to IEnumerable<object>
        public TContract InstantiateComponent<TContract>(
            GameObject gameObject, IEnumerable<object> extraArgs)
            where TContract : Component
        {
            return (TContract)InstantiateComponent(typeof(TContract), gameObject, extraArgs);
        }

        // Add new component to existing game object and fill in its dependencies
        // This is the same as AddComponent except the [Inject] fields will be filled in
        // NOTE: Gameobject here is not a prefab prototype, it is an instance
        public Component InstantiateComponent(
            Type componentType, GameObject gameObject)
        {
            return InstantiateComponent(componentType, gameObject, new object[0]);
        }

        // Add new component to existing game object and fill in its dependencies
        // This is the same as AddComponent except the [Inject] fields will be filled in
        // NOTE: Gameobject here is not a prefab prototype, it is an instance
        // Note: For IL2CPP platforms make sure to use new object[] instead of new [] when creating
        // the argument list to avoid errors converting to IEnumerable<object>
        public Component InstantiateComponent(
            Type componentType, GameObject gameObject, IEnumerable<object> extraArgs)
        {
            return InstantiateComponentExplicit(
                componentType, gameObject, InjectUtil.CreateArgList(extraArgs));
        }

        public T InstantiateComponentOnNewGameObject<T>()
            where T : Component
        {
            return InstantiateComponentOnNewGameObject<T>(typeof(T).Name);
        }

        // Note: For IL2CPP platforms make sure to use new object[] instead of new [] when creating
        // the argument list to avoid errors converting to IEnumerable<object>
        public T InstantiateComponentOnNewGameObject<T>(IEnumerable<object> extraArgs)
            where T : Component
        {
            return InstantiateComponentOnNewGameObject<T>(typeof(T).Name, extraArgs);
        }

        public T InstantiateComponentOnNewGameObject<T>(string gameObjectName)
            where T : Component
        {
            return InstantiateComponentOnNewGameObject<T>(gameObjectName, new object[0]);
        }

        // Note: For IL2CPP platforms make sure to use new object[] instead of new [] when creating
        // the argument list to avoid errors converting to IEnumerable<object>
        public T InstantiateComponentOnNewGameObject<T>(
            string gameObjectName, IEnumerable<object> extraArgs)
            where T : Component
        {
            return InstantiateComponent<T>(
                CreateEmptyGameObject(gameObjectName),
                extraArgs);
        }

        // Create a new game object from a prefab and fill in dependencies for all children
        public GameObject InstantiatePrefab(UnityEngine.Object prefab)
        {
            return InstantiatePrefab(
                prefab, GameObjectCreationParameters.Default);
        }

        // Create a new game object from a prefab and fill in dependencies for all children
        public GameObject InstantiatePrefab(UnityEngine.Object prefab, Transform parentTransform)
        {
            return InstantiatePrefab(
                prefab, new GameObjectCreationParameters { ParentTransform = parentTransform });
        }

        // Create a new game object from a prefab and fill in dependencies for all children
        public GameObject InstantiatePrefab(
            UnityEngine.Object prefab, Vector3 position, Quaternion rotation, Transform parentTransform)
        {
            return InstantiatePrefab(
                prefab, new GameObjectCreationParameters
                {
                    ParentTransform = parentTransform,
                    Position = position,
                    Rotation = rotation
                });
        }

        // Create a new game object from a prefab and fill in dependencies for all children
        public GameObject InstantiatePrefab(
            UnityEngine.Object prefab, GameObjectCreationParameters gameObjectBindInfo)
        {
            FlushBindings();

            bool shouldMakeActive;
            var gameObj = CreateAndParentPrefab(
                prefab, gameObjectBindInfo, null, out shouldMakeActive);

            InjectGameObject(gameObj);

            if (shouldMakeActive && !IsValidating)
            {
#if ZEN_INTERNAL_PROFILING
                using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
                {
                    gameObj.SetActive(true);
                }
            }

            return gameObj;
        }

        // Create a new game object from a resource path and fill in dependencies for all children
        public GameObject InstantiatePrefabResource(string resourcePath)
        {
            return InstantiatePrefabResource(resourcePath, GameObjectCreationParameters.Default);
        }

        // Create a new game object from a resource path and fill in dependencies for all children
        public GameObject InstantiatePrefabResource(string resourcePath, Transform parentTransform)
        {
            return InstantiatePrefabResource(resourcePath, new GameObjectCreationParameters { ParentTransform = parentTransform });
        }

        public GameObject InstantiatePrefabResource(
            string resourcePath, Vector3 position, Quaternion rotation, Transform parentTransform)
        {
            return InstantiatePrefabResource(
                resourcePath, new GameObjectCreationParameters
                {
                    ParentTransform = parentTransform,
                    Position = position,
                    Rotation = rotation
                });
        }

        // Create a new game object from a resource path and fill in dependencies for all children
        public GameObject InstantiatePrefabResource(
            string resourcePath, GameObjectCreationParameters creationInfo)
        {
            var prefab = (GameObject)Resources.Load(resourcePath);

            Assert.IsNotNull(prefab,
                "Could not find prefab at resource location '{0}'".Fmt(resourcePath));

            return InstantiatePrefab(prefab, creationInfo);
        }

        // Same as InstantiatePrefab but returns a component after it's initialized
        // and optionally allows extra arguments for the given component type
        public T InstantiatePrefabForComponent<T>(UnityEngine.Object prefab)
        {
            return (T)InstantiatePrefabForComponent(
                typeof(T), prefab, null, new object[0]);
        }

        // Same as InstantiatePrefab but returns a component after it's initialized
        // and optionally allows extra arguments for the given component type
        // Note: For IL2CPP platforms make sure to use new object[] instead of new [] when creating
        // the argument list to avoid errors converting to IEnumerable<object>
        public T InstantiatePrefabForComponent<T>(
            UnityEngine.Object prefab, IEnumerable<object> extraArgs)
        {
            return (T)InstantiatePrefabForComponent(
                typeof(T), prefab, null, extraArgs);
        }

        public T InstantiatePrefabForComponent<T>(
            UnityEngine.Object prefab, Transform parentTransform)
        {
            return (T)InstantiatePrefabForComponent(
                typeof(T), prefab, parentTransform, new object[0]);
        }

        // Note: For IL2CPP platforms make sure to use new object[] instead of new [] when creating
        // the argument list to avoid errors converting to IEnumerable<object>
        public T InstantiatePrefabForComponent<T>(
            UnityEngine.Object prefab, Transform parentTransform, IEnumerable<object> extraArgs)
        {
            return (T)InstantiatePrefabForComponent(
                typeof(T), prefab, parentTransform, extraArgs);
        }

        public T InstantiatePrefabForComponent<T>(
            UnityEngine.Object prefab, Vector3 position, Quaternion rotation, Transform parentTransform)
        {
            return (T)InstantiatePrefabForComponent(
                typeof(T), prefab, new object[0], new GameObjectCreationParameters
                {
                    ParentTransform = parentTransform,
                    Position = position,
                    Rotation = rotation
                });
        }

        public T InstantiatePrefabForComponent<T>(
            UnityEngine.Object prefab, Vector3 position, Quaternion rotation, Transform parentTransform, IEnumerable<object> extraArgs)
        {
            return (T)InstantiatePrefabForComponent(
                typeof(T), prefab, extraArgs, new GameObjectCreationParameters
                {
                    ParentTransform = parentTransform,
                    Position = position,
                    Rotation = rotation
                });
        }

        // Same as InstantiatePrefab but returns a component after it's initialized
        // and optionally allows extra arguments for the given component type
        // Note: For IL2CPP platforms make sure to use new object[] instead of new [] when creating
        // the argument list to avoid errors converting to IEnumerable<object>
        public object InstantiatePrefabForComponent(
            Type concreteType, UnityEngine.Object prefab,
            Transform parentTransform, IEnumerable<object> extraArgs)
        {
            return InstantiatePrefabForComponent(
                concreteType, prefab, extraArgs,
                new GameObjectCreationParameters { ParentTransform = parentTransform });
        }

        // Note: For IL2CPP platforms make sure to use new object[] instead of new [] when creating
        // the argument list to avoid errors converting to IEnumerable<object>
        public object InstantiatePrefabForComponent(
            Type concreteType, UnityEngine.Object prefab,
            IEnumerable<object> extraArgs, GameObjectCreationParameters creationInfo)
        {
            return InstantiatePrefabForComponentExplicit(
                concreteType, prefab,
                InjectUtil.CreateArgList(extraArgs), creationInfo);
        }

        // Same as InstantiatePrefabResource but returns a component after it's initialized
        // and optionally allows extra arguments for the given component type
        public T InstantiatePrefabResourceForComponent<T>(string resourcePath)
        {
            return (T)InstantiatePrefabResourceForComponent(
                typeof(T), resourcePath, null, new object[0]);
        }

        // Same as InstantiatePrefabResource but returns a component after it's initialized
        // and optionally allows extra arguments for the given component type
        // Note: For IL2CPP platforms make sure to use new object[] instead of new [] when creating
        // the argument list to avoid errors converting to IEnumerable<object>
        public T InstantiatePrefabResourceForComponent<T>(
            string resourcePath, IEnumerable<object> extraArgs)
        {
            return (T)InstantiatePrefabResourceForComponent(
                typeof(T), resourcePath, null, extraArgs);
        }

        public T InstantiatePrefabResourceForComponent<T>(
            string resourcePath, Transform parentTransform)
        {
            return (T)InstantiatePrefabResourceForComponent(
                typeof(T), resourcePath, parentTransform, new object[0]);
        }

        // Note: For IL2CPP platforms make sure to use new object[] instead of new [] when creating
        // the argument list to avoid errors converting to IEnumerable<object>
        public T InstantiatePrefabResourceForComponent<T>(
            string resourcePath, Transform parentTransform, IEnumerable<object> extraArgs)
        {
            return (T)InstantiatePrefabResourceForComponent(
                typeof(T), resourcePath, parentTransform, extraArgs);
        }

        public T InstantiatePrefabResourceForComponent<T>(
            string resourcePath, Vector3 position, Quaternion rotation, Transform parentTransform)
        {
            return InstantiatePrefabResourceForComponent<T>(resourcePath, position, rotation, parentTransform, new object[0]);
        }

        public T InstantiatePrefabResourceForComponent<T>(
            string resourcePath, Vector3 position, Quaternion rotation, Transform parentTransform, IEnumerable<object> extraArgs)
        {
            var argsList = InjectUtil.CreateArgList(extraArgs);
            var creationParameters = new GameObjectCreationParameters
            {
                ParentTransform = parentTransform,
                Position = position,
                Rotation = rotation
            };
            return (T)InstantiatePrefabResourceForComponentExplicit(
                typeof(T), resourcePath, argsList, creationParameters);
        }

        // Same as InstantiatePrefabResource but returns a component after it's initialized
        // and optionally allows extra arguments for the given component type
        // Note: For IL2CPP platforms make sure to use new object[] instead of new [] when creating
        // the argument list to avoid errors converting to IEnumerable<object>
        public object InstantiatePrefabResourceForComponent(
            Type concreteType, string resourcePath, Transform parentTransform,
            IEnumerable<object> extraArgs)
        {
            Assert.That(!extraArgs.ContainsItem(null),
                "Null value given to factory constructor arguments when instantiating object with type '{0}'. In order to use null use InstantiatePrefabForComponentExplicit", concreteType);

            return InstantiatePrefabResourceForComponentExplicit(
                concreteType, resourcePath,
                InjectUtil.CreateArgList(extraArgs),
                new GameObjectCreationParameters { ParentTransform = parentTransform });
        }

        public T InstantiateScriptableObjectResource<T>(string resourcePath)
            where T : ScriptableObject
        {
            return InstantiateScriptableObjectResource<T>(resourcePath, new object[0]);
        }

        // Note: For IL2CPP platforms make sure to use new object[] instead of new [] when creating
        // the argument list to avoid errors converting to IEnumerable<object>
        public T InstantiateScriptableObjectResource<T>(
            string resourcePath, IEnumerable<object> extraArgs)
            where T : ScriptableObject
        {
            return (T)InstantiateScriptableObjectResource(
                typeof(T), resourcePath, extraArgs);
        }

        public object InstantiateScriptableObjectResource(
            Type scriptableObjectType, string resourcePath)
        {
            return InstantiateScriptableObjectResource(
                scriptableObjectType, resourcePath, new object[0]);
        }

        // Note: For IL2CPP platforms make sure to use new object[] instead of new [] when creating
        // the argument list to avoid errors converting to IEnumerable<object>
        public object InstantiateScriptableObjectResource(
            Type scriptableObjectType, string resourcePath, IEnumerable<object> extraArgs)
        {
            Assert.DerivesFromOrEqual<ScriptableObject>(scriptableObjectType);
            return InstantiateScriptableObjectResourceExplicit(
                scriptableObjectType, resourcePath, InjectUtil.CreateArgList(extraArgs));
        }

        // Inject dependencies into any and all child components on the given game object
        public void InjectGameObject(GameObject gameObject)
        {
            FlushBindings();

            ZenUtilInternal.AddStateMachineBehaviourAutoInjectersUnderGameObject(gameObject);

            var monoBehaviours = ZenPools.SpawnList<MonoBehaviour>();
            try
            {
                ZenUtilInternal.GetInjectableMonoBehavioursUnderGameObject(gameObject, monoBehaviours);

                for (int i = 0; i < monoBehaviours.Count; i++)
                {
                    Inject(monoBehaviours[i]);
                }
            }
            finally
            {
                ZenPools.DespawnList(monoBehaviours);
            }
        }

        // Same as InjectGameObject except it will also search the game object for the
        // given component, and also optionally allow passing extra inject arguments into the
        // given component
        public T InjectGameObjectForComponent<T>(GameObject gameObject)
            where T : Component
        {
            return InjectGameObjectForComponent<T>(gameObject, new object[0]);
        }

        // Same as InjectGameObject except it will also search the game object for the
        // given component, and also optionally allow passing extra inject arguments into the
        // given component
        // Note: For IL2CPP platforms make sure to use new object[] instead of new [] when creating
        // the argument list to avoid errors converting to IEnumerable<object>
        public T InjectGameObjectForComponent<T>(
            GameObject gameObject, IEnumerable<object> extraArgs)
            where T : Component
        {
            return (T)InjectGameObjectForComponent(gameObject, typeof(T), extraArgs);
        }

        // Same as InjectGameObject except it will also search the game object for the
        // given component, and also optionally allow passing extra inject arguments into the
        // given component
        // Note: For IL2CPP platforms make sure to use new object[] instead of new [] when creating
        // the argument list to avoid errors converting to IEnumerable<object>
        public object InjectGameObjectForComponent(
            GameObject gameObject, Type componentType, IEnumerable<object> extraArgs)
        {
            return InjectGameObjectForComponentExplicit(
                gameObject, componentType, InjectUtil.CreateArgList(extraArgs), new InjectContext(this, componentType, null), null);
        }

        // Same as InjectGameObjectForComponent except allows null values
        // to be included in the argument list.  Also see InjectUtil.CreateArgList
        public Component InjectGameObjectForComponentExplicit(
            GameObject gameObject, Type componentType, List<TypeValuePair> extraArgs, InjectContext context, object concreteIdentifier)
        {
            if (!componentType.DerivesFrom<MonoBehaviour>() && extraArgs.Count > 0)
            {
                throw Assert.CreateException(
                    "Cannot inject into non-monobehaviours!  Argument list must be zero length");
            }

            ZenUtilInternal.AddStateMachineBehaviourAutoInjectersUnderGameObject(gameObject);

            var injectableMonoBehaviours = ZenPools.SpawnList<MonoBehaviour>();
            try
            {

                ZenUtilInternal.GetInjectableMonoBehavioursUnderGameObject(gameObject, injectableMonoBehaviours);

                for (int i = 0; i < injectableMonoBehaviours.Count; i++)
                {
                    var monoBehaviour = injectableMonoBehaviours[i];
                    if (monoBehaviour.GetType().DerivesFromOrEqual(componentType))
                    {
                        InjectExplicit(monoBehaviour, monoBehaviour.GetType(), extraArgs, context, concreteIdentifier);
                    }
                    else
                    {
                        Inject(monoBehaviour);
                    }
                }
            }
            finally
            {
                ZenPools.DespawnList(injectableMonoBehaviours);
            }

            var matches = gameObject.GetComponentsInChildren(componentType, true);

            Assert.That(matches.Length > 0,
                "Expected to find component with type '{0}' when injecting into game object '{1}'", componentType, gameObject.name);

            Assert.That(matches.Length == 1,
                "Found multiple component with type '{0}' when injecting into game object '{1}'", componentType, gameObject.name);

            return matches[0];
        }
#endif

        // When you call any of these Inject methods
        //    Any fields marked [Inject] will be set using the bindings on the container
        //    Any methods marked with a [Inject] will be called
        //    Any constructor parameters will be filled in with values from the container
        public void Inject(object injectable)
        {
            Inject(injectable, new object[0]);
        }

        // Same as Inject(injectable) except allows adding extra values to be injected
        // Note: For IL2CPP platforms make sure to use new object[] instead of new [] when creating
        // the argument list to avoid errors converting to IEnumerable<object>
        public void Inject(object injectable, IEnumerable<object> extraArgs)
        {
            InjectExplicit(
                injectable, InjectUtil.CreateArgList(extraArgs));
        }

        // Resolve<> - Lookup a value in the container.
        //
        // Note that this may result in a new object being created (for transient bindings) or it
        // may return an already created object (for FromInstance or ToSingle, etc. bindings)
        //
        // If a single unique value for the given type cannot be found, an exception is thrown.
        //
        public TContract Resolve<TContract>()
        {
            return (TContract)Resolve(typeof(TContract));
        }

        public object Resolve(Type contractType)
        {
            return ResolveId(contractType, null);
        }

        public TContract ResolveId<TContract>(object identifier)
        {
            return (TContract)ResolveId(typeof(TContract), identifier);
        }

        public object ResolveId(Type contractType, object identifier)
        {
            using (var context = ZenPools.SpawnInjectContext(this, contractType))
            {
                context.Identifier = identifier;
                return Resolve(context);
            }
        }

        // Same as Resolve<> except it will return null if a value for the given type cannot
        // be found.
        public TContract TryResolve<TContract>()
            where TContract : class
        {
            return (TContract)TryResolve(typeof(TContract));
        }

        public object TryResolve(Type contractType)
        {
            return TryResolveId(contractType, null);
        }

        public TContract TryResolveId<TContract>(object identifier)
            where TContract : class
        {
            return (TContract)TryResolveId(
                typeof(TContract), identifier);
        }

        public object TryResolveId(Type contractType, object identifier)
        {
            using (var context = ZenPools.SpawnInjectContext(this, contractType))
            {
                context.Identifier = identifier;
                context.Optional = true;
                return Resolve(context);
            }
        }

        // Same as Resolve<> except it will return all bindings that are associated with the given type
        public List<TContract> ResolveAll<TContract>()
        {
            return (List<TContract>)ResolveAll(typeof(TContract));
        }

        public IList ResolveAll(Type contractType)
        {
            return ResolveIdAll(contractType, null);
        }

        public List<TContract> ResolveIdAll<TContract>(object identifier)
        {
            return (List<TContract>)ResolveIdAll(typeof(TContract), identifier);
        }

        public IList ResolveIdAll(Type contractType, object identifier)
        {
            using (var context = ZenPools.SpawnInjectContext(this, contractType))
            {
                context.Identifier = identifier;
                context.Optional = true;
                return ResolveAll(context);
            }
        }

        // Removes all bindings
        public void UnbindAll()
        {
            FlushBindings();
            _providers.Clear();
        }

        // Remove all bindings bound to the given contract type
        public bool Unbind<TContract>()
        {
            return Unbind(typeof(TContract));
        }

        public bool Unbind(Type contractType)
        {
            return UnbindId(contractType, null);
        }

        public bool UnbindId<TContract>(object identifier)
        {
            return UnbindId(typeof(TContract), identifier);
        }

        public bool UnbindId(Type contractType, object identifier)
        {
            FlushBindings();

            var bindingId = new BindingId(contractType, identifier);

            return _providers.Remove(bindingId);
        }

        public void UnbindInterfacesTo<TConcrete>()
        {
            UnbindInterfacesTo(typeof(TConcrete));
        }

        public void UnbindInterfacesTo(Type concreteType)
        {
            foreach (var i in concreteType.Interfaces())
            {
                Unbind(i, concreteType);
            }
        }

        public bool Unbind<TContract, TConcrete>()
        {
            return Unbind(typeof(TContract), typeof(TConcrete));
        }

        public bool Unbind(Type contractType, Type concreteType)
        {
            return UnbindId(contractType, concreteType, null);
        }

        public bool UnbindId<TContract, TConcrete>(object identifier)
        {
            return UnbindId(typeof(TContract), typeof(TConcrete), identifier);
        }

        public bool UnbindId(Type contractType, Type concreteType, object identifier)
        {
            FlushBindings();

            var bindingId = new BindingId(contractType, identifier);

            List<ProviderInfo> providers;

            if (!_providers.TryGetValue(bindingId, out providers))
            {
                return false;
            }

            var matches = providers.Where(x => x.Provider.GetInstanceType(new InjectContext(this, contractType, identifier)).DerivesFromOrEqual(concreteType)).ToList();

            if (matches.Count == 0)
            {
                return false;
            }

            foreach (var info in matches)
            {
                bool success = providers.Remove(info);
                Assert.That(success);
            }

            return true;
        }

        // Returns true if the given type is bound to something in the container
        public bool HasBinding<TContract>()
        {
            return HasBinding(typeof(TContract));
        }

        public bool HasBinding(Type contractType)
        {
            return HasBindingId(contractType, null);
        }

        public bool HasBindingId<TContract>(object identifier)
        {
            return HasBindingId(typeof(TContract), identifier);
        }

        public bool HasBindingId(Type contractType, object identifier)
        {
            return HasBindingId(contractType, identifier, InjectSources.Any);
        }

        public bool HasBindingId(Type contractType, object identifier, InjectSources sourceType)
        {
            using (var ctx = ZenPools.SpawnInjectContext(this, contractType))
            {
                ctx.Identifier = identifier;
                ctx.SourceType = sourceType;
                return HasBinding(ctx);
            }
        }

        // You shouldn't need to use this
        public bool HasBinding(InjectContext context)
        {
            Assert.IsNotNull(context);

            FlushBindings();

            var matches = ZenPools.SpawnList<ProviderInfo>();

            try
            {
                GetProviderMatches(context, matches);
                return matches.Count > 0;
            }
            finally
            {
                ZenPools.DespawnList(matches);
            }
        }

        // You shouldn't need to use this
        public void FlushBindings()
        {
            while (_currentBindings.Count > 0)
            {
                var binding = _currentBindings.Dequeue();

                if (binding.BindingInheritanceMethod != BindingInheritanceMethods.MoveDirectOnly
                    && binding.BindingInheritanceMethod != BindingInheritanceMethods.MoveIntoAll)
                {
                    FinalizeBinding(binding);
                }

                if (binding.BindingInheritanceMethod != BindingInheritanceMethods.None)
                {
                    _childBindings.Add(binding);
                }
                else
                {
                    binding.Dispose();
                }
            }
        }

        void FinalizeBinding(BindStatement binding)
        {
            _isFinalizingBinding = true;

            try
            {
                binding.FinalizeBinding(this);
            }
            finally
            {
                _isFinalizingBinding = false;
            }
        }

        // Don't use this method
        public BindStatement StartBinding(bool flush = true)
        {
            Assert.That(!_isFinalizingBinding,
                "Attempted to start a binding during a binding finalizer.  This is not allowed, since binding finalizers should directly use AddProvider instead, to allow for bindings to be inherited properly without duplicates");

            if (flush)
            {
                FlushBindings();
            }

            var bindStatement = ZenPools.SpawnStatement();
            _currentBindings.Enqueue(bindStatement);
            return bindStatement;
        }

        public ConcreteBinderGeneric<TContract> Rebind<TContract>()
        {
            return RebindId<TContract>(null);
        }

        public ConcreteBinderGeneric<TContract> RebindId<TContract>(object identifier)
        {
            UnbindId<TContract>(identifier);
            return Bind<TContract>().WithId(identifier);
        }

        public ConcreteBinderNonGeneric Rebind(Type contractType)
        {
            return RebindId(contractType, null);
        }

        public ConcreteBinderNonGeneric RebindId(Type contractType, object identifier)
        {
            UnbindId(contractType, identifier);
            return Bind(contractType).WithId(identifier);
        }

        // Map the given type to a way of obtaining it
        // Note that this can include open generic types as well such as List<>
        public ConcreteIdBinderGeneric<TContract> Bind<TContract>()
        {
            return Bind<TContract>(StartBinding());
        }

        // This is only useful for complex cases where you want to add multiple bindings
        // at the same time and can be ignored by 99% of users
        public ConcreteIdBinderGeneric<TContract> BindNoFlush<TContract>()
        {
            return Bind<TContract>(StartBinding(false));
        }

        ConcreteIdBinderGeneric<TContract> Bind<TContract>(
            BindStatement bindStatement)
        {
            var bindInfo = bindStatement.SpawnBindInfo();

            Assert.That(!typeof(TContract).DerivesFrom<IPlaceholderFactory>(),
                "You should not use Container.Bind for factory classes.  Use Container.BindFactory instead.");

            Assert.That(!bindInfo.ContractTypes.Contains(typeof(TContract)));
            bindInfo.ContractTypes.Add(typeof(TContract));

            return new ConcreteIdBinderGeneric<TContract>(
                this, bindInfo, bindStatement);
        }

        // Non-generic version of Bind<> for cases where you only have the runtime type
        // Note that this can include open generic types as well such as List<>
        public ConcreteIdBinderNonGeneric Bind(params Type[] contractTypes)
        {
            var statement = StartBinding();
            var bindInfo = statement.SpawnBindInfo();
            bindInfo.ContractTypes.AllocFreeAddRange(contractTypes);
            return BindInternal(bindInfo, statement);
        }

        public ConcreteIdBinderNonGeneric Bind(IEnumerable<Type> contractTypes)
        {
            var statement = StartBinding();
            var bindInfo = statement.SpawnBindInfo();
            bindInfo.ContractTypes.AddRange(contractTypes);
            return BindInternal(bindInfo, statement);
        }

        ConcreteIdBinderNonGeneric BindInternal(
            BindInfo bindInfo, BindStatement bindingFinalizer)
        {
#if ZEN_INTERNAL_PROFILING
            using (ProfileTimers.CreateTimedBlock("DiContainer.Bind"))
#endif
            {
                Assert.That(bindInfo.ContractTypes.All(x => !x.DerivesFrom<IPlaceholderFactory>()),
                    "You should not use Container.Bind for factory classes.  Use Container.BindFactory instead.");

                return new ConcreteIdBinderNonGeneric(this, bindInfo, bindingFinalizer);
            }
        }

#if !(UNITY_WSA && ENABLE_DOTNET)
        public ConcreteIdBinderNonGeneric Bind(
            Action<ConventionSelectTypesBinder> generator)
        {
            var conventionBindInfo = new ConventionBindInfo();
            generator(new ConventionSelectTypesBinder(conventionBindInfo));

            var contractTypesList = conventionBindInfo.ResolveTypes();

            Assert.That(contractTypesList.All(x => !x.DerivesFrom<IPlaceholderFactory>()),
                "You should not use Container.Bind for factory classes.  Use Container.BindFactory instead.");

            var statement = StartBinding();
            var bindInfo = statement.SpawnBindInfo();
            bindInfo.ContractTypes.AllocFreeAddRange(contractTypesList);

            // This is nice because it allows us to do things like Bind(all interfaces).To<Foo>()
            // (though of course it would be more efficient to use BindInterfacesTo in this case)
            bindInfo.InvalidBindResponse = InvalidBindResponses.Skip;

            return new ConcreteIdBinderNonGeneric(this, bindInfo, statement);
        }
#endif

        // Bind all the interfaces for the given type to the same thing.
        //
        // Example:
        //
        //    public class Foo : ITickable, IInitializable
        //    {
        //    }
        //
        //    Container.BindInterfacesTo<Foo>().AsSingle();
        //
        //  This line above is equivalent to the following:
        //
        //    Container.Bind<ITickable>().ToSingle<Foo>();
        //    Container.Bind<IInitializable>().ToSingle<Foo>();
        //
        // Note here that we do not bind Foo to itself.  For that, use BindInterfacesAndSelfTo
        public FromBinderNonGeneric BindInterfacesTo<T>()
        {
            return BindInterfacesTo(typeof(T));
        }

        public FromBinderNonGeneric BindInterfacesTo(Type type)
        {
            var statement = StartBinding();
            var bindInfo = statement.SpawnBindInfo();

            var interfaces = type.Interfaces();

            if (interfaces.Length == 0)
            {
                Log.Warn("Called BindInterfacesTo for type {0} but no interfaces were found", type);
            }

            bindInfo.ContractTypes.AllocFreeAddRange(interfaces);
            bindInfo.SetContextInfo("BindInterfacesTo({0})".Fmt(type));

            // Almost always, you don't want to use the default AsTransient so make them type it
            bindInfo.RequireExplicitScope = true;
            return BindInternal(bindInfo, statement).To(type);
        }

        // Same as BindInterfaces except also binds to self
        public FromBinderNonGeneric BindInterfacesAndSelfTo<T>()
        {
            return BindInterfacesAndSelfTo(typeof(T));
        }

        public FromBinderNonGeneric BindInterfacesAndSelfTo(Type type)
        {
            var statement = StartBinding();
            var bindInfo = statement.SpawnBindInfo();

            bindInfo.ContractTypes.AllocFreeAddRange(type.Interfaces());
            bindInfo.ContractTypes.Add(type);

            bindInfo.SetContextInfo("BindInterfacesAndSelfTo({0})".Fmt(type));

            // Almost always, you don't want to use the default AsTransient so make them type it
            bindInfo.RequireExplicitScope = true;
            return BindInternal(bindInfo, statement).To(type);
        }

        //  This is simply a shortcut to using the FromInstance method.
        //
        //  Example:
        //      Container.BindInstance(new Foo());
        //
        //  This line above is equivalent to the following:
        //
        //      Container.Bind<Foo>().FromInstance(new Foo());
        //
        public IdScopeConcreteIdArgConditionCopyNonLazyBinder BindInstance<TContract>(TContract instance)
        {
            var statement = StartBinding();
            var bindInfo = statement.SpawnBindInfo();
            bindInfo.ContractTypes.Add(typeof(TContract));

            statement.SetFinalizer(
                new ScopableBindingFinalizer(
                    bindInfo,
                    (container, type) => new InstanceProvider(type, instance, container)));

            return new IdScopeConcreteIdArgConditionCopyNonLazyBinder(bindInfo);
        }

        // Unfortunately we can't support setting scope / condition / etc. here since all the
        // bindings are finalized one at a time
        public void BindInstances(params object[] instances)
        {
            for (int i = 0; i < instances.Length; i++)
            {
                var instance = instances[i];

                Assert.That(!ZenUtilInternal.IsNull(instance),
                    "Found null instance provided to BindInstances method");

                Bind(instance.GetType()).FromInstance(instance);
            }
        }

        FactoryToChoiceIdBinder<TContract> BindFactoryInternal<TContract, TFactoryContract, TFactoryConcrete>()
            where TFactoryConcrete : TFactoryContract, IFactory
            where TFactoryContract : IFactory
        {
            var statement = StartBinding();
            var bindInfo = statement.SpawnBindInfo();
            bindInfo.ContractTypes.Add(typeof(TFactoryContract));

            var factoryBindInfo = new FactoryBindInfo(typeof(TFactoryConcrete));

            statement.SetFinalizer(
                new PlaceholderFactoryBindingFinalizer<TContract>(
                    bindInfo, factoryBindInfo));

            return new FactoryToChoiceIdBinder<TContract>(
                this, bindInfo, factoryBindInfo);
        }

        public FactoryToChoiceIdBinder<TContract> BindIFactory<TContract>()
        {
            return BindFactoryInternal<TContract, IFactory<TContract>, PlaceholderFactory<TContract>>();
        }

        public FactoryToChoiceIdBinder<TContract> BindFactory<TContract, TFactory>()
            where TFactory : PlaceholderFactory<TContract>
        {
            return BindFactoryInternal<TContract, TFactory, TFactory>();
        }

        public FactoryToChoiceIdBinder<TContract> BindFactoryCustomInterface<TContract, TFactoryConcrete, TFactoryContract>()
            where TFactoryConcrete : PlaceholderFactory<TContract>, TFactoryContract
            where TFactoryContract : IFactory
        {
            return BindFactoryInternal<TContract, TFactoryContract, TFactoryConcrete>();
        }

        public MemoryPoolIdInitialSizeMaxSizeBinder<TItemContract> BindMemoryPool<TItemContract>()
        {
            return BindMemoryPool<TItemContract, MemoryPool<TItemContract>>();
        }

        public MemoryPoolIdInitialSizeMaxSizeBinder<TItemContract> BindMemoryPool<TItemContract, TPool>()
            where TPool : IMemoryPool
        {
            return BindMemoryPoolCustomInterface<TItemContract, TPool, TPool>();
        }

        public MemoryPoolIdInitialSizeMaxSizeBinder<TItemContract> BindMemoryPoolCustomInterface<TItemContract, TPoolConcrete, TPoolContract>(bool includeConcreteType = false)
            where TPoolConcrete : TPoolContract, IMemoryPool
            where TPoolContract : IMemoryPool
        {
            return BindMemoryPoolCustomInterfaceInternal<TItemContract, TPoolConcrete, TPoolContract>(includeConcreteType, StartBinding());
        }

        internal MemoryPoolIdInitialSizeMaxSizeBinder<TItemContract> BindMemoryPoolCustomInterfaceNoFlush<TItemContract, TPoolConcrete, TPoolContract>(bool includeConcreteType = false)
            where TPoolConcrete : TPoolContract, IMemoryPool
            where TPoolContract : IMemoryPool
        {
            return BindMemoryPoolCustomInterfaceInternal<TItemContract, TPoolConcrete, TPoolContract>(includeConcreteType, StartBinding(false));
        }

        MemoryPoolIdInitialSizeMaxSizeBinder<TItemContract> BindMemoryPoolCustomInterfaceInternal<TItemContract, TPoolConcrete, TPoolContract>(
            bool includeConcreteType, BindStatement statement)
            where TPoolConcrete : TPoolContract, IMemoryPool
            where TPoolContract : IMemoryPool
        {
            var contractTypes = new List<Type> { typeof(IDisposable), typeof(TPoolContract) };

            if (includeConcreteType)
            {
                contractTypes.Add(typeof(TPoolConcrete));
            }

            var bindInfo = statement.SpawnBindInfo();

            bindInfo.ContractTypes.AllocFreeAddRange(contractTypes);

            // This interface is used in the optional class PoolCleanupChecker
            // And also allow people to manually call DespawnAll() for all IMemoryPool
            // if they want
            bindInfo.ContractTypes.Add(typeof(IMemoryPool));

            var factoryBindInfo = new FactoryBindInfo(typeof(TPoolConcrete));
            var poolBindInfo = new MemoryPoolBindInfo();

            statement.SetFinalizer(
                new MemoryPoolBindingFinalizer<TItemContract>(
                    bindInfo, factoryBindInfo, poolBindInfo));

            return new MemoryPoolIdInitialSizeMaxSizeBinder<TItemContract>(
                this, bindInfo, factoryBindInfo, poolBindInfo);
        }

        FactoryToChoiceIdBinder<TParam1, TContract> BindFactoryInternal<TParam1, TContract, TFactoryContract, TFactoryConcrete>()
            where TFactoryConcrete : TFactoryContract, IFactory
            where TFactoryContract : IFactory
        {
            var statement = StartBinding();
            var bindInfo = statement.SpawnBindInfo();

            bindInfo.ContractTypes.Add(typeof(TFactoryContract));

            var factoryBindInfo = new FactoryBindInfo(typeof(TFactoryConcrete));

            statement.SetFinalizer(
                new PlaceholderFactoryBindingFinalizer<TContract>(
                    bindInfo, factoryBindInfo));

            return new FactoryToChoiceIdBinder<TParam1, TContract>(
                this, bindInfo, factoryBindInfo);
        }

        public FactoryToChoiceIdBinder<TParam1, TContract> BindIFactory<TParam1, TContract>()
        {
            return BindFactoryInternal<
                TParam1, TContract, IFactory<TParam1, TContract>, PlaceholderFactory<TParam1, TContract>>();
        }

        public FactoryToChoiceIdBinder<TParam1, TContract> BindFactory<TParam1, TContract, TFactory>()
            where TFactory : PlaceholderFactory<TParam1, TContract>
        {
            return BindFactoryInternal<
                TParam1, TContract, TFactory, TFactory>();
        }

        public FactoryToChoiceIdBinder<TParam1, TContract> BindFactoryCustomInterface<TParam1, TContract, TFactoryConcrete, TFactoryContract>()
            where TFactoryConcrete : PlaceholderFactory<TParam1, TContract>, TFactoryContract
            where TFactoryContract : IFactory
        {
            return BindFactoryInternal<TParam1, TContract, TFactoryContract, TFactoryConcrete>();
        }

        FactoryToChoiceIdBinder<TParam1, TParam2, TContract> BindFactoryInternal<TParam1, TParam2, TContract, TFactoryContract, TFactoryConcrete>()
            where TFactoryConcrete : TFactoryContract, IFactory
            where TFactoryContract : IFactory
        {
            var statement = StartBinding();
            var bindInfo = statement.SpawnBindInfo();

            bindInfo.ContractTypes.Add(typeof(TFactoryContract));

            var factoryBindInfo = new FactoryBindInfo(typeof(TFactoryConcrete));

            statement.SetFinalizer(
                new PlaceholderFactoryBindingFinalizer<TContract>(
                    bindInfo, factoryBindInfo));

            return new FactoryToChoiceIdBinder<TParam1, TParam2, TContract>(
                this, bindInfo, factoryBindInfo);
        }

        public FactoryToChoiceIdBinder<TParam1, TParam2, TContract> BindIFactory<TParam1, TParam2, TContract>()
        {
            return BindFactoryInternal<
                TParam1, TParam2, TContract, IFactory<TParam1, TParam2, TContract>, PlaceholderFactory<TParam1, TParam2, TContract>>();
        }

        public FactoryToChoiceIdBinder<TParam1, TParam2, TContract> BindFactory<TParam1, TParam2, TContract, TFactory>()
            where TFactory : PlaceholderFactory<TParam1, TParam2, TContract>
        {
            return BindFactoryInternal<
                TParam1, TParam2, TContract, TFactory, TFactory>();
        }

        public FactoryToChoiceIdBinder<TParam1, TParam2, TContract> BindFactoryCustomInterface<TParam1, TParam2, TContract, TFactoryConcrete, TFactoryContract>()
            where TFactoryConcrete : PlaceholderFactory<TParam1, TParam2, TContract>, TFactoryContract
            where TFactoryContract : IFactory
        {
            return BindFactoryInternal<TParam1, TParam2, TContract, TFactoryContract, TFactoryConcrete>();
        }

        FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TContract> BindFactoryInternal<TParam1, TParam2, TParam3, TContract, TFactoryContract, TFactoryConcrete>()
            where TFactoryConcrete : TFactoryContract, IFactory
            where TFactoryContract : IFactory
        {
            var statement = StartBinding();
            var bindInfo = statement.SpawnBindInfo();

            bindInfo.ContractTypes.Add(typeof(TFactoryContract));

            var factoryBindInfo = new FactoryBindInfo(typeof(TFactoryConcrete));

            statement.SetFinalizer(
                new PlaceholderFactoryBindingFinalizer<TContract>(
                    bindInfo, factoryBindInfo));

            return new FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TContract>(
                this, bindInfo, factoryBindInfo);
        }

        public FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TContract> BindIFactory<TParam1, TParam2, TParam3, TContract>()
        {
            return BindFactoryInternal<
                TParam1, TParam2, TParam3, TContract, IFactory<TParam1, TParam2, TParam3, TContract>, PlaceholderFactory<TParam1, TParam2, TParam3, TContract>>();
        }

        public FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TContract> BindFactory<TParam1, TParam2, TParam3, TContract, TFactory>()
            where TFactory : PlaceholderFactory<TParam1, TParam2, TParam3, TContract>
        {
            return BindFactoryInternal<
                TParam1, TParam2, TParam3, TContract, TFactory, TFactory>();
        }

        public FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TContract> BindFactoryCustomInterface<TParam1, TParam2, TParam3, TContract, TFactoryConcrete, TFactoryContract>()
            where TFactoryConcrete : PlaceholderFactory<TParam1, TParam2, TParam3, TContract>, TFactoryContract
            where TFactoryContract : IFactory
        {
            return BindFactoryInternal<TParam1, TParam2, TParam3, TContract, TFactoryContract, TFactoryConcrete>();
        }

        FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TContract> BindFactoryInternal<TParam1, TParam2, TParam3, TParam4, TContract, TFactoryContract, TFactoryConcrete>()
            where TFactoryConcrete : TFactoryContract, IFactory
            where TFactoryContract : IFactory
        {
            var statement = StartBinding();
            var bindInfo = statement.SpawnBindInfo();

            bindInfo.ContractTypes.Add(typeof(TFactoryContract));

            var factoryBindInfo = new FactoryBindInfo(typeof(TFactoryConcrete));

            statement.SetFinalizer(
                new PlaceholderFactoryBindingFinalizer<TContract>(
                    bindInfo, factoryBindInfo));

            return new FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TContract>(
                this, bindInfo, factoryBindInfo);
        }

        public FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TContract> BindIFactory<TParam1, TParam2, TParam3, TParam4, TContract>()
        {
            return BindFactoryInternal<
                TParam1, TParam2, TParam3, TParam4, TContract, IFactory<TParam1, TParam2, TParam3, TParam4, TContract>, PlaceholderFactory<TParam1, TParam2, TParam3, TParam4, TContract>>();
        }

        public FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TContract> BindFactory<TParam1, TParam2, TParam3, TParam4, TContract, TFactory>()
            where TFactory : PlaceholderFactory<TParam1, TParam2, TParam3, TParam4, TContract>
        {
            return BindFactoryInternal<
                TParam1, TParam2, TParam3, TParam4, TContract, TFactory, TFactory>();
        }

        public FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TContract> BindFactoryCustomInterface<TParam1, TParam2, TParam3, TParam4, TContract, TFactoryConcrete, TFactoryContract>()
            where TFactoryConcrete : PlaceholderFactory<TParam1, TParam2, TParam3, TParam4, TContract>, TFactoryContract
            where TFactoryContract : IFactory
        {
            return BindFactoryInternal<TParam1, TParam2, TParam3, TParam4, TContract, TFactoryContract, TFactoryConcrete>();
        }

        FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TContract> BindFactoryInternal<TParam1, TParam2, TParam3, TParam4, TParam5, TContract, TFactoryContract, TFactoryConcrete>()
            where TFactoryConcrete : TFactoryContract, IFactory
            where TFactoryContract : IFactory
        {
            var statement = StartBinding();
            var bindInfo = statement.SpawnBindInfo();

            bindInfo.ContractTypes.Add(typeof(TFactoryContract));

            var factoryBindInfo = new FactoryBindInfo(typeof(TFactoryConcrete));

            statement.SetFinalizer(
                new PlaceholderFactoryBindingFinalizer<TContract>(
                    bindInfo, factoryBindInfo));

            return new FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TContract>(
                this, bindInfo, factoryBindInfo);
        }

        public FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TContract> BindIFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TContract>()
        {
            return BindFactoryInternal<
                TParam1, TParam2, TParam3, TParam4, TParam5, TContract, IFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TContract>, PlaceholderFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TContract>>();
        }

        public FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TContract> BindFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TContract, TFactory>()
            where TFactory : PlaceholderFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TContract>
        {
            return BindFactoryInternal<
                TParam1, TParam2, TParam3, TParam4, TParam5, TContract, TFactory, TFactory>();
        }

        public FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TContract> BindFactoryCustomInterface<TParam1, TParam2, TParam3, TParam4, TParam5, TContract, TFactoryConcrete, TFactoryContract>()
            where TFactoryConcrete : PlaceholderFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TContract>, TFactoryContract
            where TFactoryContract : IFactory
        {
            return BindFactoryInternal<TParam1, TParam2, TParam3, TParam4, TParam5, TContract, TFactoryContract, TFactoryConcrete>();
        }

        FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TContract> BindFactoryInternal<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TContract, TFactoryContract, TFactoryConcrete>()
            where TFactoryConcrete : TFactoryContract, IFactory
            where TFactoryContract : IFactory
        {
            var statement = StartBinding();
            var bindInfo = statement.SpawnBindInfo();

            bindInfo.ContractTypes.Add(typeof(TFactoryContract));

            var factoryBindInfo = new FactoryBindInfo(typeof(TFactoryConcrete));

            statement.SetFinalizer(
                new PlaceholderFactoryBindingFinalizer<TContract>(
                    bindInfo, factoryBindInfo));

            return new FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TContract>(
                this, bindInfo, factoryBindInfo);
        }

        public FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TContract> BindIFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TContract>()
        {
            return BindFactoryInternal<
                TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TContract, IFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TContract>, PlaceholderFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TContract>>();
        }

        public FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TContract> BindFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TContract, TFactory>()
            where TFactory : PlaceholderFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TContract>
        {
            return BindFactoryInternal<
                TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TContract, TFactory, TFactory>();
        }

        public FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TContract> BindFactoryCustomInterface<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TContract, TFactoryConcrete, TFactoryContract>()
            where TFactoryConcrete : PlaceholderFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TContract>, TFactoryContract
            where TFactoryContract : IFactory
        {
            return BindFactoryInternal<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TContract, TFactoryContract, TFactoryConcrete>();
        }

        FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TContract> BindFactoryInternal<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TContract, TFactoryContract, TFactoryConcrete>()
            where TFactoryConcrete : TFactoryContract, IFactory
            where TFactoryContract : IFactory
        {
            var statement = StartBinding();
            var bindInfo = statement.SpawnBindInfo();

            bindInfo.ContractTypes.Add(typeof(TFactoryContract));

            var factoryBindInfo = new FactoryBindInfo(typeof(TFactoryConcrete));

            statement.SetFinalizer(
                new PlaceholderFactoryBindingFinalizer<TContract>(
                    bindInfo, factoryBindInfo));

            return new FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TContract>(
                this, bindInfo, factoryBindInfo);
        }

        public FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TContract> BindIFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TContract>()
        {
            return BindFactoryInternal<
                TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TContract, IFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TContract>, PlaceholderFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TContract>>();
        }

        public FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TContract> BindFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TContract, TFactory>()
            where TFactory : PlaceholderFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TContract>
        {
            return BindFactoryInternal<
                TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TContract, TFactory, TFactory>();
        }

        public FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TContract> BindFactoryCustomInterface<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TContract, TFactoryConcrete, TFactoryContract>()
            where TFactoryConcrete : PlaceholderFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TContract>, TFactoryContract
            where TFactoryContract : IFactory
        {
            return BindFactoryInternal<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TContract, TFactoryContract, TFactoryConcrete>();
        }

        public T InstantiateExplicit<T>(List<TypeValuePair> extraArgs)
        {
            return (T)InstantiateExplicit(typeof(T), extraArgs);
        }

        public object InstantiateExplicit(Type concreteType, List<TypeValuePair> extraArgs)
        {
            bool autoInject = true;

            return InstantiateExplicit(
                concreteType,
                autoInject,
                extraArgs,
                new InjectContext(this, concreteType, null),
                null);
        }

        public object InstantiateExplicit(Type concreteType, bool autoInject, List<TypeValuePair> extraArgs, InjectContext context, object concreteIdentifier)
        {
#if ZEN_INTERNAL_PROFILING
            using (ProfileTimers.CreateTimedBlock("DiContainer.Instantiate"))
#endif
            {
                if (IsValidating)
                {
                    if (_settings.ValidationErrorResponse == ValidationErrorResponses.Throw)
                    {
                        return InstantiateInternal(concreteType, autoInject, extraArgs, context, concreteIdentifier);
                    }

                    // In this case, just log it and continue to print out multiple validation errors
                    // at once
                    try
                    {
                        return InstantiateInternal(concreteType, autoInject, extraArgs, context, concreteIdentifier);
                    }
                    catch (Exception e)
                    {
                        Log.ErrorException(e);
                        return new ValidationMarker(concreteType, true);
                    }
                }

                return InstantiateInternal(concreteType, autoInject, extraArgs, context, concreteIdentifier);
            }
        }

#if !NOT_UNITY3D
        public Component InstantiateComponentExplicit(
            Type componentType, GameObject gameObject, List<TypeValuePair> extraArgs)
        {
            Assert.That(componentType.DerivesFrom<Component>());

            FlushBindings();

            var monoBehaviour = gameObject.AddComponent(componentType);
            InjectExplicit(monoBehaviour, extraArgs);
            return monoBehaviour;
        }

        public object InstantiateScriptableObjectResourceExplicit(
            Type scriptableObjectType, string resourcePath, List<TypeValuePair> extraArgs)
        {
            var objects = Resources.LoadAll(resourcePath, scriptableObjectType);

            Assert.That(objects.Length > 0,
                "Could not find resource at path '{0}' with type '{1}'", resourcePath, scriptableObjectType);

            Assert.That(objects.Length == 1,
                "Found multiple scriptable objects at path '{0}' when only 1 was expected with type '{1}'", resourcePath, scriptableObjectType);

            var newObj = ScriptableObject.Instantiate(objects.Single());

            InjectExplicit(newObj, extraArgs);

            return newObj;
        }

        // Same as InstantiatePrefabResourceForComponent except allows null values
        // to be included in the argument list.  Also see InjectUtil.CreateArgList
        public object InstantiatePrefabResourceForComponentExplicit(
            Type componentType, string resourcePath, List<TypeValuePair> extraArgs,
            GameObjectCreationParameters creationInfo)
        {
            return InstantiatePrefabResourceForComponentExplicit(
                componentType, resourcePath, extraArgs, new InjectContext(this, componentType, null), null, creationInfo);
        }

        public object InstantiatePrefabResourceForComponentExplicit(
            Type componentType, string resourcePath, List<TypeValuePair> extraArgs, InjectContext context, object concreteIdentifier,
            GameObjectCreationParameters creationInfo)
        {
            var prefab = (GameObject)Resources.Load(resourcePath);
            Assert.IsNotNull(prefab,
                "Could not find prefab at resource location '{0}'".Fmt(resourcePath));
            return InstantiatePrefabForComponentExplicit(
                componentType, prefab, extraArgs, context, concreteIdentifier, creationInfo);
        }

        public object InstantiatePrefabForComponentExplicit(
            Type componentType, UnityEngine.Object prefab,
            List<TypeValuePair> extraArgs)
        {
            return InstantiatePrefabForComponentExplicit(
                componentType, prefab, extraArgs, GameObjectCreationParameters.Default);
        }

        public object InstantiatePrefabForComponentExplicit(
            Type componentType, UnityEngine.Object prefab,
            List<TypeValuePair> extraArgs, GameObjectCreationParameters gameObjectBindInfo)
        {
            return InstantiatePrefabForComponentExplicit(
                componentType, prefab, extraArgs, new InjectContext(this, componentType, null), null, gameObjectBindInfo);
        }

        // Same as InstantiatePrefabForComponent except allows null values
        // to be included in the argument list.  Also see InjectUtil.CreateArgList
        public object InstantiatePrefabForComponentExplicit(
            Type componentType, UnityEngine.Object prefab,
            List<TypeValuePair> extraArgs, InjectContext context, object concreteIdentifier, GameObjectCreationParameters gameObjectBindInfo)
        {
            Assert.That(!AssertOnNewGameObjects,
                "Given DiContainer does not support creating new game objects");

            FlushBindings();

            Assert.That(componentType.IsInterface() || componentType.DerivesFrom<Component>(),
                "Expected type '{0}' to derive from UnityEngine.Component", componentType);

            bool shouldMakeActive;
            var gameObj = CreateAndParentPrefab(prefab, gameObjectBindInfo, context, out shouldMakeActive);

            var component = InjectGameObjectForComponentExplicit(
                gameObj, componentType, extraArgs, context, concreteIdentifier);

            if (shouldMakeActive && !IsValidating)
            {
#if ZEN_INTERNAL_PROFILING
                using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
                {
                    gameObj.SetActive(true);
                }
            }

            return component;
        }
#endif

        ////////////// Execution order ////////////////

        public void BindExecutionOrder<T>(int order)
        {
            BindExecutionOrder(typeof(T), order);
        }

        public void BindExecutionOrder(Type type, int order)
        {
            Assert.That(type.DerivesFrom<ITickable>() || type.DerivesFrom<IInitializable>() || type.DerivesFrom<IDisposable>() || type.DerivesFrom<ILateDisposable>() || type.DerivesFrom<IFixedTickable>() || type.DerivesFrom<ILateTickable>() || type.DerivesFrom<IPoolable>(),
                "Expected type '{0}' to derive from one or more of the following interfaces: ITickable, IInitializable, ILateTickable, IFixedTickable, IDisposable, ILateDisposable", type);

            if (type.DerivesFrom<ITickable>())
            {
                BindTickableExecutionOrder(type, order);
            }

            if (type.DerivesFrom<IInitializable>())
            {
                BindInitializableExecutionOrder(type, order);
            }

            if (type.DerivesFrom<IDisposable>())
            {
                BindDisposableExecutionOrder(type, order);
            }

            if (type.DerivesFrom<ILateDisposable>())
            {
                BindLateDisposableExecutionOrder(type, order);
            }

            if (type.DerivesFrom<IFixedTickable>())
            {
                BindFixedTickableExecutionOrder(type, order);
            }

            if (type.DerivesFrom<ILateTickable>())
            {
                BindLateTickableExecutionOrder(type, order);
            }

            if (type.DerivesFrom<IPoolable>())
            {
                BindPoolableExecutionOrder(type, order);
            }
        }

        public CopyNonLazyBinder BindTickableExecutionOrder<T>(int order)
            where T : ITickable
        {
            return BindTickableExecutionOrder(typeof(T), order);
        }

        public CopyNonLazyBinder BindTickableExecutionOrder(Type type, int order)
        {
            Assert.That(type.DerivesFrom<ITickable>(),
                "Expected type '{0}' to derive from ITickable", type);

            return BindInstance(
                ValuePair.New(type, order)).WhenInjectedInto<TickableManager>();
        }

        public CopyNonLazyBinder BindInitializableExecutionOrder<T>(int order)
            where T : IInitializable
        {
            return BindInitializableExecutionOrder(typeof(T), order);
        }

        public CopyNonLazyBinder BindInitializableExecutionOrder(Type type, int order)
        {
            Assert.That(type.DerivesFrom<IInitializable>(),
                "Expected type '{0}' to derive from IInitializable", type);

            return BindInstance(
                ValuePair.New(type, order)).WhenInjectedInto<InitializableManager>();
        }

        public CopyNonLazyBinder BindDisposableExecutionOrder<T>(int order)
            where T : IDisposable
        {
            return BindDisposableExecutionOrder(typeof(T), order);
        }

        public CopyNonLazyBinder BindLateDisposableExecutionOrder<T>(int order)
            where T : ILateDisposable
        {
            return BindLateDisposableExecutionOrder(typeof(T), order);
        }

        public CopyNonLazyBinder BindDisposableExecutionOrder(Type type, int order)
        {
            Assert.That(type.DerivesFrom<IDisposable>(),
                "Expected type '{0}' to derive from IDisposable", type);

            return BindInstance(
                ValuePair.New(type, order)).WhenInjectedInto<DisposableManager>();
        }

        public CopyNonLazyBinder BindLateDisposableExecutionOrder(Type type, int order)
        {
            Assert.That(type.DerivesFrom<ILateDisposable>(),
            "Expected type '{0}' to derive from ILateDisposable", type);

            return BindInstance(
                ValuePair.New(type, order)).WithId("Late").WhenInjectedInto<DisposableManager>();
        }

        public CopyNonLazyBinder BindFixedTickableExecutionOrder<T>(int order)
            where T : IFixedTickable
        {
            return BindFixedTickableExecutionOrder(typeof(T), order);
        }

        public CopyNonLazyBinder BindFixedTickableExecutionOrder(Type type, int order)
        {
            Assert.That(type.DerivesFrom<IFixedTickable>(),
                "Expected type '{0}' to derive from IFixedTickable", type);

            return Bind<ValuePair<Type, int>>().WithId("Fixed")
                .FromInstance(ValuePair.New(type, order)).WhenInjectedInto<TickableManager>();
        }

        public CopyNonLazyBinder BindLateTickableExecutionOrder<T>(int order)
            where T : ILateTickable
        {
            return BindLateTickableExecutionOrder(typeof(T), order);
        }

        public CopyNonLazyBinder BindLateTickableExecutionOrder(Type type, int order)
        {
            Assert.That(type.DerivesFrom<ILateTickable>(),
                "Expected type '{0}' to derive from ILateTickable", type);

            return Bind<ValuePair<Type, int>>().WithId("Late")
                .FromInstance(ValuePair.New(type, order)).WhenInjectedInto<TickableManager>();
        }

        public CopyNonLazyBinder BindPoolableExecutionOrder<T>(int order)
            where T : IPoolable
        {
            return BindPoolableExecutionOrder(typeof(T), order);
        }

        public CopyNonLazyBinder BindPoolableExecutionOrder(Type type, int order)
        {
            Assert.That(type.DerivesFrom<IPoolable>(),
                "Expected type '{0}' to derive from IPoolable", type);

            return Bind<ValuePair<Type, int>>()
                .FromInstance(ValuePair.New(type, order)).WhenInjectedInto<PoolableManager>();
        }

        class ProviderInfo
        {
            public ProviderInfo(
                IProvider provider, BindingCondition condition, bool nonLazy, DiContainer container)
            {
                Provider = provider;
                Condition = condition;
                NonLazy = nonLazy;
                Container = container;
            }

            public readonly DiContainer Container;
            public readonly bool NonLazy;
            public readonly IProvider Provider;
            public readonly BindingCondition Condition;
        }
    }
}
