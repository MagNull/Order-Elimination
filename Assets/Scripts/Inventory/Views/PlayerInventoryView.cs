using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory_Items
{
    public class PlayerInventoryView : InventoryView
    {
        public override event Action<IReadOnlyCell> CellClicked;

        private readonly Dictionary<IReadOnlyCell, InventoryCellView> _cells = new();

        [SerializeField]
        private InventoryCellView _cellViewPrefab;

        [SerializeField]
        private GridLayoutGroup _cellContainer;

        private readonly Stack<InventoryCellView> _unusedCellViews = new();

        public void ShowItemWithType(int itemType)
        {
            foreach (var cell in _cells)
            {
                if (cell.Key.Item == null)
                    continue;
                if (cell.Key.Item.Type == (ItemType)itemType)
                    cell.Value.Enable();
                else
                    cell.Value.Disable();
            }
        }

        public void ShowAllItems()
        {
            foreach (var cell in _cells)
            {
                cell.Value.Enable();
            }
        }

        public override void UpdateCells(IReadOnlyList<IReadOnlyCell> cells)
        {
            foreach (var cell in cells.Where(cell => !_cells.ContainsKey(cell)))
                OnCellAdded(cell);
        }
        

        public override void OnCellAdded(IReadOnlyCell cell)
        {
            var cellView = _unusedCellViews.Count > 0
                ? _unusedCellViews.Pop()
                : Instantiate(_cellViewPrefab, _cellContainer.transform);
            
            cellView.Init(cell);
            cellView.Clicked += OnCellClicked;
            cellView.Enable();
            _cells.Add(cell, cellView);
        }

        public override void OnCellRemoved(IReadOnlyCell cell)
        {
            var cellView = _cells[cell];
            _cells.Remove(cell);
            cellView.Disable();
            _unusedCellViews.Push(cellView);
            cellView.Clicked -= OnCellClicked;
        }

        private void OnCellClicked(IReadOnlyCell cell)
        {
            CellClicked?.Invoke(cell);
        }
    }
}