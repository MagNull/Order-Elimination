using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

    public static class AutomatedCloner<T> where T : ICloneable
    {
        public static readonly HashSet<Type> _cloneableTypes;

        static AutomatedCloner()
        {
            _cloneableTypes = ReflectionExtensions
                .GetAllInterfaceImplementationTypes<T>()
                .ToHashSet();

            //if struct (recurcively) contains children with class elements => deep copy
            //Ignore static members

            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Instance);
            var properties = type.GetProperties(BindingFlags.Instance).Where(p => p.CanWrite).ToArray();
            //Copied by .Clone()
            var cloneableFields = fields.Where(p => _cloneableTypes.Contains(p.FieldType)).ToArray();
            var cloneableProperties = properties.Where(p => _cloneableTypes.Contains(p.PropertyType)).ToArray();
            var restFields = fields.Except(cloneableFields).ToArray();
            var restProperties = properties.Except(cloneableProperties).ToArray();
            //Cloned members copied using ICloneable.Clone();
            //var collectionProperties =
        }

        public static T GetClonedObject(T instance)
        {
            if (instance == null) Logging.LogException( new ArgumentNullException());
            Logging.LogException( new NotImplementedException());
            throw new NotImplementedException();
        }
    }
}
