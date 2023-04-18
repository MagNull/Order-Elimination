using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Inventory_Items
{
    [Serializable]
    public class Inventory
    {
        public event Action<IReadOnlyCell, IReadOnlyCell> OnCellChanged;
        public event Action<IReadOnlyCell> OnCellRemoved;
        public event Action<IReadOnlyCell> OnCellAdded;
        
        [ShowInInspector]
        private readonly List<Cell> _cells;
        private readonly int _size;

        public Inventory(int size)
        {
            _cells = new List<Cell>(size);
            _size = size;
        }

        public IReadOnlyList<IReadOnlyCell> Cells => _cells;

        public void AddItem(Item item, int quantity = 1)
        {
            if (item == null)
                throw new ArgumentException("Item can't be null");

            for (var i = 0; i < quantity; i++)
            {
                if (_cells.Count >= _size)
                {
                    Debug.LogWarning("Inventory is full");
                    return;
                }

                var newCell = new Cell(item, quantity);
                _cells.Add(newCell);
                OnCellAdded?.Invoke(newCell);
            }
        }

        //TODO: Refactor to delete empty cells
        public void RemoveItem(Item item, int quantity = 1)
        {
            if (item == null)
                throw new ArgumentException("Item can't be null");

            var indexOfItem = _cells.FindIndex(cell => cell.Item == item);
            if (indexOfItem == -1)
            {
                Debug.LogWarning("Not found item in inventory");
                return;
            }

            if (_cells[indexOfItem].ItemQuantity - quantity <= 0)
            {
                OnCellRemoved?.Invoke(_cells[indexOfItem]);
                _cells.RemoveAt(indexOfItem);
                return;
            }
            var newCell = new Cell(item, _cells[indexOfItem].ItemQuantity - quantity);
            OnCellChanged?.Invoke(_cells[indexOfItem], newCell);
            _cells[indexOfItem] = newCell;
        }

        public void MoveItemTo(Item item, Inventory other)
        {
            RemoveItem(item);
            other.AddItem(item);
        }
    }
}