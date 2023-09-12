using System;
using System.Collections;
using System.Collections.Generic;

namespace OrderElimination.Infrastructure
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }

        public static IEnumerable<object> AsEnumerable(this ICollection collection)
        {
            foreach (var item in collection)
            {
                yield return item;
            }
        }
    }
}
