using System;
using System.Collections.Generic;
using System.Linq;
using GameInventory.Items;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace GameInventory.Views
{
    public class PlayerInventoryView : InventoryView
    {
        public override event Action<IReadOnlyCell> CellClicked;

        [OdinSerialize]
        protected List<InventoryCellView> _cellViews = new();

        [SerializeField]
        private InventoryCellView _cellViewPrefab;

        [SerializeField]
        private GridLayoutGroup _cellContainer;

        private readonly Stack<InventoryCellView> _unusedCellViews = new();

        public void ShowItemWithType(int itemType)
        {
            foreach (var cell in _cellViews)
            {
                if (cell.Model.Item == null)
                    continue;
                if (cell.Model.Item.Data.Type == (ItemType)itemType)
                    cell.Enable();
                else
                    cell.Disable();
            }
        }

        public void ShowAllItems()
        {
            foreach (var cell in _cellViews)
            {
                cell.Enable();
            }
        }

        public override void UpdateCells(IReadOnlyList<IReadOnlyCell> cells)
        {
            foreach (var cell in cells.Where(cell => _cellViews.All(c => c.Model != cell)))
                OnCellAdded(cell);
            
            //Trash
            var viewCopy = new InventoryCellView[_cellViews.Count];
            _cellViews.CopyTo(viewCopy);
            foreach (var view in viewCopy)
            {
                if (!cells.Contains(view.Model))
                    OnCellRemoved(view.Model);
            }
        }

        public override void OnCellAdded(IReadOnlyCell cell)
        {
            var cellView = _unusedCellViews.Count > 0
                ? _unusedCellViews.Pop()
                : Instantiate(_cellViewPrefab, _cellContainer.transform);
            
            cellView.Init(cell);
            cellView.Clicked += OnCellClicked;
            cellView.Enable();
            _cellViews.Add(cellView);
        }

        public override void OnCellRemoved(IReadOnlyCell cell)
        {
            var cellView = _cellViews.Find(c => c.Model == cell);
            _cellViews.Remove(cellView);
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