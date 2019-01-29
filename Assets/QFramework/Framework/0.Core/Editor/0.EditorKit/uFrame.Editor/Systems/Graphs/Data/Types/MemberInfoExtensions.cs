using System.Linq;

public static class MemberInfoExtensions
{
    public static TAttribute GetAttribute<TAttribute>(this IMemberInfo memberInfo)
    {
        return memberInfo.GetAttributes().OfType<TAttribute>().FirstOrDefault();
    }
    public static bool HasAttribute<TAttribute>(this IMemberInfo memberInfo)
    {
        return memberInfo.GetAttributes().OfType<TAttribute>().Any();
    }
}