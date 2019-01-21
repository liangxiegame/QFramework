using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSG.Utils
{
    /// <summary>
    /// Provider of reflection services.
    /// </summary>
    public interface IReflection
    {
        /// <summary>
        /// Returns true when the type has a property with the specified name.
        /// </summary>
        bool HasProperty(Type type, string propertyName);

        /// <summary>
        /// Get the value of a property.
        /// </summary>
        T GetPropertyValue<T>(object obj, string propertyName);

        /// <summary>
        /// Sets the value of the property.
        /// </summary>
        void SetPropertyValue<T>(object obj, string propertyName, T propertyValue);

        /// <summary>
        /// Gets all of the properties for this object
        /// </summary>
        string[] GetPropertyNames(object obj);

        /// <summary>
        /// Returns true if the requested property name contains the specified attribute
        /// </summary>
        bool PropertyHasAttribute(object obj, string propertyName, Type attributeType);

        Type GetPropertyType(Type type, string propertyName);

        T GetPropertyAttribute<T>(object obj, string propertyName)
            where T : Attribute;

        /// <summary>
        /// Retrieves the class attribute object of the type specified
        /// returns null if attribute not found
        /// </summary>
        AttributeT GetClassAttribute<AttributeT>(Type type)
            where AttributeT : Attribute;

        /// <summary>
        /// Gets the type of the supplied object
        /// </summary>        
        Type GetObjectType(object target);

        /// <summary>
        /// Get the names of types that impemented the specified interface.
        /// </summary>
        IEnumerable<Type> GetTypesThatImplement<T>();

        /// <summary>
        /// Get the names of types that impemented the specified interface.
        /// </summary>
        IEnumerable<string> GetTypeNamesThatImplement<T>();

        /// <summary>
        /// Invoke a method with the name [methodName] on [parentObject] with the arguments [args]
        /// argumentValueConverter converts/marshals arguments to the expected type.
        /// </summary>
        object InvokeMethod(object parentObject, string methodName, object[] args);

        /// <summary>
        /// Invoke a method with the name [methodName] on [parentObject] with the arguments [args]
        /// argumentValueConverter converts/marshals arguments to the expected type.
        /// </summary>
        object InvokeMethod(object parentObject, string methodName, object[] args, Func<object, Type, object> argumentValueConverter);

        /// <summary>
        /// Find all types marked by the specified attributes.
        /// </summary>
        IEnumerable<Type> FindTypesMarkedByAttributes(IEnumerable<Type> attributesTypes);

        /// <summary>
        /// Get all attributes attached to the specified type.
        /// </summary>
        IEnumerable<AttributeT> GetAttributes<AttributeT>(Type type)
            where AttributeT : Attribute;

        /// <summary>
        /// Returns true if the requested types has the specified attribute.
        /// </summary>
        bool HasAttribute<AttributeT>(Type type)
            where AttributeT : Attribute;
    }

    /// <summary>
    /// Provider of reflection services.
    /// </summary>
    public class Reflection : IReflection
    {
        /// <summary>
        /// Used to filter assemblies when searching for types, etc.
        /// </summary>
        private Predicate<string> assemblyIgnoreFilter;

        public Reflection()
        {

        }

        public Reflection(Predicate<string> assemblyIgnoreFilter)
        {
            Argument.NotNull(() => assemblyIgnoreFilter);

            this.assemblyIgnoreFilter = assemblyIgnoreFilter;
        }

        /// <summary>
        /// Returns true when the type has a property with the specified name.
        /// </summary>
        public bool HasProperty(Type type, string propertyName)
        {
            return ReflectionUtils.HasProperty(type, propertyName);
        }

        /// <summary>
        /// Get the value of a property.
        /// </summary>
        public T GetPropertyValue<T>(object obj, string propertyName)
        {
            return (T)ReflectionUtils.GetPropertyValue(obj, propertyName);
        }

        /// <summary>
        /// Sets the value of the property.
        /// </summary>
        public void SetPropertyValue<T>(object obj, string propertyName, T propertyValue)
        {
            ReflectionUtils.SetPropertyValue(obj, propertyName, propertyValue);
        }

        /// <summary>
        /// Gets all of the properties for this object
        /// </summary>
        public string[] GetPropertyNames(object obj)
        {
            Type type = obj.GetType();

            string[] properties = type.GetProperties().Select(p => p.Name).ToArray();

            return properties;
        }

        /// <summary>
        /// Returns true if the requested property name contains the specified attribute
        /// </summary>
        public bool PropertyHasAttribute(object obj, string propertyName, Type attributeType)
        {
            var property = obj.GetType().GetProperty(propertyName);
            
            if (property == null)
            {
                throw new ApplicationException("Failed to get property, Property " + propertyName + " not found for object " + obj.GetType().Name);
            }

            return ReflectionUtils.PropertyHasAttribute(property, attributeType);
        }


        public Type GetPropertyType(Type type, string propertyName)
        {
            return ReflectionUtils.GetPropertyType(type, propertyName);
        }


        public T GetPropertyAttribute<T>(object obj, string propertyName)
            where T : Attribute
        {
            var property = obj.GetType().GetProperty(propertyName);

            if (property == null)
            {
                throw new ApplicationException("Failed to get property, Property " + propertyName + " not found for object " + obj.GetType().Name);
            }

            return ReflectionUtils.GetPropertyAttribute<T>(property);
        }

        /// <summary>
        /// Retrieves the class attribute object of the type specified
        /// returns null if attribute not found
        /// </summary>
        public AttributeT GetClassAttribute<AttributeT>(Type type)
            where AttributeT : Attribute
        {
            return ReflectionUtils.GetAttribute<AttributeT>(type);
        }

        /// <summary>
        /// Gets the type of the supplied object
        /// </summary>        
        public Type GetObjectType(object target)
        {
            return target.GetType();
        }

        /// <summary>
        /// Get the names of types that impemented the specified interface.
        /// </summary>
        public IEnumerable<Type> GetTypesThatImplement<T>()
        {
            return ReflectionUtils
                .GetAllTypes()
                .Where(t => ReflectionUtils.ClassImplements(t, typeof(T)))
                .Where(t => !t.IsInterface);
        }

        /// <summary>
        /// Get the names of types that impemented the specified interface.
        /// </summary>
        public IEnumerable<string> GetTypeNamesThatImplement<T>()
        {
            return GetTypesThatImplement<T>()
                .Select(t => t.Name);
        }

        /// <summary>
        /// Invoke a method with the name [methodName] on [parentObject] with the arguments [args]
        /// argumentValueConverter converts/marshals arguments to the expected type.
        /// </summary>
        public object InvokeMethod(object parentObject, string methodName, object[] args)
        {
            return InvokeMethod(parentObject, methodName, args, (a, t) => a);
        }

        /// <summary>
        /// Invoke a method with the name [methodName] on [parentObject] with the arguments [args]
        /// argumentValueConverter converts/marshals arguments to the expected type.
        /// </summary>
        public object InvokeMethod(object parentObject, string methodName, object[] args, Func<object, Type, object> argumentValueConverter)
        {
            Argument.NotNull(() => parentObject);
            Argument.StringNotNullOrEmpty(() => methodName);
            Argument.NotNull(() => args);
            Argument.NotNull(() => argumentValueConverter);        

            try
            {
                int numArgs = args.Count();
                var method = ReflectionUtils.GetMethod(parentObject.GetType(), methodName, numArgs);
                if(method == null)
                {
                    throw new ApplicationException("Failed to find method " + methodName + " with " + numArgs + " arguments on type " + parentObject.GetType().Name);
                }

                var methodParameters = method.GetParameters();
                if(numArgs == 0 && methodParameters.Count() > 0)
                {
                    throw new ApplicationException("Got no arguments, expected " + methodParameters.Count());
                }
                else if(numArgs != methodParameters.Count())
                {
                    throw new ApplicationException("Got " + numArgs + " arguments, expected " + methodParameters.Count());
                }

                var convertedArgs = new List<object>();
                for (int argIndex = 0; argIndex < methodParameters.Count(); ++argIndex)
                {
                    convertedArgs.Add(argumentValueConverter(args[argIndex], methodParameters[argIndex].ParameterType));
                }

                return ReflectionUtils.InvokeMethod(parentObject, method, convertedArgs.ToArray());
            }
            catch (Exception e)
            {
                throw new ApplicationException("Invocation of method " + methodName + " failed, " + e);
            }
        }

        /// <summary>
        /// Find all types marked by the specified attributes.
        /// </summary>
        public IEnumerable<Type> FindTypesMarkedByAttributes(IEnumerable<Type> attributesTypes)
        {
            Argument.NotNull(() => attributesTypes);

            return ReflectionUtils.FindTypesMarkedByAttributes(attributesTypes, assemblyIgnoreFilter);
        }

        /// <summary>
        /// Get all attributes attached to the specified type.
        /// </summary>
        public IEnumerable<AttributeT> GetAttributes<AttributeT>(Type type)
            where AttributeT : Attribute
        {
            Argument.NotNull(() => type);

            return ReflectionUtils.GetAttributes<AttributeT>(type);
        }

        /// <summary>
        /// Returns true if the requested types has the specified attribute.
        /// </summary>
        public bool HasAttribute<AttributeT>(Type type)
            where AttributeT : Attribute
        {
            Argument.NotNull(() => type);

            return GetAttributes<AttributeT>(type).Any();
        }
    }
}
