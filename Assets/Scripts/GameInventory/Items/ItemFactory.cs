﻿using System;

namespace GameInventory.Items
{
    public static class ItemFactory
    {
        public static Item Create(ItemData data)
        {
            return data.Type switch
            {
                ItemType.Consumable => data is HealItemData healItemData
                    ? new HealItem(healItemData)
                    : new ConsumableItem(data),
                ItemType.Equipment => new EquipmentItem(data),
                ItemType.Others => new Item(data),
                _ => throw new NotImplementedException()
            };
        }
    }
}