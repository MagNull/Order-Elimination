using GameInventory.Items;
using Newtonsoft.Json;
using OrderElimination.Infrastructure;
using System;

namespace OrderElimination.SavesManagement
{
    public class SaveDataPacker
    {
        private readonly IDataMapping<Guid, IGameCharacterTemplate> _charactersMapping;
        private readonly IDataMapping<Guid, ItemData> _itemsMapping;

        public SaveDataPacker(
            IDataMapping<Guid, IGameCharacterTemplate> charactersMapping, 
            IDataMapping<Guid, ItemData> itemsMapping)
        {
            _charactersMapping = charactersMapping;
            _itemsMapping = itemsMapping;
        }

        public string PackSaveData(IPlayerProgress progress)
        {
            return JsonConvert.SerializeObject(progress, Formatting.Indented, GetConverters());
        }

        public IPlayerProgress UnpackSaveData(string saveData)
        {
            return JsonConvert.DeserializeObject<PlayerProgress>(saveData, GetConverters());
        }

        private JsonConverter[] GetConverters()
        {
            var characterTemplateConverter = new CharacterTemplateJsonConverter(_charactersMapping);
            var characterConverter = new GameCharacterJsonConverter(_charactersMapping);
            var itemDataConverter = new ItemDataJsonConverter(_itemsMapping);
            var itemConverter = new ItemJsonConverter();
            var inventoryConverter = new InventoryJsonConverter();
            return new JsonConverter[]
            {
                characterTemplateConverter, 
                characterConverter, 
                itemDataConverter, 
                itemConverter, 
                inventoryConverter 
            };
        }
    }
}
