using GameInventory.Items;
using GameInventory.Views;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace GameInventory
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
            if (_inventoryView is CharacterInventoryView)
                return;
            _inventoryModel = inventory;
        }

        //TODO(CANYA): CE PIZDEC
        public void InitInventoryModel(Inventory inventory)
        {
            _inventoryModel = inventory;
        }

        private void OnEnable()
        {
            _inventoryView.UpdateCells(_inventoryModel.Cells);
            _inventoryModel.OnCellChanged += _inventoryView.OnCellChanged;
            OnEnableAdditional();
        }

        protected virtual void OnEnableAdditional()
        {
            
        }

        private void OnDisable()
        {
            _inventoryModel.OnCellChanged -= _inventoryView.OnCellChanged;
            OnDisableAdditional();
        }

        protected virtual void OnDisableAdditional()
        {
            
        }
        
        [Button]
        public void AddItem()
        {
            _lastItem = ItemFactory.Create(_items[Random.Range(0, _items.Length)]);
            _inventoryModel.AddItem(_lastItem);
        }

        [Button]
        private void AddItemByData(ItemData itemData)
        {
            _lastItem = ItemFactory.Create(itemData);
            _inventoryModel.AddItem(_lastItem);
        }

        [Button]
        public void Clear()
        {
            foreach (var item in _inventoryModel.GetItems())
            {
                _inventoryModel.RemoveItem(item);
            }
            InventorySerializer.Delete();
        }
    }
}