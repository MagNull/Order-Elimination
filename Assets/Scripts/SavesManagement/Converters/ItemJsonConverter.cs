using GameInventory.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace OrderElimination.SavesManagement
{
    public class ItemJsonConverter : JsonConverter<Item>
    {
        private readonly string _itemDataToken = nameof(Item.Data);
        private readonly string _consumesToken = nameof(ConsumableItem.ConsumesLeft);

        public override Item ReadJson(
            JsonReader reader, Type objectType,
            Item existingValue, bool hasExistingValue, 
            JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var itemData = jo[_itemDataToken].ToObject<ItemData>(serializer);
            Item item;
            if (itemData.Type == ItemType.Consumable)
            {
                var consumesLeft = jo[_consumesToken].ToObject<int>(serializer);
                if (itemData is HealItemData healItemData)
                {
                    item = new HealItem(healItemData, consumesLeft);
                }
                else
                {
                    item = new ConsumableItem(itemData, consumesLeft);
                }
            }
            else
            {
                item = ItemFactory.Create(itemData);
            }
            return item;
        }

        public override void WriteJson(
            JsonWriter writer, Item value, JsonSerializer serializer)
        {
            var jo = new JObject();

            var itemData = value.Data;
            jo[_itemDataToken] = JToken.FromObject(itemData, serializer);
            if (value is ConsumableItem consumable)
            {
                jo[_consumesToken] = consumable.ConsumesLeft;
            }

            jo.WriteTo(writer);
        }
    }
}
