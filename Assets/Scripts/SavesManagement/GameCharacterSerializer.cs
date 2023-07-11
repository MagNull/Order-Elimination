using Newtonsoft.Json;
using OrderElimination.MacroGame;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace OrderElimination.SavesManagement
{
    public static class GameCharacterSerializer
    {
        public static string PlayerCharactersFileSavePath { get; }
        //fileExtension = .oegc//order elimination game character

        public static async Task SaveCharacter(GameCharacter character)
        {
            Logging.LogException(new NotImplementedException());
            if (!Directory.Exists(PlayerCharactersFileSavePath))
                Directory.CreateDirectory(PlayerCharactersFileSavePath);
            var filesCount = Directory.GetFiles(PlayerCharactersFileSavePath);
            var filename = Path.Combine(PlayerCharactersFileSavePath, $"playercharacter{filesCount}");
            var fileStream = new FileStream(filename, FileMode.CreateNew);
            var formatter = new BinaryFormatter();

            var characterTemplatePath = "";
            var stats = new GameCharacterStats(character.CharacterStats);
            var data = new GameCharacterSaveData(characterTemplatePath, stats, character.CurrentHealth);
            formatter.Serialize(fileStream, data);
            fileStream.Close();
        }

        public static async Task<GameCharacter[]> LoadPlayerCharacters()
        {
            Logging.LogException(new NotImplementedException());
            foreach (var file in Directory.EnumerateFiles(PlayerCharactersFileSavePath))
            {
                var fileStream = new FileStream(file, FileMode.Open);
            }

            Logging.LogException(new NotImplementedException());
            throw new NotImplementedException();
        }
    }
}
