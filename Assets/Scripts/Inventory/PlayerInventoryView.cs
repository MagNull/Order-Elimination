using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class PlayerInventoryView : SerializedMonoBehaviour
    {
        [SerializeField]
        private readonly Dictionary<IReadOnlyCell, IInventoryCellView> _cells = new();
        [SerializeField]
        private InventoryCellFullView _cellViewPrefab;
        [SerializeField]
        private GridLayoutGroup _cellContainer;

        public void ShowItemWithType(int itemType)
        {
            foreach (var cell in _cells)
            {
                if(cell.Key.Item.Type == (ItemType) itemType)
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

        public void UpdateCells(IReadOnlyList<IReadOnlyCell> cells)
        {
            foreach (var cell in cells.Where(cell => !_cells.ContainsKey(cell)))
                OnCellAdded(cell);
        }

        public void OnCellChanged(IReadOnlyCell oldCell, IReadOnlyCell newCell)
        {
            var cellView = _cells[oldCell];
            _cells.Remove(oldCell);
            cellView.OnCellChanged(newCell);

            if (newCell.ItemQuantity == 0)
                return;
            _cells.Add(newCell, cellView);
        }

        public void OnCellAdded(IReadOnlyCell cell)
        {
            var cellView = Instantiate(_cellViewPrefab, _cellContainer.transform);
            cellView.OnCellChanged(cell);
            _cells.Add(cell, cellView);
        }
    }
}