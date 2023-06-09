using Inventory_Items;
using UnityEngine;

namespace Inventory
{
    public enum ItemType
    {
        Null,
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
        
        [field: SerializeField]
        public int ItemId { get; private set; }
    }
}