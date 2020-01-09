using System;
using System.Linq;
using System.Reflection;

namespace Domain.Helpers
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty(this object obj)
        {
            return obj == null || string.IsNullOrWhiteSpace(obj.ToString());
        }
    }

    public static class Properties<T>
    {

        public static bool HasEmptyProperties(T obj)
        {
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var hasProperty = properties.Select(x => x.GetValue(obj, null))
                .Any(x => !x.IsNullOrEmpty());
            return !hasProperty;
        }
    }

    public static class TimeZoneExtensions
    {
        private static readonly TimeZoneInfo Eastern = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public static DateTime GetCurrentTime()
        {
            var utc = DateTime.UtcNow;
            var today = TimeZoneInfo.ConvertTimeFromUtc(utc, Eastern);

            return today;
        }
    }
}
