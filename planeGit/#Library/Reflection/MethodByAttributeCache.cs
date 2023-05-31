using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Library.Util.Reflection
{
    public static class MethodByAttributeCache
    {
        static Dictionary<(Type, Type), MethodInfo[]> contexts = new();

        public static ReadOnlyCollection<MethodInfo> GetMethodsByAttribute(Type targetType, Type attributeType, bool useCached = true)
        {
            if (useCached && contexts.TryGetValue((targetType, attributeType), out var context))
            {
                return new ReadOnlyCollection<MethodInfo>(context);
            }

            InternalInit(targetType, attributeType);

            return new ReadOnlyCollection<MethodInfo>(contexts[(targetType, attributeType)]);
        }

        static void InternalInit(Type targetType, Type attributeType)
        {
            MethodInfo[] context = ReflectionUtility.GetMethods(targetType).Where(type => type.GetCustomAttributes(attributeType, true).Any()).ToArray();

            contexts[(targetType, attributeType)] = context;
        }
    }
}
