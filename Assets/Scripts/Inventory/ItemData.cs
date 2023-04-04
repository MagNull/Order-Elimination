using UnityEngine;

namespace Inventory_Items
{
    public enum ItemType
    {
        Consumable,
        Equipment,
        Modificator
    }
    
    [CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
    public class ItemData : ScriptableObject
    {
        [field: SerializeField]
        public ItemView ItemView { get; private set; }
        
        [field: SerializeField]
        public ItemType ItemType { get; private set; }
    }
}