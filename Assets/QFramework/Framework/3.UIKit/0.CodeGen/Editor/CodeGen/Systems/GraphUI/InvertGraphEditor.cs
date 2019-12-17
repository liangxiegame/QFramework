using System;
using System.Collections.Generic;
using System.Linq;
using Invert.Data;
using QFramework;

namespace QFramework.CodeGen
{
    /// <summary>
    /// 这个是 uFrame 中的主要框架
    /// </summary>
    public static class InvertGraphEditor
    {
        private static IConnectionStrategy[] mConnectionStrategies;


        private static QFrameworkContainer mTypesContainer;

        private static IQFrameworkContainer mUiContainer;


        public static IConnectionStrategy[] ConnectionStrategies
        {
            get
            {
                return mConnectionStrategies ??
                       (mConnectionStrategies = Container.ResolveAll<IConnectionStrategy>().ToArray());
            }
            set { mConnectionStrategies = value; }
        }

        public static IQFrameworkContainer Container
        {
            get { return InvertApplication.Container; }
        }




        public static IEnumerable<OutputGenerator> GetAllCodeGenerators(IGraphConfiguration graphConfiguration,
            IDataRecord[] items, bool includeDisabled = false)
        {
            var graphItemGenerators = Container.ResolveAll<DesignerGeneratorFactory>().ToArray();

            foreach (var outputGenerator in GetAllCodeGeneratorsForItems(graphConfiguration, graphItemGenerators, items,
                includeDisabled))
                yield return outputGenerator;
        }

        private static IEnumerable<OutputGenerator> GetAllCodeGeneratorsForItems(IGraphConfiguration graphConfiguration,
            DesignerGeneratorFactory[] graphItemGenerators, IDataRecord[] items, bool includeDisabled = false)
        {
            foreach (var graphItemGenerator in graphItemGenerators)
            {
                DesignerGeneratorFactory generator = graphItemGenerator;
                // If its a generator for the entire diagram
                foreach (var item in items)
                {
                    if (generator.DiagramItemType.IsAssignableFrom(item.GetType()))
                    {
                        var codeGenerators = generator.GetGenerators(graphConfiguration, item);
                        foreach (var codeGenerator in codeGenerators)
                        {
                            if (!includeDisabled && !codeGenerator.IsValid()) continue;
                            // TODO Had to remove this?
                            //if (!codeGenerator.IsEnabled(prsteroject)) continue;

                            codeGenerator.AssetDirectory = graphConfiguration.CodeOutputPath;
                            //codeGenerator.Settings = settings;
                            if (codeGenerator.ObjectData == null)
                                codeGenerator.ObjectData = item;
                            codeGenerator.GeneratorFor = graphItemGenerator.DiagramItemType;
                            yield return codeGenerator;
                        }
                    }
                }
            }
        }


        public static bool IsFilter(Type type)
        {
            return FilterExtensions.AllowedFilterNodes.ContainsKey(type);
        }
    }
}