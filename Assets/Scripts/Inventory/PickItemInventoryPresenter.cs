using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Inventory_Items
{
    public class PickItemInventoryPresenter : InventoryPresenter
    {
        [SerializeField]
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

            var characterItems = _targetInventory.Cells.Select(c => c.Item);
            var itemOfType = characterItems.FirstOrDefault(item => cell.Item.Type == item.Type);
            if (itemOfType != null)
            {
                _targetInventory.MoveItemTo(itemOfType, _inventoryModel);
            }
            _inventoryModel.MoveItemTo(cell.Item, _targetInventory);
        }
    }
}