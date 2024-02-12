using System;
using System.Linq;

namespace OrderElimination.Infrastructure
{
    public static class EnumExtensions
    {
        public static T[] GetValues<T>() where T : Enum //To optimize - make Lazy
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        }
    }
}
