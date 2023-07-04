using Inventory_Items;
using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Inventory
{
    public enum ItemType
    {
        Null,
        Consumable,
        Equipment,
    }

    [CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
    public class ItemData : ScriptableObject
    {
        [field: SerializeField] public ItemView View { get; private set; }

        [field: SerializeField] public ItemType Type { get; private set; }

        [field: SerializeField] public int Id { get; private set; }

        [field: SerializeReference, ShowIf("@Type == ItemType.Consumable")]
        public IActiveAbilityData UseAbility { get; private set; }

        [field: SerializeReference, ShowIf("@Type == ItemType.Consumable")]
        public int UseTimes { get; private set; }

        [field: SerializeReference, ShowIf("@Type == ItemType.Equipment")]
        public IPassiveAbilityData EquipAbility { get; private set; }
    }
}