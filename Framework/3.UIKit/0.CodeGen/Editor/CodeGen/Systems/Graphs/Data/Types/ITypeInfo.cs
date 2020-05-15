using System;
using System.Collections.Generic;
using QFramework.CodeGen;
using Invert.Data;

public interface ITypeInfo : IItem, IValueItem
{
    bool IsArray { get; }
    bool IsEnum { get; }
    string TypeName { get; }
    string FullName { get; }
    string Namespace { get; }
    IEnumerable<IMemberInfo> GetMembers();

    bool HasAttribute(Type attribute);
}