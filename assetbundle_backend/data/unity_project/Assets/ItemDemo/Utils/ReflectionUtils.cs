using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ItemDemo.Utils
{
    public static class ReflectionUtils
    {
        public static void LookupByType<T>(out List<T> list) where T : class
        {
            var lookupInterface = typeof(T);
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetInterfaces().Contains(lookupInterface));
            list = new List<T>();
            foreach (var retrievedType in types)
            {
                if (retrievedType.IsAbstract) continue;
                list.Add(Activator.CreateInstance(retrievedType) as T);
            }
        }
    }
}