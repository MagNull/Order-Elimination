using System;
using System.Collections.Generic;
using System.Linq;
using GameInventory.Items;

namespace GameInventory
{
    public static class ItemIdentifier
    {
        private static readonly Dictionary<ItemData, string> _itemsById = new();

        public static string GetID(ItemData itemData)
        {
            if(!_itemsById.ContainsKey(itemData))
                _itemsById.Add(itemData, Guid.NewGuid().ToString());
            return _itemsById[itemData];
        }

        public static void RecoverID(ItemData itemData, string id)
        {
            _itemsById[itemData] = id;
        }
        
        public static ItemData[] GetItems() => _itemsById.Keys.ToArray();
    }
}