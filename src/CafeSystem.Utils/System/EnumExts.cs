using System;
using System.Linq;
using System.Resources;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace CafeSystem.Utils
{
    public static class EnumExts
    {
        /// <summary>
        /// Get the name of enum field.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetName(this Enum value)
        {
            return Enum.GetName(value.GetType(), value);
        }

        /// <summary>
        /// Get enum field names.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>array of enum field names</returns>
        public static string[] GetNames<T>() where T : Enum
        {
            return Enum.GetNames(typeof(T));
        }

        /// <summary>
        /// Get value associated to the EnumMemberAttribute
        /// </summary>
        /// <param name="value">enum value</param>
        /// <returns>EnumMemberAttribute value</returns>
        public static string GetEnumMemberValue(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.GetName());
            return fieldInfo?.GetCustomAttribute<EnumMemberAttribute>()?.Value;
        }

        /// <summary>
        /// Return enum values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Returns the descriptor value associated to the enum field. 
        /// </summary>
        /// <param name="value">enum value</param>
        /// <returns>enum descriptor </returns>
        public static string GetDescriptor(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.GetName());
            return GetFieldDescriptor(fieldInfo);
        }

        /// <summary>
        /// Returns the enum descriptor values.
        /// </summary>
        /// <typeparam name="T">enum type</typeparam>
        /// <returns>array of enum descriptor values</returns>
        public static string[] GetDescriptors<T>() where T : Enum
        {
            return typeof(T).GetEnumNames().Select(x => GetFieldDescriptor(typeof(T).GetField(x))).ToArray();
        }

        /// <summary>
        /// Get the descriptor value associated to the enum field.
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        private static string GetFieldDescriptor(MemberInfo fieldInfo)
        {
            var attr = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
            if (attr != null)
            {
                return attr.Description;
            }

            const string pattern = @"(?<=[A-Za-z])(?=[A-Z][a-z])|(?<=[a-z0-9])(?=[0-9]?[A-Z])";
            var regex = new Regex(pattern);
            return regex.Replace(fieldInfo.Name, " ");
        }

        public static string GetDescription(this Enum @enum)
        {
            var descriptions = new List<string>();
            var type = @enum.GetType();

            foreach (var item in @enum.GetFlags())
            {
                var enumDescription = item.ToString();

                var fieldInfo = type.GetRuntimeField(enumDescription);
                enumDescription = GetFieldInfoDescription(fieldInfo, defaultValue: enumDescription);

                descriptions.Add(enumDescription);
            }

            if (descriptions.Any())
            {
                return string.Join(", ", descriptions);
            }

            return @enum.ToString();
        }

        public static IEnumerable<T> GetFlags<T>(this T @enum) where T : Enum
        {
            var values = Enum.GetValues(@enum.GetType()).Cast<T>();
            var bits = Convert.ToInt64(@enum);
            var results = new List<T>();

            for (var i = values.Count() - 1; i >= 0; i--)
            {
                var mask = Convert.ToInt64(values.ElementAt(i));
                if (i == 0 && mask == 0L)
                {
                    break;
                }

                if ((bits & mask) == mask)
                {
                    results.Add(values.ElementAt(i));
                    bits -= mask;
                }
            }

            if (bits != 0L)
            {
                return Enumerable.Empty<T>();
            }

            if (Convert.ToInt64(@enum) != 0L)
            {
                return results.Reverse<T>();
            }

            if (bits == Convert.ToInt64(@enum) && values.Any() && Convert.ToInt64(values.ElementAt(0)) == 0L)
            {
                return values.Take(1);
            }

            return Enumerable.Empty<T>();
        }

        public static Dictionary<int, string> GetDescriptions<T>() where T : Enum
            => GetDescriptions(typeof(T));

        public static Dictionary<int, string> GetDescriptions(this Type enumType)
        {
            var descriptions = new Dictionary<int, string>();
            var fields = enumType.GetRuntimeFields().Where(f => f.IsStatic).ToList();

            foreach (var fieldInfo in fields)
            {
                var value = (int)fieldInfo.GetValue(null);
                var enumDescription = GetFieldInfoDescription(fieldInfo, defaultValue: Enum.GetName(enumType, value));

                descriptions.Add(value, enumDescription);
            }

            return descriptions;
        }

        private static string GetFieldInfoDescription(FieldInfo fieldInfo, string defaultValue)
        {
            string enumDescription;

            var displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute?.ResourceType is null)
            {
                enumDescription = displayAttribute?.Name;
            }
            else
            {
                var resourceManager = new ResourceManager(displayAttribute.ResourceType);
                enumDescription = resourceManager.GetString(displayAttribute.Name) ?? displayAttribute.Name;
            }

            return enumDescription ?? defaultValue;
        }
    }
}