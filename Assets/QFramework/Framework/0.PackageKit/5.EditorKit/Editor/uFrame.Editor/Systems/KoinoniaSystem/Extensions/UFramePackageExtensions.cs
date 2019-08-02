using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QF.GraphDesigner.Unity.KoinoniaSystem.Data;

namespace QF.GraphDesigner.Unity.KoinoniaSystem.Extensions
{
    public static class UFramePackageExtensions
    {
        public static UFramePackageDescriptor GetDescriptor()
        {
            return null;
        }

        public static System.Type[] GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type aType)
        {
            var result = new System.Collections.Generic.List<Type>();
            var assemblies = aAppDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(aType))
                        result.Add(type);
                }
            }
            return result.ToArray();
        }

    }
}
