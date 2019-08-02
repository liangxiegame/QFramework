using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RSG.Utils
{
    /// <summary>
    /// Misc helpers for working with C# reflection.
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// Return only attributes that match the specified type.
        /// </summary>
        private static IEnumerable<object> MatchingAttributes(IEnumerable<object> attributes, Type attributeType)
        {
            return attributes.Where(attribute => attributeType.IsAssignableFrom(attribute.GetType()));
        }

        /// <summary>
        /// Retrieve an attribute by name from a particular type.
        /// </summary>
        public static AttributeT GetAttribute<AttributeT>(Type type)
            where AttributeT : Attribute
        {
            Argument.NotNull(() => type);

            return (AttributeT)MatchingAttributes(type.GetCustomAttributes(false), typeof(AttributeT)).FirstOrDefault();
        }

        /// <summary>
        /// Retrieve attributes by name from a particular type.
        /// </summary>
        public static IEnumerable<AttributeT> GetAttributes<AttributeT>(Type type)
            where AttributeT : Attribute
        {
            Argument.NotNull(() => type);

            return MatchingAttributes(type.GetCustomAttributes(false), typeof(AttributeT)).Cast<AttributeT>();
        }

        /// <summary>
        /// Retreive an attribute from the property, or null if the attribute doesn't exist.
        /// </summary>
        public static Attribute GetPropertyAttribute(PropertyInfo property, Type attributeType)
        {
            Argument.NotNull(() => property);
            Argument.NotNull(() => attributeType);

            return (Attribute)MatchingAttributes(property.GetCustomAttributes(true), attributeType).FirstOrDefault();
        }

        /// <summary>
        /// Retreive an attribute from the property, or null if the attribute doesn't exist.
        /// </summary>
        public static AttributeT GetPropertyAttribute<AttributeT>(PropertyInfo property) 
            where AttributeT : Attribute
        {
            Argument.NotNull(() => property);

            return (AttributeT)GetPropertyAttribute(property, typeof(AttributeT));
        }

        /// <summary>
        /// Determine if a property has a particular attribute.
        /// </summary>
        public static bool PropertyHasAttribute(PropertyInfo property, Type attributeType)
        {
            Argument.NotNull(() => property);
            Argument.NotNull(() => attributeType);

            return GetPropertyAttribute(property, attributeType) != null;
        }

        /// <summary>
        /// Determine if a property has a particular attribute.
        /// </summary>
        public static bool PropertyHasAttribute<AttributeT>(PropertyInfo property)
            where AttributeT : Attribute
        {
            Argument.NotNull(() => property);

            return PropertyHasAttribute(property, typeof(AttributeT));
        }


        /// <summary>
        /// Retreive an attribute from the method, or null if the attribute doesn't exist.
        /// </summary>
        public static Attribute GetMethodAttribute(MethodInfo method, Type attributeType)
        {
            Argument.NotNull(() => method);
            Argument.NotNull(() => attributeType);

            return (Attribute)MatchingAttributes(method.GetCustomAttributes(true), attributeType).FirstOrDefault();
        }

        /// <summary>
        /// Retreive an attribute from the method, or null if the attribute doesn't exist.
        /// </summary>
        public static AttributeT GetMethodAttribute<AttributeT>(MethodInfo method)
            where AttributeT : Attribute
        {
            Argument.NotNull(() => method);

            return (AttributeT)GetMethodAttribute(method, typeof(AttributeT));
        }

        /// <summary>
        /// Determine if a method has a particular attribute.
        /// </summary>
        public static bool MethodHasAttribute(MethodInfo method, Type attributeType)
        {
            Argument.NotNull(() => method);
            Argument.NotNull(() => attributeType);

            return GetMethodAttribute(method, attributeType) != null;
        }

        /// <summary>
        /// Determine if a method has a particular attribute.
        /// </summary>
        public static bool MethodHasAttribute<AttributeT>(MethodInfo method)
            where AttributeT : Attribute
        {
            Argument.NotNull(() => method);

            return MethodHasAttribute(method, typeof(AttributeT));
        }

        /// <summary>
        /// Determine if a particular type has a particular attribute.
        /// </summary>
        public static bool TypeHasAttribute(Type type, Type attributeType)
        {
            Argument.NotNull(() => type);
            Argument.NotNull(() => attributeType);            

            return MatchingAttributes(type.GetCustomAttributes(false), attributeType).Any();
        }


        /// <summary>
        /// Determine if a particular type has a particular attribute.
        /// </summary>
        public static bool TypeHasAttribute<AttributeT>(Type type)
            where AttributeT : Attribute
        {
            Argument.NotNull(() => type);

            return TypeHasAttribute(type, typeof(AttributeT));
        }

        private static readonly HashSet<string> assembliesToIgnore = new HashSet<string>
        {
            "UnityEditor",
            "Moq",
        };
           
        /// <summary>
        /// Returns true to ignore a paritcular assembly.
        /// </summary>
        private static bool IgnoreAssembly(Assembly assembly, Predicate<string> ignoreFilter)
        {
            try
            {
                if (assembly.GlobalAssemblyCache)
                {
                    // Ignore system assemblies by default.
                    return true;
                }

                if (assembly.ManifestModule.Name == "<In Memory Module>" ||  // MS
                    assembly.ManifestModule.Name == "Default Dynamic Module") // Mono
                {
                    // Ignore dynamically generated assemblies.
                    return true;
                }

                if (assembly.Location.EndsWith(".vshost.exe"))
                {
                    // Ignore VS generated assemblies.
                    return true;
                }

                // Ignore specific assemblies.
                var assemblyName = Path.GetFileNameWithoutExtension(assembly.Location);
                if (assembliesToIgnore.Contains(assemblyName))
                {
                    return true;
                }

                // Ignore assemblies specified by caller.
                if (ignoreFilter != null && ignoreFilter(assemblyName))
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to assess Assembly for ignoring:\n" + assembly.ManifestModule.Name, ex);
            }            
        }

        /// <summary>
        /// Return an enumerable of all known types.
        /// </summary>
        public static IEnumerable<Type> GetAllTypes()
        {
            return GetAllTypes(null);
        }

        /// <summary>
        /// Return an enumerable of all known types.
        /// </summary>
        public static IEnumerable<Type> GetAllTypes(Predicate<string> ignoreFilter)
        {
            var assemblies =
                AppDomain.CurrentDomain.GetAssemblies()
                    // Automatically exclude the Unity assemblies, which throw exceptions when we try to access them.
                    .Where(a => 
                        !a.FullName.StartsWith("UnityEngine") && 
                        !a.FullName.StartsWith("UnityEditor")
                    );

            foreach (var assembly in assemblies)
            {
                if (ignoreFilter != null && IgnoreAssembly(assembly, ignoreFilter))
                {
                    continue;
                }

                Type[] types;

                try
                {
                    types = assembly.GetTypes();
                }
                catch (Exception)
                {
                    // Ignore assemblies that can't be loaded.
                    continue;
                }

                foreach (var type in types)
                {
                    yield return type;
                }
            }
        }

        /// <summary>
        /// Find all types that are marked with the specified attribute.
        /// </summary>
        public static Type[] FindTypesMarkedByAttributes(IEnumerable<Type> markerAttributeTypes, Predicate<string> ignoreFilter)
        {
            if (markerAttributeTypes == null)
            {
                throw new ArgumentNullException("attributeTypes");
            }

            if (!markerAttributeTypes.Any())
            {
                throw new ArgumentException("Empty array.", "attributeTypes");
            }

            var typesFound = new List<Type>();

            foreach (var type in GetAllTypes(ignoreFilter))
            {
                try
                {
                    //todo: ash: might be able to use this fn instead:
                    // http://msdn.microsoft.com/en-us/library/dwc6ew1d(v=vs.110).aspx
                    foreach (var foundAttribute in type.GetCustomAttributes(false)) //todo: merge with match attributes code
                    {
                        var hasMarkerAttribute = 
                                markerAttributeTypes
                                    .Where(markerAttributeType => markerAttributeType.IsAssignableFrom(foundAttribute.GetType()))
                                    .Any();

                        if (hasMarkerAttribute)
                        {
                            typesFound.Add(type);
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    continue; // Some types throw exception when we try to use reflection on them.
                }
            }

            return typesFound.ToArray();
        }

        /// <summary>
        /// Determine if a type has a property.
        /// </summary>
        public static bool HasProperty(Type type, string propertyName)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Bad property name.", "propertyName");
            }

            string[] splitPropertyNames = propertyName.Split(new char[] { '.' });
            foreach (string name in splitPropertyNames)
            {
                var property = type.GetProperty(name);
                if (property == null)
                {
                    return false;
                }

                type = property.PropertyType;
            }

            return true;
        }

        /// <summary>
        /// Get the type of a property.
        /// </summary>
        public static Type GetPropertyType(Type type, string propertyName)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Bad property name.", "propertyName");
            }

            string[] splitPropertyNames = propertyName.Split(new char[] { '.' });
            Type propertyType = null;
            foreach (string name in splitPropertyNames)
            {
                var property = type.GetProperty(name);
                if (property == null)
                {
                    var field = type.GetField(name);
                    if (field == null)
                    {
                        throw new ApplicationException("Invalid property name: " + propertyName + " for type " + type.Name);
                    }
                    else
                    {
                        propertyType = field.FieldType;
                    }
                }
                else
                {
                    propertyType = property.PropertyType;
                }
            }

            return propertyType;
        }


        /// <summary>
        /// Determine if a type has a writable property.
        /// </summary>
        public static bool HasWritableProperty(Type type, string propertyName)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Bad property name.", "propertyName");
            }

            string[] splitPropertyNames = propertyName.Split(new char[] { '.' });
            PropertyInfo property = null;
            foreach (string name in splitPropertyNames)
            {
                property = type.GetProperty(name);
                if (property == null)
                {
                    return false;
                }

                type = property.PropertyType;
            }

            if (!property.CanWrite)
            {
                return false;
            }

            var setMethod = property.GetSetMethod();
            if (setMethod == null)
            {
                return false;
            }

            return setMethod.IsPublic;
        }

        /// <summary>
        /// Get the value of a property.
        /// </summary>
        public static object GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Bad property name.", "propertyName");
            }

            return GetPropertyValue(obj, obj.GetType(), propertyName);
        }

        /// <summary>
        /// Get the value of a static property.
        /// </summary>
        public static object GetStaticPropertyValue(Type type, string propertyName)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Bad property name.", "propertyName");
            }

            return GetPropertyValue(null, type, propertyName);
        }

        /// <summary>
        /// Get the value of a property.
        /// </summary>
        private static object GetPropertyValue(object obj, Type type, string propertyName)
        {
            string[] splitPropertyNames = propertyName.Split(new char[] { '.' });
            foreach (string name in splitPropertyNames)
            {
                var property = type.GetProperty(name);
                if (property == null)
                {
                    throw new ApplicationException("Failed to get property, Property " + propertyName + " not found for object " + obj.GetType().Name);
                }

                obj = property.GetValue(obj, null);
                if (obj == null)
                {
                    return null;
                }

                type = obj.GetType();
            }

            return obj;
        }

        /// <summary>
        /// Set the value of a property.
        /// </summary>
        public static void SetPropertyValue(object obj, string propertyName, object value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Bad property name.", "propertyName");
            }

            Type type = obj.GetType();
            var property = type.GetProperty(propertyName);
            if (property == null)
            {
                var field = type.GetField(propertyName);
                if (field == null)
                {
                    throw new ApplicationException("Failed to set property, Property " + propertyName + " not found for object " + obj.GetType().Name);
                }

                if (!field.IsPublic)
                {
                    throw new ApplicationException("Failed to set field " + obj.GetType().Name + "." + propertyName + " (" + field.FieldType.Name + ") to " + value.ToString() + " (" + value.GetType().Name + ")" + " Field is not marked as public");
                }

                //
                // Set field value.
                //
                try
                {
                    field.SetValue(obj, value);
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Failed to set field " + obj.GetType().Name + "." + propertyName + " (" + field.FieldType.Name + ") to " + value.ToString() + " (" + value.GetType().Name + ")", e);
                }
            }
            else
            {
                // 
                // Set property value.
                //

                if (!property.GetSetMethod(true).IsPublic)
                {
                    throw new ApplicationException("Failed to set property " + obj.GetType().Name + "." + propertyName + " (" + property.PropertyType.Name + ") to " + value.ToString() + " (" + value.GetType().Name + ")" + " Property is not marked as public");
                }

                try
                {
                    property.SetValue(obj, value, null);
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Failed to set property " + obj.GetType().Name + "." + propertyName + " (" + property.PropertyType.Name + ") to " + value.ToString() + " (" + value.GetType().Name + ")", e);
                }
            }
        }

        /// <summary>
        /// Determine if the specified type has a particular function.
        /// </summary>
        public static bool HasMethod(Type type, string methodName)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (string.IsNullOrEmpty(methodName))
            {
                throw new ArgumentException("Bad function name.", "functionName");
            }

            var methods = type.GetMethods();
            var methodNames = methods.Select(m => m.Name).ToArray();
            var methodIndex = Array.IndexOf(methodNames, methodName);
            return methodIndex != -1;
        }

        /// <summary>
        /// Invoke a function on the object.
        /// </summary>
        public static object InvokeMethod(object obj, string methodName, object[] args)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            if (string.IsNullOrEmpty(methodName))
            {
                throw new ArgumentException("Bad method name.", "methodName");
            }

            var argTypes = args != null ? args.Select(a => a.GetType()).ToArray() : new Type[0];
            var method = obj.GetType().GetMethod(methodName, argTypes);
            if (method == null)
            {
                throw new ApplicationException("Invalid method: " + methodName +" method does not exist.");
            }

            return InvokeMethod(obj, method, args);
        }

        /// <summary>
        /// Invoke a function on the object.
        /// </summary>
        public static object InvokeMethod(object obj, MethodInfo method, object[] args)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            try
            {
                return method.Invoke(obj, args);
            }
            catch (TargetInvocationException ex)
            {
                // Re-throw the original exception.
                throw ex.InnerException;
            }
        }

        /// <summary>
        /// Find a method based on name and number of arguments.
        /// </summary>
        public static MethodInfo GetMethod(Type type, string name, int numArgs)
        {
            var matchingMethods = type.GetMethods().Where(m => m.Name == name && m.GetParameters().Length == numArgs);
            if (matchingMethods.Count() == 0)
            {
                return null;
            }

            if (matchingMethods.Count() > 1)
            {
                throw new ApplicationException("Found multiple methods with name " + name + " and " + numArgs + " arguments.");
            }

            return matchingMethods.First();
        }

        /// <summary>
        /// Returns true if the specified class type implements the specified interface.
        /// </summary>
        public static bool ClassImplements(Type classType, Type interfaceType)
        {
            if (classType == null)
            {
                throw new ArgumentNullException("classType");
            }

            if (interfaceType == null)
            {
                throw new ArgumentNullException("interfaceType");
            }

            foreach (var iface in classType.GetInterfaces())
            {
                if (iface == interfaceType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
