using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace Invert.Common.Utilities
{
    public static class ProjectInfo
    {
        public static List<MemberInfo> CachedMemberInfos;
        public static List<Type> CachedTypeInfos ;
     
        
        public static IEnumerable<MemberInfo> TypeMemberSearch(string searchText,Func<Type,bool> memberTypeFilter)
        {
            if (CachedMemberInfos == null)
            {
                CachedMemberInfos = new List<MemberInfo>();
                var allowed = new string[]
                {
                    "System.Core",
                    "mscorlib",
                    "UBehaviours",
                    "Assembly-CSharp",
                    "UnityEngine"
                };
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                CachedTypeInfos = new List<Type>();
                foreach (var item in assemblies)
                {
                    if (!allowed.Any(p => item.FullName.Contains(p)))
                        continue;

                    var types = item.GetTypes();
                    foreach (var type in types)
                    {
                       // if (type.IsSubclassOf(typeof(MonoBehaviour)))
                        CachedTypeInfos.Add(type);
                        var members = type.GetMembers(BindingFlags.Public | BindingFlags.Instance);
                        foreach (var propertyInfo in members)
                            if (propertyInfo is PropertyInfo || propertyInfo is FieldInfo)
                            {
                                var memberType = (propertyInfo is PropertyInfo
                                    ? ((PropertyInfo) propertyInfo).PropertyType
                                    : ((FieldInfo) propertyInfo).FieldType);
                                if (memberTypeFilter(memberType))
                                CachedMemberInfos.Add(propertyInfo);
                            }
                                
                    }
                }
            }
            //var reg = new Regex(searchText, RegexOptions.IgnoreCase);

            foreach (var cachedMemberInfo in CachedMemberInfos)
            {
                if (cachedMemberInfo.DeclaringType != null)
                {
                    var search = string.Format("{0}.{1}", cachedMemberInfo.DeclaringType.Name.ToUpper(), cachedMemberInfo.Name.ToUpper());
                    if (search.StartsWith(searchText.ToUpper()))
                    {
                        yield return cachedMemberInfo;
                    }
                }
            }
        }

    }
}
