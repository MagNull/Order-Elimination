using System;
using System.Linq;
using GameInventory.Items;
using GameInventory.Views;
using OrderElimination;
using UnityEngine;
using UnityEngine.UI;

namespace GameInventory
{
    public class PickItemInventoryPresenter : InventoryPresenter
    {
        [SerializeField]
        private Inventory _targetInventory;

        [SerializeField]
        private FullInventoryCellView _activeCellView;

        [SerializeField]
        private GameObject _activeCellPanel;

        [SerializeField]
        private Button _dropItemButton;

        [SerializeField]
        private int _activeCellIndex;

        public void UpdateTargetInventory(Inventory inventory) => _targetInventory = inventory;

        public void SetActiveCellIndex(int i)
        {
            _activeCellIndex = i;
            if (_targetInventory.Cells[i].Item is EmptyItem)
            {
                _activeCellView.Disable();
                _activeCellPanel.SetActive(false);
                return;
            }

            InitActiveEquipment(_activeCellIndex);
        }

        private void InitActiveEquipment(int i)
        {
            _activeCellView.Init(_targetInventory.Cells[i]);
            _activeCellPanel.SetActive(true);
            _dropItemButton.onClick.AddListener(() =>
            {
                Debug.Log("Drop");
                _activeCellView.Disable();
                _activeCellPanel.SetActive(false);
                _targetInventory.MoveItemTo(_targetInventory.Cells[_activeCellIndex].Item, _inventoryModel);
                _dropItemButton.onClick.RemoveAllListeners();
            });
        }

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
            InitActiveEquipment(_activeCellIndex);
        }
    }
}