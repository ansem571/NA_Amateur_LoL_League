using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Domain.Helpers
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty(this object obj)
        {
            return obj == null || string.IsNullOrWhiteSpace(obj.ToString());
        }
    }

    public static class Properties<T>{

        public static bool HasEmptyProperties(T obj)
        {
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var hasProperty = properties.Select(x => x.GetValue(obj, null))
                .Any(x => !x.IsNullOrEmpty());
            return !hasProperty;
        }
    }
}
