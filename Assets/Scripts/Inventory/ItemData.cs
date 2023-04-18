using UnityEngine;

namespace Inventory_Items
{
    public enum ItemType
    {
        Consumable,
        Equipment,
        Modificator,
        Null
    }
    
    [CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
    public class ItemData : ScriptableObject
    {
        [field: SerializeField]
        public virtual ItemView ItemView { get; private set; }
        
        [field: SerializeField]
        public virtual ItemType ItemType { get; private set; }
    }
}