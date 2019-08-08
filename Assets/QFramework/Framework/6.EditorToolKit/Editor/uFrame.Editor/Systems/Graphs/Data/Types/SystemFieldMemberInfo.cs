using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class SystemFieldMemberInfo : IMemberInfo
{
    private FieldInfo FieldInfo;

    public SystemFieldMemberInfo(FieldInfo fieldInfo)
    {
        FieldInfo = fieldInfo;
    }

    public string MemberName { get { return FieldInfo.Name; } }

    public ITypeInfo MemberType
    {
        get
        {
            return new SystemTypeInfo(FieldInfo.FieldType);
        }
    }
    public IEnumerable<Attribute> GetAttributes()
    {
       return FieldInfo.GetCustomAttributes(true).OfType<Attribute>();
    }
}