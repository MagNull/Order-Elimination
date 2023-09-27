using System;
using AI;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace GameInventory.Items
{
    public enum ItemType
    {
        Consumable,
        Equipment,
        Others
    }

    public enum ItemRarity
    {
        Common,
        Rare,
        Epic,
    }

    [CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
    public class ItemData : SerializedScriptableObject
    {
        public EnumMask<Role> RoleFilter = new();
        [field: SerializeField] public ItemView View { get; private set; }
        [field: SerializeField] public ItemType Type { get; private set; }

        [field: SerializeField] public ItemRarity Rarity { get; private set; }

        [field: ShowInInspector, DisplayAsString]
        public string Id { get; private set; }

        [field: SerializeReference, ShowIf("@Type == ItemType.Consumable")]
        public ActiveAbilityBuilder UseAbility { get; private set; }

        [field: SerializeReference, ShowIf("@Type == ItemType.Consumable")]
        public int UseTimes { get; private set; }

        [field: SerializeReference, ShowIf("@Type == ItemType.Equipment")]
        public PassiveAbilityBuilder EquipAbility { get; private set; }

        private void Awake()
        {
            Id = ItemIdentifier.GetID(this);
        }
    }
}