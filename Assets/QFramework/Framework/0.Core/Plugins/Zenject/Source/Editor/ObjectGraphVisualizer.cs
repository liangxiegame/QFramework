using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModestTree;

namespace Zenject
{
    // Responsibilities:
    // - Output a file specifying the full object graph for a given root dependency
    // - This file uses the DOT language with can be fed into GraphViz to generate an image
    // - http://www.graphviz.org/
    public static class ObjectGraphVisualizer
    {
        public static void OutputObjectGraphToFile(
            DiContainer container, string outputPath,
            IEnumerable<Type> externalIgnoreTypes, IEnumerable<Type> contractTypes)
        {
            // Output the entire object graph to file
            var graph = CalculateObjectGraph(container, contractTypes);

            var ignoreTypes = new List<Type>
            {
                typeof(DiContainer),
                typeof(InitializableManager)
            };

            ignoreTypes.AddRange(externalIgnoreTypes);

            var resultStr = "digraph { \n";

            resultStr += "rankdir=LR;\n";

            foreach (var entry in graph)
            {
                if (ShouldIgnoreType(entry.Key, ignoreTypes))
                {
                    continue;
                }

                foreach (var dependencyType in entry.Value)
                {
                    if (ShouldIgnoreType(dependencyType, ignoreTypes))
                    {
                        continue;
                    }

                    resultStr += GetFormattedTypeName(entry.Key) + " -> " + GetFormattedTypeName(dependencyType) + "; \n";
                }
            }

            resultStr += " }";

            File.WriteAllText(outputPath, resultStr);
        }

        static bool ShouldIgnoreType(Type type, List<Type> ignoreTypes)
        {
            return ignoreTypes.Contains(type);
        }

        static Dictionary<Type, List<Type>> CalculateObjectGraph(
            DiContainer container, IEnumerable<Type> contracts)
        {
            var map = new Dictionary<Type, List<Type>>();

            foreach (var contractType in contracts)
            {
                var depends = GetDependencies(container, contractType);

                if (depends.Any())
                {
                    map.Add(contractType, depends);
                }
            }

            return map;
        }

        static List<Type> GetDependencies(
            DiContainer container, Type type)
        {
            var dependencies = new List<Type>();

            foreach (var contractType in container.GetDependencyContracts(type))
            {
                List<Type> dependTypes;

                if (contractType.FullName.StartsWith("System.Collections.Generic.List"))
                {
                    var subTypes = contractType.GenericArguments();
                    Assert.IsEqual(subTypes.Length, 1);

                    var subType = subTypes[0];
                    dependTypes = container.ResolveTypeAll(subType);
                }
                else
                {
                    dependTypes = container.ResolveTypeAll(contractType);
                    Assert.That(dependTypes.Count <= 1);
                }

                foreach (var dependType in dependTypes)
                {
                    dependencies.Add(dependType);
                }
            }

            return dependencies;
        }

        static string GetFormattedTypeName(Type type)
        {
            var str = type.PrettyName();

            // GraphViz does not read names with <, >, or . characters so replace them
            str = str.Replace(">", "_");
            str = str.Replace("<", "_");
            str = str.Replace(".", "_");

            return str;
        }
    }
}

