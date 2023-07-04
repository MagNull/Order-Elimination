using Inventory_Items;

namespace Inventory.Items
{
    public static class ItemFactory
    {
        public static Item Create(ItemData data)
        {
            return data.Type switch
            {
                ItemType.Consumable => new ConsumableItem(data),
                ItemType.Equipment => new EquipmentItem(data),
                _ => null
            };
        }
    }
}