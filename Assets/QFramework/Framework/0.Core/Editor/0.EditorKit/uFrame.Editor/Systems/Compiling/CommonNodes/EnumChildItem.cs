using System;
using System.Collections.Generic;
using QFramework.GraphDesigner;

public class EnumChildItem : GenericNodeChildItem, IMemberInfo
{
    public string MemberName { get { return this.Name; }}
    public ITypeInfo MemberType { get { return new SystemTypeInfo(typeof(int)); } }
    public IEnumerable<Attribute> GetAttributes()
    {
        yield break;
    }
}