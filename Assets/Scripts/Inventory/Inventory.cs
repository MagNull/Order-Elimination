using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class Inventory
    {
        public event Action<Cell, Cell> OnCellChanged;
        public event Action<Cell> OnCellAdded;
        
        private readonly List<Cell> _cells;
        private readonly int _size;

        public Inventory(int size)
        {
            _cells = new List<Cell>(size);
            _size = size;
        }

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

            var newCell = new Cell(item, _cells[indexOfItem].ItemQuantity - quantity);
            OnCellChanged?.Invoke(_cells[indexOfItem], newCell);
            _cells[indexOfItem] = newCell;
        }
    }
}