using UnityEngine;

namespace GameInventory.Items
{
    [CreateAssetMenu(fileName = "Item", menuName = "Inventory/Heal Item")]
    public class HealItemData : ItemData
    {
        [field: SerializeReference]
        public int HealAmount { get; private set; }
    }
}