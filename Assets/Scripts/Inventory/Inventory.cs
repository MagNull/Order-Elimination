﻿using System;
using System.Collections.Generic;
using OrderElimination;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Inventory_Items
{
    [Serializable]
    public class Inventory
    {
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

        public void AddItem(Item item)
        {
            if (item == null)
                Logging.LogException(new ArgumentException("Item can't be null"));

            if (_cells.Count >= _size)
            {
                Logging.LogWarning("Inventory is full");
                return;
            }

            var newCell = new Cell(item);
            _cells.Add(newCell);
            OnCellAdded?.Invoke(newCell);
        }

        public void RemoveItem(Item item)
        {
            if (item == null)
                Logging.LogException(new ArgumentException("Item can't be null"));

            var indexOfItem = _cells.FindIndex(cell => cell.Item == item);
            if (indexOfItem == -1)
            {
                Logging.LogWarning("Not found item in inventory");
                return;
            }

            OnCellRemoved?.Invoke(_cells[indexOfItem]);
            _cells.RemoveAt(indexOfItem);
        }

        public List<Item> GetItems()
        {
            var result = new List<Item>();
            foreach (var cell in _cells)
            {
                if (cell.Item == null)
                    continue;
                
                result.Add(cell.Item);
            }

            return result;
        }

        public void MoveItemTo(Item item, Inventory other)
        {
            RemoveItem(item);
            other.AddItem(item);
        }
    }
}