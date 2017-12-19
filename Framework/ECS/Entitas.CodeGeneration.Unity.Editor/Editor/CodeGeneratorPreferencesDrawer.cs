using System;
using System.Collections.Generic;
using System.Linq;

using QFramework.Unity.Editor;

using UnityEditor;
using UnityEngine;


namespace QFramework.CodeGeneration.Unity.Editor
{
    public class CodeGeneratorPreferencesDrawer : AbstractPreferencesDrawer
    {
        public override int Priority
        {
            get { return 10; }
        }

        public override string Title
        {
            get { return "Code Generator"; }
        }

        string[] mAvailableDataProviderTypes;
        string[] mAvailableGeneratorTypes;
        string[] mAvailablePostProcessorTypes;

        string[] mAvailableDataProviderNames;
        string[] mAvailableGeneratorNames;
        string[] mAvailablePostProcessorNames;

        Properties mProperties;
        Type[] mTypes;

        CodeGeneratorConfig mCodeGeneratorConfig;

        public override void Initialize(Properties properties)
        {
            mProperties = properties;
            mCodeGeneratorConfig = new CodeGeneratorConfig();
            properties.AddProperties(mCodeGeneratorConfig.DefaultProperties, false);
            mCodeGeneratorConfig.Configure(properties);

            mTypes = CodeGeneratorUtil.LoadTypesFromPlugins(properties);

            InitPhase<ICodeGeneratorDataProvider>(mTypes, out mAvailableDataProviderTypes,
                out mAvailableDataProviderNames);
            InitPhase<ICodeGenerator>(mTypes, out mAvailableGeneratorTypes, out mAvailableGeneratorNames);
            InitPhase<ICodeGenFilePostProcessor>(mTypes, out mAvailablePostProcessorTypes,
                out mAvailablePostProcessorNames);

            mProperties.AddProperties(GetConfigurables(), false);
        }

        protected override void DrawContent(Properties properties)
        {
            mCodeGeneratorConfig.DataProviders = DrawMaskField("Data Providers", mAvailableDataProviderTypes,
                mAvailableDataProviderNames, mCodeGeneratorConfig.DataProviders);
            mCodeGeneratorConfig.CodeGenerators = DrawMaskField("Code Generators", mAvailableGeneratorTypes,
                mAvailableGeneratorNames, mCodeGeneratorConfig.CodeGenerators);
            mCodeGeneratorConfig.PostProcessors = DrawMaskField("Post Processors", mAvailablePostProcessorTypes,
                mAvailablePostProcessorNames, mCodeGeneratorConfig.PostProcessors);

            EditorGUILayout.Space();
            drawConfigurables();

            DrawGenerateButton();
        }

        Dictionary<string, string> GetConfigurables()
        {
            return CodeGeneratorUtil.GetConfigurables(
                CodeGeneratorUtil.GetUsed<ICodeGeneratorDataProvider>(mTypes, mCodeGeneratorConfig.DataProviders),
                CodeGeneratorUtil.GetUsed<ICodeGenerator>(mTypes, mCodeGeneratorConfig.CodeGenerators),
                CodeGeneratorUtil.GetUsed<ICodeGenFilePostProcessor>(mTypes, mCodeGeneratorConfig.PostProcessors)
            );
        }

        void drawConfigurables()
        {
            var configurables = GetConfigurables();
            mProperties.AddProperties(configurables, false);

            foreach (var kv in configurables.OrderBy(kv => kv.Key))
            {
                mProperties[kv.Key] =
                    EditorGUILayout.TextField(kv.Key.ShortTypeName().ToSpacedCamelCase(), mProperties[kv.Key]);
            }
        }

        static string[] InitPhase<T>(Type[] types, out string[] availableTypes, out string[] availableNames)
            where T : ICodeGeneratorInterface
        {
            IEnumerable<T> instances = CodeGeneratorUtil.GetOrderedInstances<T>(types);

            availableTypes = instances
                .Select(instance => instance.GetType().ToCompilableString())
                .ToArray();

            availableNames = instances
                .Select(instance => instance.Name)
                .ToArray();

            return instances
                .Where(instance => instance.IsEnabledByDefault)
                .Select(instance => instance.GetType().ToCompilableString())
                .ToArray();
        }

        static string[] DrawMaskField(string title, string[] types, string[] names, string[] input)
        {
            var mask = 0;

            for (int i = 0; i < types.Length; i++)
            {
                if (input.Contains(types[i]))
                {
                    mask += (1 << i);
                }
            }

            mask = EditorGUILayout.MaskField(title, mask, names);

            var selected = new List<string>();
            for (int i = 0; i < types.Length; i++)
            {
                var index = 1 << i;
                if ((index & mask) == index)
                {
                    selected.Add(types[i]);
                }
            }

            return selected.ToArray();
        }

        void DrawGenerateButton()
        {
            var bgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Generate", GUILayout.Height(32)))
            {
                UnityCodeGenerator.Generate();
            }
            GUI.backgroundColor = bgColor;
        }
    }
}