using System;
using System.Collections.Generic;
using Inventory;
using Inventory_Items;
using Sirenix.OdinInspector;
using OrderElimination;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemsLibrary
{
    [CreateAssetMenu(fileName = "Library", menuName = "Library/LibraryInstance")]
    public class Library : SerializedScriptableObject
    {
        [SerializeField]
        private Dictionary<ItemType, List<ItemView>> _addedItems = new();
        private HashSet<int> _allItemsIds = new();
        public IReadOnlyCollection<int> GetAllItemIds => _allItemsIds;
        public IReadOnlyList<ItemView> GetItems(ItemType type) => _addedItems[type];

        public Library()
        {
            _addedItems[ItemType.Consumable] = new List<ItemView>();
            _addedItems[ItemType.Equipment] = new List<ItemView>();
            _addedItems[ItemType.Modificator] = new List<ItemView>();
            
            Logging.Log("Initialize library");
        }

        public void AddItem(Inventory_Items.IReadOnlyCell cell)
        {
            if (cell == null)
                Logging.LogException( new ArgumentException("Item can't be null."));
            
            AddItem(cell.Item);
        }

        public void AddItem(IReadOnlyList<ItemData> itemsList)
        {
            if (itemsList == null)
                Logging.LogException( new ArgumentException("Item can't be null."));
            foreach (var item in itemsList)
                AddItem(item);
        }

        private void AddItem(Item item)
        {
            if (item == null)
                Logging.LogException( new ArgumentException("Item can't be null."));
            
            if (!_allItemsIds.Contains(item.Id))
            {
                Logging.Log("Item added:" + item.View.Name);
                _addedItems[item.Type].Add(item.View);
                _allItemsIds.Add(item.Id);
            }
        }

        private void AddItem(ItemData data)
        {
            if (data == null)
                Logging.LogException( new ArgumentException("Item data can't be null."));
            
            if (!_allItemsIds.Contains(data.ItemId))
            {
                Logging.Log("Item added:" + data.ItemView.Name);
                _addedItems[data.ItemType].Add(data.ItemView);
                _allItemsIds.Add(data.ItemId);
            }
        }
    }    
}

