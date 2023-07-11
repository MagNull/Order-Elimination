using Newtonsoft.Json;
using OrderElimination.MacroGame;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Collections.Generic;

namespace OrderElimination.SavesManagement
{
    public static class GameCharacterSerializer
    {
        public static string CharacterFileExtension => ".oechar";
        public static string PlayerCharacterSavesPath => @"C:\Users\Fakumen\Documents\Projects\Order-Elimination\Assets\Saves";
        //fileExtension = .oegc//order elimination game character

        public static void SaveCharacter(GameCharacter character)
        {
            if (!Directory.Exists(PlayerCharacterSavesPath))
                Directory.CreateDirectory(PlayerCharacterSavesPath);
            var filesCount = Directory.GetFiles(PlayerCharacterSavesPath, CharacterFileExtension).Length;
            var filename = Path.Combine(PlayerCharacterSavesPath, $"playercharacter{filesCount}{CharacterFileExtension}");

            var characterTemplateId = character.CharacterData.TemplateId;
            var unityObject = Resources.InstanceIDToObject(characterTemplateId);
            if (unityObject is not CharacterTemplate characterTemplate)
                throw new NotSupportedException($"Unknown implementation of {nameof(IGameCharacterTemplate)}.");
            var stats = new GameCharacterStats(character.CharacterStats);
            var data = new GameCharacterSaveData(characterTemplateId, stats, character.CurrentHealth);

            var fileStream = new FileStream(filename, FileMode.CreateNew);
            var formatter = new BinaryFormatter();
            formatter.Serialize(fileStream, data);
            fileStream.Close();
        }

        public static void SaveCharacters(IEnumerable<GameCharacter> characters)
        {
            foreach (var character in characters)
            {
                SaveCharacter(character);
            }
        }

        public static GameCharacter[] LoadPlayerCharacters()
        {
            if (!Directory.Exists(PlayerCharacterSavesPath))
                throw new DirectoryNotFoundException($"Saved characters directory wasn't found at {PlayerCharacterSavesPath}");
            var formatter = new BinaryFormatter();
            var restoredCharacters = new List<GameCharacter>();
            foreach (var file in Directory.EnumerateFiles(PlayerCharacterSavesPath))
            {
                var fileStream = new FileStream(file, FileMode.Open);
                var saveData = (GameCharacterSaveData)formatter.Deserialize(fileStream);
                var template = (IGameCharacterTemplate)Resources.Load(file);
                var character = GameCharactersFactory.RestoreGameCharacter(
                    template, saveData.CharacterStats, saveData.CurrentHealth);
                restoredCharacters.Add(character);
            }
            return restoredCharacters.ToArray();
        }
    }
}
