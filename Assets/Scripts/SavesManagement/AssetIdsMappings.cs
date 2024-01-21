using OrderElimination.GameContent;
using OrderElimination.Infrastructure;
using System;
using System.Linq;
using UnityEngine;

namespace OrderElimination.SavesManagement
{
    public static class AssetIdsMappings
    {
        private static DateTime _lastUpdateTime;
        private static DataMapping<Guid, IGameCharacterTemplate> _charactersMapping;

        public static IDataMapping<Guid, IGameCharacterTemplate> CharactersMapping
            => _charactersMapping;

        public static bool IsValidationRequired 
            => _lastUpdateTime == default || (DateTime.Now -_lastUpdateTime).Seconds > 5;//mock

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

            foreach (var type in dataTypes)
            {
                var dataObjects = Resources.FindObjectsOfTypeAll(type).Cast<TData>();
                foreach (var obj in dataObjects)
                {
                    var id = guidGetter(obj);
                    result.Add(id, obj);
                }
            }

            return result;
        }
    }
}
