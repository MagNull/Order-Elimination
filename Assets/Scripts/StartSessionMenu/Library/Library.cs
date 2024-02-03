using System;
using System.Collections.Generic;
using GameInventory.Items;
using Sirenix.OdinInspector;
using OrderElimination;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace ItemsLibrary
{
    [CreateAssetMenu(fileName = "Library", menuName = "Library/LibraryInstance")]
    public class Library : SerializedScriptableObject
    {
        [SerializeField]
        private Dictionary<ItemType, List<ItemView>> _addedItems = new();
        private readonly HashSet<Guid> _allItemsIds = new();
        public IReadOnlyList<ItemView> GetItems(ItemType type) => _addedItems[type];

        public Library()
        {
            _addedItems[ItemType.Consumable] = new List<ItemView>();
            _addedItems[ItemType.Equipment] = new List<ItemView>();
        }

        public void AddItem(GameInventory.IReadOnlyCell cell)
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
            
            // if (!_allItemsIds.Contains(item.Data.Id))
            // {
            //     Logging.Log("Item added:" + item.Data.View.Name);
            //     _addedItems[item.Data.Type].Add(item.Data.View);
            //     _allItemsIds.Add(item.Data.Id);
            // }
            Logging.LogWarning("Rework library");
        }

        private void AddItem(ItemData data)
        {
            if (data == null)
                Logging.LogException( new ArgumentException("Item data can't be null."));
            
            if (!_allItemsIds.Contains(data.AssetId))
            {
                Logging.Log("Item added:" + data.View.Name);
                _addedItems[data.Type].Add(data.View);
                _allItemsIds.Add(data.AssetId);
            }
        }
    }    
}

