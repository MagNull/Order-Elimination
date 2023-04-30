﻿using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace Inventory
{
    public class InventoryPresenter : MonoBehaviour
    {
        [SerializeField]
        private PlayerInventoryView _inventoryView;
        [SerializeField]
        private ItemData[] _items;
        private Inventory _inventoryModel;

        private Item _lastItem;

        [Inject]
        private void Configure(Inventory inventory)
        {
            _inventoryModel = inventory;
        }

        private void OnEnable()
        {
            _inventoryView.UpdateCells(_inventoryModel.Cells);
            _inventoryModel.OnCellAdded += _inventoryView.OnCellAdded;
            _inventoryModel.OnCellChanged += _inventoryView.OnCellChanged;
        }

        private void OnDisable()
        {
            _inventoryModel.OnCellAdded -= _inventoryView.OnCellAdded;
            _inventoryModel.OnCellChanged -= _inventoryView.OnCellChanged;
        }
        
        [Button]
        public void AddItem()
        {
            _lastItem = new Item(_items[Random.Range(0, _items.Length)]);
            _inventoryModel.AddItem(_lastItem);
        }
        
        [Button]
        public void RemoveItem()
        {
            _inventoryModel.RemoveItem(_lastItem);
        }
    }
}