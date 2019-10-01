using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Net.Bluewalk.DotNetEnvironmentExtensions
{
    public static class EnvironmentExtensions
    {
        /// <summary>
        /// Read object properties from Environment variables
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="autoCreateInstances">Auto-create instances of non instantiated objects (requires parameter-less constructor)</param>
        public static object FromEnvironment(this Type type, bool autoCreateInstances = true)
        {
            var obj = Activator.CreateInstance(type);
            obj.FromEnvironment(autoCreateInstances);

            return obj;
        }

        /// <summary>
        /// Read object properties from Environment variables
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="autoCreateInstances">Auto-create instances of non instantiated objects (requires parameter-less constructor)</param>
        public static void FromEnvironment(this object obj, bool autoCreateInstances = true)
        {
            var props = GetPublicProperties(obj.GetType());

            foreach (var p in props)
            {
                if (!p.PropertyType.GetTypeInfo().IsSimple())
                {
                    if (p.GetValue(obj) == null)
                    {
                        if (p.PropertyType.GetConstructor(
                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                null, Type.EmptyTypes, null) == null)
                            throw new NoParameterLessConstructorException(p.PropertyType);

                        p.SetValue(obj, Activator.CreateInstance(p.PropertyType));
                    }

                    p.GetValue(obj).FromEnvironment();
                }

                var attr = p.GetCustomAttribute<EnvironmentVariable>(false);
                if (attr == null) continue;

                var val = GetEnvironmentVariable(p.PropertyType, attr.Name, attr.Default);

                p.SetValue(obj, val);
            }
        }

        /// <summary>
        /// Check if a type is a simple type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsSimple(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return type.GetGenericArguments().FirstOrDefault()?.GetTypeInfo().IsSimple() == true;

            return type.IsPrimitive
                   || type.IsEnum
                   || type == typeof(string)
                   || type == typeof(decimal);
        }

        /// <summary>
        /// Get environment variable to specified type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static object GetEnvironmentVariable(Type type, string name, object defaultValue = default)
        {
            var value = Environment.GetEnvironmentVariable(name);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (value == null)
                    return defaultValue;

                type = Nullable.GetUnderlyingType(type);
            }

            return string.IsNullOrEmpty(value) ? defaultValue : Convert.ChangeType(value, type);
        }

        /// <summary>
        /// Get environment variable to specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static T GetEnvironmentVariable<T>(string name, T defaultValue = default)
        {
            return (T)GetEnvironmentVariable(typeof(T), name, defaultValue);
        }

        /// <summary>
        /// Gets the public properties.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;PropertyInfo&gt;.</returns>
        private static IEnumerable<PropertyInfo> GetPublicProperties(this Type type)
        {
            if (!type.IsInterface)
                return type.GetProperties();

            return (new[] { type })
                .Concat(type.GetInterfaces())
                .SelectMany(i => i.GetProperties());
        }
    }


    public class NoParameterLessConstructorException : Exception
    {
        public NoParameterLessConstructorException(Type type) : base($"{type} does not have a parameter-less constructor")
        {
        }
    }
}
