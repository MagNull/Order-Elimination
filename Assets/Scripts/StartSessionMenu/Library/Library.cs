using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using UnityEngine;

namespace ItemsLibrary
{
    public enum LibraryItemType
    {
        Consumable,
        Equipment,
        Modificator,
        
    }
    public class Library
    {
        private HashSet<Item> _addedItems = new HashSet<Item>();
        public IReadOnlyList<Item> GetConsumables => _addedItems.Where(x => x.Type == ItemType.Consumable).ToList();
        public IReadOnlyList<Item> GetEquipments => _addedItems.Where(x => x.Type == ItemType.Equipment).ToList();
        public IReadOnlyList<Item> GetModificators => _addedItems.Where(x => x.Type == ItemType.Modificator).ToList();
        
        public void AddItemFromInventory(Inventory.IReadOnlyCell cell)
        {
            if (cell == null)
                throw new ArgumentException("Item can't be null.");
            _addedItems.Add(cell.Item);
        }

        public IReadOnlyList<Item> GetItems(LibraryItemType type)
        {
            switch (type)
            {
                case LibraryItemType.Consumable:
                    return GetConsumables;
                case LibraryItemType.Equipment:
                    return GetEquipments;
                case LibraryItemType.Modificator:
                    return GetModificators;
                default:
                    return new List<Item>();
            }
        }

    }    
}

