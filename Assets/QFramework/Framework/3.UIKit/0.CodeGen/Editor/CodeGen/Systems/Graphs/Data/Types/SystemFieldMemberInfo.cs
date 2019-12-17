using System.Reflection;

public class SystemFieldMemberInfo : IMemberInfo
{
    private FieldInfo FieldInfo;

    public SystemFieldMemberInfo(FieldInfo fieldInfo)
    {
        FieldInfo = fieldInfo;
    }


    public ITypeInfo MemberType
    {
        get
        {
            return new SystemTypeInfo(FieldInfo.FieldType);
        }
    }
}