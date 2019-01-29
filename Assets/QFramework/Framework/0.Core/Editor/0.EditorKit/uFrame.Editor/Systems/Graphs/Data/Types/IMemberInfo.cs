using System;
using System.Collections.Generic;

public interface IMemberInfo
{
    string MemberName { get;  }
    ITypeInfo MemberType { get; }
    IEnumerable<Attribute> GetAttributes();
}

public interface IMethodMemberInfo : IMemberInfo
{
    string MethodIdentifier { get; }
    
    IEnumerable<IMemberInfo> GetParameters();
}
