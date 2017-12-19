using System;
using System.Collections.Generic;
using System.Linq;
using QFramework.CodeGeneration.Attributes;




namespace QFramework {

    public class ComponentDataProvider : ICodeGeneratorDataProvider, IConfigurable {

        public string Name { get { return "Component"; } }
        public int Priority { get { return 0; } }
        public bool IsEnabledByDefault { get { return true; } }
        public bool RunInDryMode { get { return true; } }

        public Dictionary<string, string> DefaultProperties {
            get {
                var dataProviderProperties = _dataProviders
                    .OfType<IConfigurable>()
                    .Select(i => i.DefaultProperties)
                    .ToArray();

                return _assembliesConfig.DefaultProperties
                       .Merge(_contextsComponentDataProvider.DefaultProperties)
                       .Merge(dataProviderProperties);
            }
        }

        readonly CodeGeneratorConfig _codeGeneratorConfig = new CodeGeneratorConfig();
        readonly AssembliesConfig _assembliesConfig = new AssembliesConfig();
        readonly ContextsComponentDataProvider _contextsComponentDataProvider = new ContextsComponentDataProvider();

        static IComponentDataProvider[] getComponentDataProviders() {
            return new IComponentDataProvider[] {
                new ComponentTypeComponentDataProvider(),
                new MemberDataComponentDataProvider(),
                new ContextsComponentDataProvider(),
                new IsUniqueComponentDataProvider(),
                new CustomPrefixComponentDataProvider(),
                new ShouldGenerateComponentComponentDataProvider(),
                new ShouldGenerateMethodsComponentDataProvider(),
                new ShouldGenerateComponentIndexComponentDataProvider()
            };
        }

        Type[] _types;
        IComponentDataProvider[] _dataProviders;

        public ComponentDataProvider() : this(null) {
        }

        public ComponentDataProvider(Type[] types) : this(types, getComponentDataProviders()) {
        }

        protected ComponentDataProvider(Type[] types, IComponentDataProvider[] dataProviders) {
            _types = types;
            _dataProviders = dataProviders;
        }

        public void Configure(Properties properties) {
            _codeGeneratorConfig.Configure(properties);
            _assembliesConfig.Configure(properties);
            foreach (var dataProvider in _dataProviders.OfType<IConfigurable>()) {
                dataProvider.Configure(properties);
            }
            _contextsComponentDataProvider.Configure(properties);
        }

        public CodeGeneratorData[] GetData() {
            if (_types == null) {
                _types = PluginUtil
                    .GetAssembliesResolver(_assembliesConfig.assemblies, _codeGeneratorConfig.SearchPaths)
                    .GetTypes();
            }

            var dataFromComponents = _types
                .Where(type => type.ImplementsInterface<IComponent>())
                .Where(type => !type.IsAbstract)
                .Select(type => createDataForComponent(type));

            var dataFromNonComponents = _types
                .Where(type => !type.ImplementsInterface<IComponent>())
                .Where(type => !type.IsGenericType)
                .Where(type => hasContexts(type))
                .SelectMany(type => createDataForNonComponent(type));

            var generatedComponentsLookup = dataFromNonComponents.ToLookup(data => data.GetFullTypeName());
            return dataFromComponents
                .Where(data => !generatedComponentsLookup.Contains(data.GetFullTypeName()))
                .Concat(dataFromNonComponents)
                .ToArray();
        }

        ComponentData createDataForComponent(Type type) {
            var data = new ComponentData();
            foreach (var provider in _dataProviders) {
                provider.Provide(type, data);
            }

            return data;
        }

        ComponentData[] createDataForNonComponent(Type type) {
            return getComponentNames(type)
                .Select(componentName => {
                    var data = createDataForComponent(type);
                    data.SetFullTypeName(componentName.AddComponentSuffix());
                    data.SetMemberData(new [] {
                        new MemberData(type.ToCompilableString(), "value")
                    });

                    return data;
                }).ToArray();
        }

        bool hasContexts(Type type) {
            return _contextsComponentDataProvider.GetContextNames(type).Length != 0;
        }

        string[] getComponentNames(Type type) {
            var attr = Attribute
                .GetCustomAttributes(type)
                .OfType<CustomComponentNameAttribute>()
                .SingleOrDefault();

            if (attr == null) {
                return new [] { type.ToCompilableString().ShortTypeName().AddComponentSuffix() };
            }

            return attr.componentNames;
        }
    }
}
