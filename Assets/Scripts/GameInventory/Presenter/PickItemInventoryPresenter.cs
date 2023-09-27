using System;
using System.Linq;
using GameInventory.Items;
using OrderElimination;
using UnityEngine;

namespace GameInventory
{
    public class PickItemInventoryPresenter : InventoryPresenter
    {
        [SerializeField]
        private Inventory _targetInventory;

        [SerializeField]
        private int _activeCellIndex;

        public void UpdateTargetInventory(Inventory inventory) => _targetInventory = inventory;
        public void SetActiveCellIndex(int i) => _activeCellIndex = i;

        protected override void OnEnableAdditional()
        {
            _inventoryView.CellClicked += OnCellClicked;
        }

        protected override void OnDisableAdditional()
        {
            _inventoryView.CellClicked -= OnCellClicked;
            gameObject.SetActive(false);
        }

        private void OnCellClicked(IReadOnlyCell cell)
        {
            if (_targetInventory == null)
                Logging.LogException(new Exception("Target inventory is null"));
            
            var itemInActiveCell = _targetInventory.Cells[_activeCellIndex].Item;
            if (itemInActiveCell is not EmptyItem)
                _targetInventory.MoveItemTo(itemInActiveCell, _inventoryModel);

            _inventoryModel.MoveItemTo(cell.Item, _targetInventory, _activeCellIndex);
        }
    }
}