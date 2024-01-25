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
        public int Size => _cells.Length;

        public event Action<IReadOnlyCell> OnCellChanged;

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
            RemoveItemAtCell(removeItemCell);
        }

        public void RemoveItem(ItemData itemData)
        {
            if(itemData == null)
                Logging.LogException(new ArgumentException("Item data can't be null"));
            
            var removeItemCell = _cells.FirstOrDefault(cell => cell.Item.Data.Id == itemData.Id);
            if (removeItemCell == null)
            {
                Logging.LogWarning("Not found item in inventory");
                return;
            }
            RemoveItemAtCell(removeItemCell);
        }

        public Item[] GetItems()
        {
            return _cells
                .Select(cell => cell.Item)
                .Where(i => i != null && i is not EmptyItem).ToArray();
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

        private void RemoveItemAtCell(Cell cell)
        {
            if (cell == null)
                throw new ArgumentNullException(nameof(cell));
            var item = cell.Item;
            cell.SetItem(new EmptyItem());
            if (item is ConsumableItem consumableItem)
                consumableItem.UseTimesOver -= OnConsumableItemOver;
            OnCellChanged?.Invoke(cell);
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