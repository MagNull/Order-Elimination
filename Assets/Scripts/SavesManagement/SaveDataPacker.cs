using OrderElimination.GameContent;
using OrderElimination.Infrastructure;
using OrderElimination.MacroGame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.SavesManagement
{
    public static class SaveDataPacker
    {
        private static IDataMapping<Guid, IGameCharacterTemplate> _charactersMapping;

        public static PlayerProgressSaveData PackSaveData(
            GameCharacter[] playerCharacters,
            //Mapping/Predicate activeSquadMembers,
            //Inventory playerInventory,
            StrategyStats statsUpgrades)
        {
            ValidateMappings();
            var playerCharactersData = new List<GameCharacterSaveData>();
            foreach (var character in playerCharacters)
            {
                var charData = new GameCharacterSaveData(
                    _charactersMapping.GetKey(character.CharacterData),
                    new GameCharacterStats(character.CharacterStats),
                    character.CurrentHealth);
                //character.Inventory);
                playerCharactersData.Add(charData);
            }
            var saveData = new PlayerProgressSaveData(
                playerCharactersData.ToArray(), statsUpgrades);
            return saveData;
        }

        public static GameCharacter[] UnpackPlayerCharacters(this PlayerProgressSaveData saveData)
        {
            ValidateMappings();
            return saveData.PlayerSquadCharacters
                .Select(c => GameCharacterSerializer.UnpackCharacterFromSaveData(c, _charactersMapping))
                .ToArray();
        }

        public static void RefreshMappings()
        {
            _charactersMapping = BuildGuidAssetMapping<IGameCharacterTemplate>(
                c => ((IGuidAsset)c).AssetId);
        }

        private static void ValidateMappings()
        {
            if (_charactersMapping == null)
            {
                _charactersMapping = BuildGuidAssetMapping<IGameCharacterTemplate>(
                    c => ((IGuidAsset)c).AssetId);
            }
        }

        private static IDataMapping<Guid, TData> BuildGuidAssetMapping<TData>(
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
                var dataObjects = Resources.FindObjectsOfTypeAll(dataType).Cast<TData>();
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
