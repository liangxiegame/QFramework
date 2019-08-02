using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QF.GraphDesigner;

public class SystemTypeInfo : ITypeInfo
{
    private Type _systemType;

    public Type SystemType
    {
        get { return _systemType ?? typeof(void); }
        set { _systemType = value; }
    }

    public ITypeInfo Other { get; set; }
    public SystemTypeInfo(Type systemType)
    {
        SystemType = systemType;
    }

    public bool IsArray
    {
        get
        {
            if (Other != null)
            {
                return Other.IsArray;
            }
            return SystemType.IsArray;
        }
    }

    public bool IsList
    {
        get { return typeof (IList).IsAssignableFrom(SystemType); }
    }
    public static implicit operator SystemTypeInfo(Type a)
    {
        return new SystemTypeInfo(a);
    }
  
    public bool IsEnum
    {
        get
        {
            if (Other != null)
            {
                return Other.IsEnum;
            }
            return SystemType.IsEnum;
        }
    }

    public ITypeInfo InnerType
    {
        get
        {
            var genericType = SystemType.GetGenericArguments().FirstOrDefault();
            if (genericType != null)
            {
                return new SystemTypeInfo(genericType);
            }
            return null;
        }
    }

    public string TypeName
    {
        get
        {
            if (Other != null)
            {
                return Other.TypeName;
            }
            return SystemType.Name;
        }
    }

    public virtual string FullName
    {
        get
        {
            if (Other != null)
            {
                return Other.FullName;
            }
            return SystemType.FullName;
        }
    }

    public string Namespace
    {
        get
        {
            if (Other != null)
            {
                return Other.Namespace;
            }
            return SystemType.Namespace;
        }
    }

    public virtual IEnumerable<IMemberInfo> GetMembers()
    {

        if (SystemType != null)
        {
            if (IsEnum)
            {
                foreach (var item in SystemType.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    if (item == null) continue;
                    yield return new SystemFieldMemberInfo(item);
                }
            }
            else
            {

                foreach (var item in SystemType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (item == null) continue;
                    yield return new SystemFieldMemberInfo(item);
                }
                foreach (var item in SystemType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    yield return new SystemPropertyMemberInfo(item);
                }
                foreach (var item in SystemType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (item.IsSpecialName && (item.Name.StartsWith("set_") || item.Name.StartsWith("get_"))) continue;
                    yield return new SystemMethodMemberInfo(item);
                }
            }
         
        }

        
    }

    public bool IsAssignableTo(ITypeInfo info)
	{
        var systemInfo = info as SystemTypeInfo;
        if (systemInfo != null)
        {
            return systemInfo.SystemType.IsAssignableFrom(SystemType) || systemInfo.SystemType.IsCastableTo(SystemType);
        }
        return info.FullName == FullName;
    }

    public ITypeInfo BaseTypeInfo
    {
        get
        {
            if (SystemType.BaseType == typeof (object)) return null;
            return (SystemTypeInfo) SystemType.BaseType;
        }
    }

    public bool HasAttribute(Type attribute)
    {
        return SystemType.IsDefined(attribute, true);
    }

    public virtual string Title { get { return TypeName; } }
    public virtual string Group { get { return Namespace; } }
    public virtual string SearchTag { get { return FullName; } }
    public virtual  string Description { get; set; }
    public virtual string Identifier { get {return FullName;} set {}}
}