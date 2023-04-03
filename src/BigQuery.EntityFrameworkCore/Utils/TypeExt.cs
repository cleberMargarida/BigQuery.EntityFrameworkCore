using BigQuery.EntityFrameworkCore;
using BigQuery.EntityFrameworkCore.Utils;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Reflection
{
    internal static class TypeExt
    {
        public static string KeyColumn(this Type type)
        {
            if (typeof(Table).IsAssignableFrom(type))
            {
                type = type.GetGenericArguments()[0];
            }

            var properties = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(x => x.Name);

            properties.TryGetValue("Id", out var keyColumnProperty);

            var keyColumn = properties.Values
                .SingleOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null)?
                .Name ?? keyColumnProperty?.Name ?? throw GetInvalidOperationException(type);

            return keyColumn;
        }

        private static InvalidOperationException GetInvalidOperationException(Type type)
        {
            return new InvalidOperationException(string.Format(ErrorMessages.KeyColumnNotFounded, type.FullName));
        }
    }

}

namespace System
{
    internal static class TypeExt
    {
        public static bool IsString(this Type type)
        {
            return type == typeof(string);
        }

        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsAnonymousType(this Type type)
        {
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType
                && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>", StringComparison.OrdinalIgnoreCase) ||
                    type.Name.StartsWith("VB$", StringComparison.OrdinalIgnoreCase))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }
    }
}