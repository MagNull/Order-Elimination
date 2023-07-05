﻿using Inventory_Items;
using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Inventory
{
    public enum ItemType
    {
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
        public ActiveAbilityBuilder UseAbility { get; private set; }

        [field: SerializeReference, ShowIf("@Type == ItemType.Consumable")]
        public int UseTimes { get; private set; }

        [field: SerializeReference, ShowIf("@Type == ItemType.Equipment")]
        public PassiveAbilityBuilder EquipAbility { get; private set; }
    }
}