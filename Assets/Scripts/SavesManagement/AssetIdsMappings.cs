using GameInventory.Items;
using OrderElimination.GameContent;
using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.SavesManagement
{
    public static class AssetIdsMappings
    {
        private static DateTime _lastUpdateTime;
        private static DataMapping<Guid, IGameCharacterTemplate> _charactersMapping;
        private static DataMapping<Guid, ItemData> _itemsMapping;

        public static IDataMapping<Guid, IGameCharacterTemplate> CharactersMapping
            => _charactersMapping;
        public static IDataMapping<Guid, ItemData> ItemsMapping
            => _itemsMapping;

        public static bool IsValidationRequired
            => true;
            //=> _lastUpdateTime == default || (DateTime.Now -_lastUpdateTime).Seconds > 2;//mock

        static AssetIdsMappings()
        {
            RefreshMappings();
        }

        public static void RefreshMappings()
        {
            if (!IsValidationRequired)
                return;
            _charactersMapping = BuildGuidAssetMapping<IGameCharacterTemplate>(
                c => ((IGuidAsset)c).AssetId);
            _itemsMapping = BuildGuidAssetMapping<ItemData>(d => d.AssetId);
            _lastUpdateTime = DateTime.Now;
        }

        private static DataMapping<Guid, TData> BuildGuidAssetMapping<TData>(
            Func<TData, Guid> guidGetter)
        {
            var dataType = typeof(TData);
            var unityObjectType = typeof(UnityEngine.Object);
            var dataTypes = ReflectionExtensions
                .GetAllSubTypes(dataType)
                .Where(t => unityObjectType.IsAssignableFrom(t));

            var result = new DataMapping<Guid, TData>();
            var foundObjects = new Dictionary<TData, Guid>();

            foreach (var type in dataTypes)
            {
                var dataObjects = Resources.FindObjectsOfTypeAll(type).Cast<TData>();
                foreach (var obj in dataObjects)
                {
                    if (!foundObjects.ContainsKey(obj))
                    {
                        var id = guidGetter(obj);
                        foundObjects.Add(obj, id);
                    }
                }
            }
            foreach (var obj in foundObjects.Keys)
            {
                result.Add(foundObjects[obj], obj);
            }

            return result;
        }
    }
}
