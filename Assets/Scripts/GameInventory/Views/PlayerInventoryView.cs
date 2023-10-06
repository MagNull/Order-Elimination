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

        public void ShowItemWithType(int itemType)
        {
            foreach (var cell in _cellViews)
            {
                if (cell.Model.Item is EmptyItem)
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
            if (_cellViews.Count == 0) FillInventoryViews();

            for(var i = 0; i < _cellViews.Count; i++)
            {
                if(i < cells.Count)
                    _cellViews[i].Init(cells[i]);
                else
                    _cellViews[i].Disable();
            }
        }

        public override void OnCellChanged(IReadOnlyCell cell)
        {
            foreach (var cellView in _cellViews.Where(cellView => cellView.Model == cell))
            {
                cellView.UpdateView();
                return;
            }
        }

        private void FillInventoryViews()
        {
            for (var i = 0; i < 100; i++)
            {
                var cellView = Instantiate(_cellViewPrefab, _cellContainer.transform);
                cellView.Disable();
                cellView.Clicked += OnCellClicked;
                _cellViews.Add(cellView);
            }
        }

        private void OnCellClicked(IReadOnlyCell cell)
        {
            CellClicked?.Invoke(cell);
        }
    }
}