﻿using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

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

        public IReadOnlyDictionary<T, bool> ValueFlags => _valueFlags;

        public bool this[T type]
        {
            get => _valueFlags[type];
            set => _valueFlags[type] = value;
        }

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

        public EnumMask<T> Clone()
        {
            var clone = new EnumMask<T>();
            clone._valueFlags = _valueFlags.ToDictionary(kv => kv.Key, kv => kv.Value);
            return clone;
        }
    }
}