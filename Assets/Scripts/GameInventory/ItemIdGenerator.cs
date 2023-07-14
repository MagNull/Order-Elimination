using System;
using System.Collections.Generic;
using GameInventory.Items;

namespace GameInventory
{
    public static class ItemIdGenerator
    {
        private static Dictionary<ItemData, string> _itemsById = new();

        public static string GetID(ItemData itemData)
        {
            if(!_itemsById.ContainsKey(itemData))
                _itemsById.Add(itemData, Guid.NewGuid().ToString());
            return _itemsById[itemData];
        }
    }
}