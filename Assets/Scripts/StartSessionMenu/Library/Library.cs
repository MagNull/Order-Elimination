using System;
using System.Collections.Generic;
using Inventory;
using UnityEngine;

namespace ItemsLibrary
{
    [CreateAssetMenu(fileName = "Library", menuName = "Library/LibraryInstance")]
    public class Library : ScriptableObject
    {
        private Dictionary<ItemType, List<ItemView>> _addedItems;
        private HashSet<int> _allItemsIndexes;
        public IReadOnlyCollection<int> GetAllItemIndexes => _allItemsIndexes;
        public IReadOnlyList<ItemView> GetItems(ItemType type) => _addedItems[type];

        public Library()
        {
            _allItemsIndexes = new HashSet<int>();
            _addedItems = new Dictionary<ItemType, List<ItemView>>();
            
            _addedItems[ItemType.Consumable] = new List<ItemView>();
            _addedItems[ItemType.Equipment] = new List<ItemView>();
            _addedItems[ItemType.Modificator] = new List<ItemView>();
        }

        public void AddItem(Inventory.IReadOnlyCell cell)
        {
            if (cell == null)
                throw new ArgumentException("Item can't be null.");
            
            if (!_allItemsIndexes.Contains(cell.Item.Index))
            {
                _addedItems[cell.Item.Type].Add(cell.Item.View);
                _allItemsIndexes.Add(cell.Item.Index);
            }
        }
    }    
}

