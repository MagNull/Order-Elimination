using Newtonsoft.Json;
using OrderElimination.Infrastructure;
using System;

namespace OrderElimination.SavesManagement
{
    public class SaveDataPacker
    {
        private readonly IDataMapping<Guid, IGameCharacterTemplate> _charactersMapping;

        public SaveDataPacker(IDataMapping<Guid, IGameCharacterTemplate> charactersMapping)
        {
            _charactersMapping = charactersMapping;
        }

        public string PackSaveData(
            IPlayerProgress progress)
        {
            var characterConverter = new GameCharacterJsonConverter(_charactersMapping);
            return JsonConvert.SerializeObject(
                progress, Formatting.Indented, characterConverter);
        }

        public IPlayerProgress UnpackSaveData(string saveData)
        {
            var characterConverter = new GameCharacterJsonConverter(_charactersMapping);
            return JsonConvert.DeserializeObject<PlayerProgress>(
                saveData, characterConverter);
        }
    }
}
