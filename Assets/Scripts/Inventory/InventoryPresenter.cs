using Sirenix.OdinInspector;
using UnityEngine;

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

        private void Awake()
        {
            _inventoryModel = new Inventory(100);
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