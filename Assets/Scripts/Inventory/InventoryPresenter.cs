using Sirenix.OdinInspector;
using UnityEngine;

namespace Inventory
{
    public class InventoryPresenter : MonoBehaviour
    {
        [SerializeField]
        private InventoryView _inventoryView;
        [SerializeField]
        private ItemData[] _items;
        private Inventory _inventoryModel;

        private void Awake()
        {
            _inventoryModel = new Inventory(100);
        }

        private void OnEnable()
        {
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
            _inventoryModel.AddItem(new Item(_items[Random.Range(0, _items.Length)]));
        }
    }
}