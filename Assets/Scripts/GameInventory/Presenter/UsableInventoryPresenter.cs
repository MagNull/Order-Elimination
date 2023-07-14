using GameInventory.Items;

namespace GameInventory
{
    public class UsableInventoryPresenter : InventoryPresenter
    {
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
            if(cell.Item is not ConsumableItem consumableItem)
                return;
            
        }
    }
}