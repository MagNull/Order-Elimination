using System;
using System.Collections.Generic;
using System.Linq;
using GameInventory.Items;
using OrderElimination;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameInventory
{
    [Serializable]
    public class Inventory
    {
        public event Action<IReadOnlyCell> OnCellChanged;

        [ShowInInspector, SerializeField]
        private Cell[] _cells;

        public Inventory(int size)
        {
            _cells = new Cell[size];
            for (var i = 0; i < size; i++)
            {
                _cells[i] = new Cell(new EmptyItem());
            }
        }

        public IReadOnlyList<IReadOnlyCell> Cells => _cells;

        public void AddItem(Item item, int index = -1)
        {
            if (item == null)
                Logging.LogException(new ArgumentException("Item can't be null"));

            var availableCellIndex = index == -1 ? GetFirstAvailableCellIndex() : index;
            if (availableCellIndex == -1)
            {
                Logging.LogWarning("Inventory is full");
                return;
            }

            _cells[availableCellIndex].SetItem(item);
            if (item is ConsumableItem consumableItem)
                consumableItem.UseTimesOver += OnConsumableItemOver;
            OnCellChanged?.Invoke(_cells[availableCellIndex]);
        }

        public void RemoveItem(Item item)
        {
            if (item == null)
                Logging.LogException(new ArgumentException("Item can't be null"));

            var removeItemCell = _cells.FirstOrDefault(cell => cell.Item == item);
            if (removeItemCell == null)
            {
                Logging.LogWarning("Not found item in inventory");
                return;
            }

            removeItemCell.SetItem(new EmptyItem());
            OnCellChanged?.Invoke(removeItemCell);
        }

        public void RemoveItem(ItemData itemData)
        {
            if(itemData == null)
                Logging.LogException(new ArgumentException("Item data can't be null"));
            
            var indexOfItem = _cells.FindIndex(cell => cell.Item.Data.Id == itemData.Id);
            if (indexOfItem == -1)
            {
                Logging.LogWarning("Not found item in inventory");
                return;
            }

            var removedCell = _cells[indexOfItem];
            _cells.RemoveAt(indexOfItem);
            OnCellRemoved?.Invoke(removedCell);
        }

        public List<Item> GetItems()
        {
            var result = new List<Item>();
            foreach (var cell in _cells)
            {
                if (cell.Item is EmptyItem)
                    continue;

                result.Add(cell.Item);
            }

            return result;
        }

        public bool Contains(ItemData itemData)
        {
            return _cells.Any(cell => cell.Item.Data.Id == itemData.Id);
        }

        public void InitConsumables()
        {
            foreach (var item in GetItems())
            {
                if (item is ConsumableItem consumableItem)
                {
                    consumableItem.UseTimesOver += OnConsumableItemOver;
                }
            }
        }

        public void MoveItemTo(Item item, Inventory other, int index = -1)
        {
            RemoveItem(item);
            other.AddItem(item, index);
        }

        private void OnConsumableItemOver(ConsumableItem item)
        {
            item.UseTimesOver -= OnConsumableItemOver;
            RemoveItem(item);
        }

        private int GetFirstAvailableCellIndex()
        {
            for (var i = 0; i < _cells.Length; i++)
            {
                if (_cells[i].Item is EmptyItem)
                    return i;
            }

            return -1;
        }
    }
}