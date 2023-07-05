﻿using System;
using Inventory;
using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;

namespace Inventory_Items
{
    [Serializable]
    public abstract class Item
    {
        [ShowInInspector]
        private readonly ItemView _itemView;

        private readonly ItemType _itemType;
        private readonly int _itemId;

        public ItemView View => _itemView;
        public ItemType Type => _itemType;
        public int Id => _itemId;

        protected Item(ItemData itemData)
        {
            _itemView = itemData.View;
            _itemType = itemData.Type;
            _itemId = itemData.Id;
        }

        public abstract void OnTook(AbilitySystemActor abilitySystemActor);
    }

    public class EquipmentItem : Item
    {
        private IPassiveAbilityData _equipAbility;
        
        public EquipmentItem(ItemData itemData) : base(itemData)
        {
            _equipAbility = AbilityFactory.CreatePassiveAbility(itemData.EquipAbility);
        }

        public override void OnTook(AbilitySystemActor abilitySystemActor)
        {
            abilitySystemActor.GrantPassiveAbility(new PassiveAbilityRunner(_equipAbility, AbilityProvider.Equipment));
        }
    }

    public class ConsumableItem : Item
    {
        private IActiveAbilityData _useAbility;
        private int _useTimes;

        public ConsumableItem(ItemData itemData) : base(itemData)
        {
            _useAbility = AbilityFactory.CreateActiveAbility(itemData.UseAbility);
            _useTimes = itemData.UseTimes;
        }

        public override void OnTook(AbilitySystemActor abilitySystemActor)
        {
            var ability = new ActiveAbilityRunner(_useAbility, AbilityProvider.Equipment);
            abilitySystemActor.GrantActiveAbility(ability);
            ability.AbilityExecutionStarted += _ =>
            {
                _useTimes--;
                if (_useTimes <= 0)
                    abilitySystemActor.RemoveActiveAbility(ability);
            };
        }
    }
}