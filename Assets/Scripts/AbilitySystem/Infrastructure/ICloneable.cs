using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.Infrastructure
{
    public interface ICloneable<T> : ICloneable
    {
        public new T Clone();

        object ICloneable.Clone() => Clone();
    }

    public static class CloneableCollectionsExtensions
    {
        public static List<T> Clone<T>(this List<T> list) where T : ICloneable
        {
            if (list == null) return null;
            return list.Select(e => (T)e.Clone()).ToList();
        }

        public static T[] Clone<T>(this T[] array) where T : ICloneable
        {
            if (array == null) return null;
            return array.Select(e => (T)e.Clone()).ToArray();
        }

        public static HashSet<T> Clone<T>(HashSet<T> hashSet) where T : ICloneable
        {
            if (hashSet == null) return null;
            return hashSet.AsEnumerable().Select(e => (T)e.Clone()).ToHashSet();
        }

        public static Dictionary<T1, T2> Clone<T1, T2>(this Dictionary<T1, T2> dict) 
            where T1 : ICloneable
            where T2 : ICloneable
        {
            if (dict == null) return null;
            return dict.ToDictionary(kv => (T1)kv.Key.Clone(), kv => (T2)kv.Value.Clone());
        }

        public static Dictionary<T1, T2> CloneForStructValue<T1, T2>(this Dictionary<T1, T2> dict)
            where T1 : ICloneable
            where T2 : struct
        {
            if (dict == null) return null;
            return dict.ToDictionary(kv => (T1)kv.Key.Clone(), kv => kv.Value);
        }

        public static Dictionary<T1, T2> CloneForStructKey<T1, T2>(this Dictionary<T1, T2> dict)
            where T1 : struct
            where T2 : ICloneable
        {
            if (dict == null) return null;
            return dict.ToDictionary(kv => kv.Key, kv => (T2)kv.Value.Clone());
        }
    }

    public class AutomatedCloner<T>
    {
        //static AutomatedCloner()
        //{
        //    var type = typeof(T);
        //    var properties = type.GetProperties().Where(p => p.CanWrite);
        //    var fields = type.GetFields();
        //    var cloneableProperties = properties.Where(p => p.PropertyType.GetInterfaces().Contains(typeof(ICloneable)));
        //    var cloneableFields = fields.Where(p => p.FieldType.GetInterfaces().Contains(typeof(ICloneable)));
        //    //Cloned members copied using ICloneable.Clone();
        //    //var collectionProperties =
        //}

        //public T GetClonedObject(T instance)
        //{

        //}
    }
}
