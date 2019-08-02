using System;
using System.Collections.Generic;
using QF.GraphDesigner;
using Invert.Data;

public interface ITypeInfo : IItem, IValueItem
{
    bool IsArray { get; }
    bool IsList { get; }
    bool IsEnum { get; }
    ITypeInfo InnerType { get; }
    string TypeName { get; }
    string FullName { get; }
    string Namespace { get; }
    IEnumerable<IMemberInfo> GetMembers();
    bool IsAssignableTo(ITypeInfo info);
    ITypeInfo BaseTypeInfo { get; }

    bool HasAttribute(Type attribute);
}

public static class TypeInfoExtensions
{
    public static IEnumerable<IMemberInfo> GetAllMembers(this ITypeInfo typeInfo)
    {
        if (typeInfo == typeof (void)) yield break;
        foreach (var item in typeInfo.GetMembers())
        {
            yield return item;
        }
        var baseType = typeInfo.BaseTypeInfo;

        if (baseType != null && baseType != typeInfo)
        {
            foreach (var item in baseType.GetAllMembers())
            {
                yield return item;
            }
        }

    }
}