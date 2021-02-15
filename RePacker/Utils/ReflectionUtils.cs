using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Refsa.RePacker.Utils
{
    public static class ReflectionUtils
    {
        public static IEnumerable<Type> GetAllTypes()
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(e => e.GetTypes());
        }

        public static IEnumerable<Type> WithAttribute(this IEnumerable<Type> self, Type attributeType)
        {
            return self.Where(t =>
                Attribute.GetCustomAttribute(t, attributeType) != null
            );
        }

        public static IEnumerable<PropertyInfo> WithAttribute(this IEnumerable<PropertyInfo> self, Type attributeType)
        {
            return self.Where(t =>
                Attribute.GetCustomAttribute(t, attributeType) != null
            );
        }

        public static FieldInfo GetPropertyBackingFieldInfo(Type target, string propertyName)
        {
            string backingFieldName = $"<{propertyName}>k__BackingField";
            FieldInfo fi = target.GetField(backingFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            return fi;
        }

        public static IEnumerable<Type> WithGenericBaseType(this IEnumerable<Type> self, Type genericBaseType)
        {
            return
                self.Where(t =>
                    t.BaseType != null &&
                    t.BaseType.IsGenericType &&
                    t.BaseType.GetGenericTypeDefinition() == genericBaseType
                );
        }

        public static IEnumerable<Type> NotGeneric(this IEnumerable<Type> self)
        {
            return self.Where(t => !t.IsGenericType);
        }
    }
}