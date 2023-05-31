using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Library.Util.Reflection
{
    public static class TypeByAttributeCache
    {
        static Dictionary<(Type, Assembly[]), Type[]> contexts = new();

        public static ReadOnlyCollection<Type> GetTypesByAttribute(Type attributeType, params Assembly[] assemblies)
        {
            if (contexts.TryGetValue((attributeType, assemblies), out Type[] context) == true)
            {
                return new ReadOnlyCollection<Type>(context);
            }

            InternalInit(attributeType, assemblies);

            return new ReadOnlyCollection<Type>(contexts[(attributeType, assemblies)]);
        }

        static void InternalInit(Type attributeType, Assembly[] assemblies)
        {
            Type[] context;
            if (assemblies != null && assemblies.Length > 0)
            {
                List<Type> temp = new();
                foreach (Assembly assembly in assemblies)
                {
                    temp.AddRange(assembly.GetTypes().Where(type => type.GetCustomAttributes(attributeType, true).Any()));
                }

                context = temp.ToArray();
            }
            else
            {
                context = Assembly.GetExecutingAssembly().GetTypes().Where(type => type.GetCustomAttributes(attributeType, true).Any()).ToArray();
            }

            contexts.Add((attributeType, assemblies), context);
        }
    }
}