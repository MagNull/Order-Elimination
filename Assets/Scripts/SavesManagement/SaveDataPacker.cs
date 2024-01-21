using OrderElimination.Infrastructure;
using OrderElimination.MacroGame;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.SavesManagement
{
    public class SaveDataPacker
    {
        public PlayerProgressSerializableData PackSaveData(
            IPlayerProgressData progress)
        {
            AssetIdsMappings.RefreshMappings();
            var playerCharactersData = new List<GameCharacterSaveData>();
            foreach (var character in progress.PlayerCharacters)
            {
                var charData = new GameCharacterSaveData(
                    AssetIdsMappings.CharactersMapping.GetKey(character.CharacterData),
                    new GameCharacterStats(character.CharacterStats),
                    character.CurrentHealth);
                //character.Inventory);
                playerCharactersData.Add(charData);
            }
            var saveData = new PlayerProgressSerializableData(
                playerCharactersData.ToArray(), 
                progress.StatsUpgrades, progress.Currencies);
            return saveData;
        }

        public IPlayerProgressData UnpackSaveData(PlayerProgressSerializableData saveData)
        {
            AssetIdsMappings.RefreshMappings();
            var playerCharacters = saveData.PlayerCharacters
                .Select(c => GameCharacterSerializer.UnpackCharacterFromSaveData(
                    c, AssetIdsMappings.CharactersMapping))
                .ToArray();
            return new PlayerProgressData(
                playerCharacters, saveData.StatsUpgrades, saveData.Currencies);
        }
    }
}
