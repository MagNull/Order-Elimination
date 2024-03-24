using GameInventory.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrderElimination.GameContent;
using OrderElimination.Infrastructure;
using System;

namespace OrderElimination.SavesManagement
{
    public class ItemDataJsonConverter : JsonConverter<ItemData>
    {
        private readonly string _templateToken = nameof(IGuidAsset.AssetId);
        private readonly IDataMapping<Guid, ItemData> _templatesMap;

        public ItemDataJsonConverter(IDataMapping<Guid, ItemData> templatesMap)
        {
            _templatesMap = templatesMap;
        }

        public override ItemData ReadJson(
            JsonReader reader, Type objectType,
            ItemData existingValue, bool hasExistingValue, 
            JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var id = jo[_templateToken].ToObject<Guid>();
            var itemData = _templatesMap.GetData(id);
            return itemData;
        }

        public override void WriteJson(JsonWriter writer, ItemData value, JsonSerializer serializer)
        {
            if (value == null)
                return;

            var jo = new JObject();
            var id = _templatesMap.GetKey(value);
            jo[_templateToken] = id;

            jo.WriteTo(writer);
        }
    }
}
