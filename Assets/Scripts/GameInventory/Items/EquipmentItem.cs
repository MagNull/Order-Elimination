using System;
using OrderElimination.AbilitySystem;
using UnityEngine;

namespace GameInventory.Items
{
    [Serializable]
    public class EquipmentItem : Item
    {
        public EquipmentItem(ItemData itemData) : base(itemData)
        {
            if (itemData.EquipAbility == null)
                throw new ArgumentException(
                    $"{nameof(itemData)} has no {nameof(itemData.EquipAbility)} assigned.");
        }

        public override void OnTook(AbilitySystemActor abilitySystemActor)
        {
            var ability = Data.EquipAbility;
            abilitySystemActor.GrantPassiveAbility(
                new PassiveAbilityRunner(AbilityFactory.CreatePassiveAbility(ability),
                    AbilityProvider.Equipment));
        }
    }
}