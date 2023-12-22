using OrderElimination.GameContent;
using OrderElimination.Infrastructure;
using OrderElimination.MacroGame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.SavesManagement
{
    public class SaveDataPacker
    {
        private IDataMapping<Guid, IGameCharacterTemplate> _charactersMapping;

        public PlayerProgressSerializableData PackSaveData(
            PlayerProgressData progress)
        {
            ValidateMappings();
            var playerCharactersData = new List<GameCharacterSaveData>();
            foreach (var character in progress.PlayerCharacters)
            {
                var charData = new GameCharacterSaveData(
                    _charactersMapping.GetKey(character.CharacterData),
                    new GameCharacterStats(character.CharacterStats),
                    character.CurrentHealth);
                //character.Inventory);
                playerCharactersData.Add(charData);
            }
            var saveData = new PlayerProgressSerializableData(
                playerCharactersData.ToArray(), progress.StatsUpgrades, progress.Currencies);
            return saveData;
        }

        public PlayerProgressData UnpackSaveData(PlayerProgressSerializableData saveData)
        {
            ValidateMappings();
            var playerCharacters = saveData.PlayerSquadCharacters
                .Select(c => GameCharacterSerializer.UnpackCharacterFromSaveData(c, _charactersMapping))
                .ToArray();
            return new PlayerProgressData(
                playerCharacters, saveData.StatsUpgrades, saveData.Currencies);
        }

        public void RefreshMappings()
        {
            _charactersMapping = BuildGuidAssetMapping<IGameCharacterTemplate>(
                c => ((IGuidAsset)c).AssetId);
        }

        private void ValidateMappings()
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
