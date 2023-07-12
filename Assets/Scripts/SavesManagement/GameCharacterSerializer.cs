using Newtonsoft.Json;
using OrderElimination.MacroGame;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OrderElimination.SavesManagement
{
    public static class GameCharacterSerializer
    {
        public static IFormatter Formatter { get; private set; } = new BinaryFormatter();
        public static string CharacterFileExtension => ".oechar";
        public static string PlayerCharacterSavesPath => Path.Combine(Application.persistentDataPath, "Saves", "PlayerCharacters");

        public static void SaveCharacter(
            GameCharacter character, IDataMapping<int, IGameCharacterTemplate> templatesMapping)
        {
            if (!Directory.Exists(PlayerCharacterSavesPath))
                Directory.CreateDirectory(PlayerCharacterSavesPath);
            var filesCount = Directory.GetFiles(PlayerCharacterSavesPath, $"*{CharacterFileExtension}").Length;
            var filename = Path.Combine(PlayerCharacterSavesPath, $"playercharacter{filesCount}{CharacterFileExtension}");

            var characterTemplateId = templatesMapping.GetKey(character.CharacterData);
            var stats = new GameCharacterStats(character.CharacterStats);
            var data = new GameCharacterSaveData(
                characterTemplateId, stats, character.CurrentHealth, character.Inventory);

            var fileStream = new FileStream(filename, FileMode.CreateNew);
            Formatter.Serialize(fileStream, data);
            fileStream.Close();
        }

        public static void SaveCharacters(
            IEnumerable<GameCharacter> characters, 
            IDataMapping<int, IGameCharacterTemplate> templatesMapping)
        {
            foreach (var character in characters)
            {
                SaveCharacter(character, templatesMapping);
            }
        }

        public static GameCharacter[] LoadPlayerCharacters(
            IDataMapping<int, IGameCharacterTemplate> templatesMapping)
        {
            if (!Directory.Exists(PlayerCharacterSavesPath))
            {
                Logging.LogError(new DirectoryNotFoundException($"Saved characters directory wasn't found at {PlayerCharacterSavesPath}. Creating a new one."));
                Directory.CreateDirectory(PlayerCharacterSavesPath);
            }
                
            var restoredCharacters = new List<GameCharacter>();
            foreach (var file in Directory.EnumerateFiles(PlayerCharacterSavesPath, $"*{CharacterFileExtension}"))
            {
                var fileStream = new FileStream(file, FileMode.Open);
                var saveData = (GameCharacterSaveData)Formatter.Deserialize(fileStream);
                var template = templatesMapping.GetData(saveData.BasedTemplateId);
                var character = GameCharactersFactory.RestoreGameCharacter(
                    template, saveData.CharacterStats, saveData.CurrentHealth);
                restoredCharacters.Add(character);
            }
            return restoredCharacters.ToArray();
        }
    }
}
