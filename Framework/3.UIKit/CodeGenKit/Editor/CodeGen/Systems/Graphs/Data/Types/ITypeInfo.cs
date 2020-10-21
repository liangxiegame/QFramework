using QFramework.CodeGen;
using Invert.Data;

public interface ITypeInfo : IItem, IValueItem
{
    bool HasAttribute();
}