using System;
using System.Collections.Generic;
using System.Linq;

namespace Classy.DotNet.Mvc.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || !list.Any();
        }

        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var t in list)
                action(t);
        }

        public static IEnumerable<KeyValuePair<int, T>> Indexed<T>(this IEnumerable<T> list)
        {
            var i = 0;
            foreach (var t in list)
            {
                yield return new KeyValuePair<int, T>(i, t);
                i++;
            }
        }
    }
}