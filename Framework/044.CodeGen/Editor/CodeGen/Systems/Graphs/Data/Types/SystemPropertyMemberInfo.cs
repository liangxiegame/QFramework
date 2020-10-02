using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class SystemPropertyMemberInfo : IMemberInfo
{
    private PropertyInfo PropertyInfo;

    public SystemPropertyMemberInfo(PropertyInfo propertyInfo)
    {
        PropertyInfo = propertyInfo;
    }

    public string MemberName { get { return PropertyInfo.Name; } }

    public ITypeInfo MemberType
    {
        get
        {
            return new SystemTypeInfo(PropertyInfo.PropertyType);
        }
    }

    public IEnumerable<Attribute> GetAttributes()
    {
        return PropertyInfo.GetCustomAttributes(true).OfType<Attribute>();
    }
}
public class SystemMethodParameterInfo : IMemberInfo
{
    private ParameterInfo ParameterInfo;

    public SystemMethodParameterInfo(ParameterInfo methodInfo)
    {
        ParameterInfo = methodInfo;
    }

    public string MemberName { get { return ParameterInfo.Name; } }

    public ITypeInfo MemberType
    {
        get
        {
            return new SystemTypeInfo(ParameterInfo.ParameterType);
        }
    }

    public IEnumerable<Attribute> GetAttributes()
    {
        return ParameterInfo.GetCustomAttributes(true).OfType<Attribute>();
    }

}
public class SystemMethodMemberInfo : IMethodMemberInfo
{
    private MethodInfo MethodInfo;

    public SystemMethodMemberInfo(MethodInfo methodInfo)
    {
        MethodInfo = methodInfo;
    }

    public string MemberName { get { return MethodInfo.Name; } }

    public ITypeInfo MemberType
    {
        get
        {
            return new SystemTypeInfo(MethodInfo.ReturnType);
        }
    }

    public IEnumerable<Attribute> GetAttributes()
    {
        return MethodInfo.GetCustomAttributes(true).OfType<Attribute>();
    }


    public string MethodIdentifier
    {
        get
        {
            return string.Format("{0}({1})", MemberName,
                string.Join(",",GetParameters().Select(p => p.MemberType.TypeName).ToArray()));
        }
    }


    public IEnumerable<IMemberInfo> GetParameters()
    {
        foreach (var item in MethodInfo.GetParameters())
        {
            yield return new SystemMethodParameterInfo(item);
        }
    }
}