using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace Inventory_Items
{
    public class InventoryPresenter : SerializedMonoBehaviour
    {
        [SerializeField]
        protected InventoryView _inventoryView;
        [SerializeField]
        private ItemData[] _items;
        [SerializeField]
        protected Inventory _inventoryModel;

        private Item _lastItem;
        
        [Inject]
        public void Construct(Inventory inventory)
        {
            _inventoryModel = inventory;
        }

        private void OnEnable()
        {
            _inventoryView.UpdateCells(_inventoryModel.Cells);
            _inventoryModel.OnCellAdded += _inventoryView.OnCellAdded;
            _inventoryModel.OnCellChanged += _inventoryView.OnCellChanged;
            OnEnableAdditional();
        }

        protected virtual void OnEnableAdditional()
        {
            
        }

        private void OnDisable()
        {
            _inventoryModel.OnCellAdded -= _inventoryView.OnCellAdded;
            _inventoryModel.OnCellChanged -= _inventoryView.OnCellChanged;
            OnDisableAdditional();
        }

        protected virtual void OnDisableAdditional()
        {
            
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