public interface IMemberInfo
{
    ITypeInfo MemberType { get; }
}

public interface IMethodMemberInfo : IMemberInfo
{
}
