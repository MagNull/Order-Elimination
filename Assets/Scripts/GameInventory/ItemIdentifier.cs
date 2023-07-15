using System;
using System.Collections.Generic;
using System.Linq;
using GameInventory.Items;
using UnityEngine;

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

        public static void RecoverID()
        {
            var items = Resources.FindObjectsOfTypeAll<ItemData>();
            foreach (var itemData in items)
            {
                _itemsById[itemData] = itemData.Id;
            }
        }
        
        public static ItemData[] GetItems() => _itemsById.Keys.ToArray();
    }
}