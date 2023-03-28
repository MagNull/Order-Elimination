using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class InventoryView : SerializedMonoBehaviour
    {
        [SerializeField]
        private Dictionary<Cell, InventoryCellView> _cells = new Dictionary<Cell, InventoryCellView>();
        [SerializeField]
        private InventoryCellView _cellViewPrefab;
        [SerializeField]
        private GridLayoutGroup _cellContainer;

        public void ShowItemWithType(int itemType)
        {
            foreach (var cell in _cells)
            {
                cell.Value.gameObject.SetActive(cell.Key.Item.Type == (ItemType)itemType);
            }
        }

        public void OnCellChanged(Cell oldCell, Cell newCell)
        {
            var cellView = _cells[oldCell];
            cellView.OnCellChanged(newCell);

            _cells.Remove(oldCell);
            _cells.Add(newCell, cellView);
        }

        public void OnCellAdded(Cell cell)
        {
            var cellView = Instantiate(_cellViewPrefab, _cellContainer.transform);
            cellView.OnCellChanged(cell);
            _cells.Add(cell, cellView);
        }
    }
}