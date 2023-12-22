using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.Infrastructure
{
    /// <summary>
    /// Contains both-sided mapping for pairs of elements where each element is unique.
    /// (Содержит двусторонние соответствия двух элементов, каждый из которых уникален.)
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public class DataMapping<TKey, TData> : IDataMapping<TKey, TData>
    {
        [HideInInspector, OdinSerialize]
        private Dictionary<TKey, TData> _dataMapping = new();

        [HideInInspector, OdinSerialize]
        private Dictionary<TData, TKey> _keysMapping = new();

        public bool ContainsKey(TKey key) => _dataMapping.ContainsKey(key);

        public bool ContainsData(TData data) => _keysMapping.ContainsKey(data);

        public TData GetData(TKey key) => _dataMapping[key];

        public TKey GetKey(TData data) => _keysMapping[data];

        public bool Add(TKey key, TData data)
        {
            if (_keysMapping.ContainsKey(data))
            {
                Logging.LogError($"Mapping already contains data: {data}");
                return false;
            }
            if (_dataMapping.ContainsKey(key))
            {
                Logging.LogError($"Mapping already contains key: {key}");
                return false;
            }
            _dataMapping.Add(key, data);
            _keysMapping.Add(data, key);
            return true;
        }

        public bool RemoveByKey(TKey key)
        {
            if (!_dataMapping.ContainsKey(key))
                return false;
            var data = _dataMapping[key];
            _keysMapping.Remove(data);
            _dataMapping.Remove(key);
            return true;
        }

        public bool RemoveByData(TData data)
        {
            if (!_keysMapping.ContainsKey(data))
                return false;
            var key = _keysMapping[data];
            _keysMapping.Remove(data);
            _dataMapping.Remove(key);
            return true;
        }

        public void Clear()
        {
            _keysMapping.Clear();
            _dataMapping.Clear();
        }
    }
}
