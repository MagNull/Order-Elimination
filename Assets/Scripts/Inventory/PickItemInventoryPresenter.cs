using System;

namespace Inventory_Items
{
    public class PickItemInventoryPresenter : InventoryPresenter
    {
        private Inventory _targetInventory;
        
        public void UpdateTargetInventory(Inventory inventory) => _targetInventory = inventory;

        protected override void OnEnableAdditional()
        {
            _inventoryView.CellClicked += OnCellClicked;
        }

        protected override void OnDisableAdditional()
        {
            _inventoryView.CellClicked -= OnCellClicked;
        }

        private void OnCellClicked(IReadOnlyCell cell)
        {
            if (_targetInventory == null)
                throw new Exception("Target inventory is null");
            
            _inventoryModel.MoveItemTo(cell.Item, _targetInventory);
        }
    }
}