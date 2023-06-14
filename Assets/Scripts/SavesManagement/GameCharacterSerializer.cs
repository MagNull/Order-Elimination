using Newtonsoft.Json;
using OrderElimination.MetaGame;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

namespace OrderElimination.SavesManagement
{
    public static class GameCharacterSerializer
    {
        public static string PlayerCharactersFileSavePath { get; }
        //fileExtension = .oegc//order elimination game character

        public static async Task SaveCharacter(GameCharacter character)
        {
            throw new NotImplementedException();
            if (!Directory.Exists(PlayerCharactersFileSavePath))
                Directory.CreateDirectory(PlayerCharactersFileSavePath);
            var filesCount = Directory.GetFiles(PlayerCharactersFileSavePath);
            var filename = Path.Combine(PlayerCharactersFileSavePath, $"playercharacter{filesCount}");
            var fileStream = new FileStream(filename, FileMode.CreateNew);
            var formatter = new BinaryFormatter();
            var data = new GameCharacterSaveData();
            formatter.Serialize(fileStream, character);
            fileStream.Close();
        }

        public static async Task<GameCharacter[]> LoadPlayerCharacters()
        {
            throw new NotImplementedException();
            foreach (var file in Directory.EnumerateFiles(PlayerCharactersFileSavePath))
            {
                var fileStream = new FileStream(file, FileMode.Open);
            }

            throw new NotImplementedException();
        }
    }
}
