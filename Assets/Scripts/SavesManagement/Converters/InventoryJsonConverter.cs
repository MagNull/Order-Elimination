using GameInventory;
using GameInventory.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace OrderElimination.SavesManagement
{
    public class InventoryJsonConverter : JsonConverter<Inventory>
    {
        private readonly string _sizeToken = nameof(Inventory.Size);
        private readonly string _itemsToken = "Items";

        public override Inventory ReadJson(
            JsonReader reader, Type objectType, 
            Inventory existingValue, bool hasExistingValue, 
            JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var size = jo[_sizeToken].ToObject<int>(serializer);
            var items = jo[_itemsToken].ToObject<Item[]>(serializer);
            if (size < items.Length)
                throw new InvalidOperationException("Inventory size is less than items count");
            var inventory = new Inventory(size);
            foreach (var item in items )
            {
                inventory.AddItem(item);
            }
            return inventory;
        }

        public override void WriteJson(
            JsonWriter writer, Inventory value, JsonSerializer serializer)
        {
            var jo = new JObject();

            jo[_sizeToken] = value.Size;
            jo[_itemsToken] = JToken.FromObject(value.GetItems(), serializer);

            jo.WriteTo(writer);
        }
    }
}
