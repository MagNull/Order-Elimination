using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.Infrastructure
{
    public class EnumMask<T> : ICloneable<EnumMask<T>> where T : Enum
    {
        [OnCollectionChanged(nameof(ValidateDictionary))]
        [OnValueChanged(nameof(ValidateDictionary))]
        [DictionaryDrawerSettings(KeyLabel = "Type", ValueLabel = "Is allowed")]
        [OnInspectorInit("@$property.State.Expanded = true")]
        [ShowInInspector, OdinSerialize]
        private Dictionary<T, bool> _valueFlags = EnumExtensions.GetValues<T>().ToDictionary(e => e, e => false);

        //public IReadOnlyDictionary<T, bool> ValueFlags => _valueFlags;

        public static EnumMask<T> Empty => GetFilledInstance(false);

        public static EnumMask<T> Full => GetFilledInstance(true);

        public bool this[T type]
        {
            get => _valueFlags[type];
            set => _valueFlags[type] = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj is not EnumMask<T> mask) 
                return false;
            else
                return DifferentNotNullMaskEquals(this, mask);
        }

        public static bool operator ==(EnumMask<T> maskA, EnumMask<T> maskB)
        {
            if (maskA is null ^ maskB is null)
                return false;
            if (ReferenceEquals(maskA, maskB))
                return true;
            if (maskA._valueFlags.Count != maskB._valueFlags.Count)
                return false;
            return DifferentNotNullMaskEquals(maskA, maskB);
        }

        public static bool operator !=(EnumMask<T> maskA, EnumMask<T> maskB)
            => !(maskA == maskB);

        private static bool DifferentNotNullMaskEquals(EnumMask<T> maskA, EnumMask<T> maskB)
        {
            if (maskA._valueFlags.Count != maskB._valueFlags.Count)
                return false;
            foreach (var key in maskA._valueFlags.Keys)
            {
                if (!maskB._valueFlags.ContainsKey(key))
                    return false;
                if (maskA._valueFlags[key] != maskB._valueFlags[key])
                    return false;
            }
            return true;
        }

        public override int GetHashCode() => _valueFlags.GetHashCode();

        public EnumMask<T> Clone()
        {
            var clone = new EnumMask<T>();
            clone._valueFlags = _valueFlags.ToDictionary(kv => kv.Key, kv => kv.Value);
            return clone;
        }

        private static EnumMask<T> GetFilledInstance(bool defaultValue)
        {
            return new EnumMask<T>
            {
                _valueFlags = EnumExtensions.GetValues<T>().ToDictionary(e => e, e => defaultValue)
            };
        }

        [OnInspectorInit]
        private void ValidateDictionary()
        {
            _valueFlags ??= new Dictionary<T, bool>();
            foreach (var entityType in EnumExtensions.GetValues<T>())
            {
                if (!_valueFlags.ContainsKey(entityType))
                {
                    _valueFlags.Add(entityType, false);
                }
            }
        }
    }
}
