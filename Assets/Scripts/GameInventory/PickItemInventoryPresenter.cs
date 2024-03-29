﻿using System;
using System.Linq;
using OrderElimination;
using UnityEngine;

namespace GameInventory
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
            gameObject.SetActive(false);
        }

        private void OnCellClicked(IReadOnlyCell cell)
        {
            if (_targetInventory == null)
                Logging.LogException( new Exception("Target inventory is null"));

            var characterItems = _targetInventory.Cells.Select(c => c.Item);
            var itemOfType = characterItems.FirstOrDefault(item => cell.Item.Data.Type == item.Data.Type);
            if (itemOfType != null) 
                _targetInventory.MoveItemTo(itemOfType, _inventoryModel);
            
            _inventoryModel.MoveItemTo(cell.Item, _targetInventory);
        }
    }
}