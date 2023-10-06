using System;
using OrderElimination.AbilitySystem;
using UnityEngine;

namespace GameInventory.Items
{
    [Serializable]
    public class EquipmentItem : Item
    {
        [SerializeField]
        private PassiveAbilityBuilder _equipAbility;

        public EquipmentItem(ItemData itemData) : base(itemData)
        {
            _equipAbility = itemData.EquipAbility;
        }

        public override void OnTook(AbilitySystemActor abilitySystemActor)
        {
            abilitySystemActor.GrantPassiveAbility(
                new PassiveAbilityRunner(AbilityFactory.CreatePassiveAbility(_equipAbility),
                    AbilityProvider.Equipment));
        }
    }
}