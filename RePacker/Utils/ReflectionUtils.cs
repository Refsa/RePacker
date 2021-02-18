using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;

namespace Refsa.RePacker.Utils
{
    public static class ReflectionUtils
    {
        public static IEnumerable<Type> GetAllTypes()
        {
#if NET461 || NET471 || NET48
            List<Type> types = new List<Type>();

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var asmTypes = asm.GetTypes();
                    types.AddRange(asmTypes);
                }
                catch (Exception e)
                {
                    RePacker.Logger.Exception(e);
                }
            }

            return types;
#else
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(e => e.GetTypes());
#endif
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

        public static bool HasGetterAndSetter(this PropertyInfo target)
        {
            return target.GetMethod != null && target.SetMethod != null;
        }

        public static FieldInfo GetPropertyBackingFieldInfo(Type target, string propertyName)
        {
            string backingFieldName = $"<{propertyName}>k__BackingField";
            FieldInfo fi = target.GetField(backingFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            return fi;
        }
    }
}