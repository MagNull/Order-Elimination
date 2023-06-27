using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.Infrastructure
{
    public static class EnumExtensions
    {
        public static IEnumerable<T> GetValues<T>() where T : Enum //To optimize - make Lazy
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
