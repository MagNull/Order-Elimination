using System;
using System.Collections.Generic;
using Inventory;
using Inventory_Items;
using UnityEngine;

namespace ItemsLibrary
{
    [CreateAssetMenu(fileName = "Library", menuName = "Library/LibraryInstance")]
    public class Library : ScriptableObject
    {
        private Dictionary<ItemType, List<ItemView>> _addedItems = new Dictionary<ItemType, List<ItemView>>();
        private HashSet<int> _allItemsIds = new HashSet<int>();
        public IReadOnlyCollection<int> GetAllItemIds => _allItemsIds;
        public IReadOnlyList<ItemView> GetItems(ItemType type) => _addedItems[type];

        public Library()
        {
            _addedItems[ItemType.Null] = new List<ItemView>();
            _addedItems[ItemType.Consumable] = new List<ItemView>();
            _addedItems[ItemType.Equipment] = new List<ItemView>();
            _addedItems[ItemType.Modificator] = new List<ItemView>();
            
            Debug.Log("Initialize library");
        }

        public void AddItem(Inventory_Items.IReadOnlyCell cell)
        {
            if (cell == null)
                throw new ArgumentException("Item can't be null.");
            
            AddItem(cell.Item);
        }

        public void AddItem(IReadOnlyList<ItemData> itemsList)
        {
            if (itemsList == null)
                throw new ArgumentException("Item can't be null.");
            foreach (var item in itemsList)
                AddItem(item);
        }

        private void AddItem(Item item)
        {
            if (item == null)
                throw new ArgumentException("Item can't be null.");
            
            if (!_allItemsIds.Contains(item.Id))
            {
                Debug.Log("Item added:" + item.View.Name);
                _addedItems[item.Type].Add(item.View);
                _allItemsIds.Add(item.Id);
            }
        }

        private void AddItem(ItemData data)
        {
            if (data == null)
                throw new ArgumentException("Item data can't be null.");
            
            if (!_allItemsIds.Contains(data.ItemId))
            {
                Debug.Log("Item added:" + data.ItemView.Name);
                _addedItems[data.ItemType].Add(data.ItemView);
                _allItemsIds.Add(data.ItemId);
            }
        }
    }    
}

