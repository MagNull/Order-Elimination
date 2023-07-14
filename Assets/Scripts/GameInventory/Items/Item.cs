using System;
using AI;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using OrderElimination.MacroGame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GameInventory.Items
{
    [Serializable]
    public class Item
    {
        [SerializeField, HideInInspector]
        private ItemData _itemData;

        public ItemData Data => _itemData;

        public Item(ItemData itemData)
        {
            _itemData = itemData;
        }

        public virtual void OnTook(AbilitySystemActor abilitySystemActor)
        {
            return;
        }
    }

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

    [Serializable]
    public class ConsumableItem : Item
    {
        public event Action<ConsumableItem> UseTimesOver;

        [SerializeField]
        private ActiveAbilityBuilder _useAbility;

        [SerializeField]
        private int _useTimes;

        protected int UseTimes
        {
            get => _useTimes;
            set
            {
                _useTimes = value;
                if (_useTimes > 0)
                    return;
                UseTimesOver?.Invoke(this);
            }
        }

        public ConsumableItem(ItemData itemData) : base(itemData)
        {
            _useAbility = itemData.UseAbility;
            _useTimes = itemData.UseTimes;
        }

        public override void OnTook(AbilitySystemActor abilitySystemActor)
        {
            var ability = new ActiveAbilityRunner(AbilityFactory.CreateActiveAbility(_useAbility),
                AbilityProvider.Equipment);
            abilitySystemActor.GrantActiveAbility(ability);
            UseTimesOver += _ => abilitySystemActor.RemoveActiveAbility(ability);
            ability.AbilityExecutionStarted += _ => UseTimes--;
        }
    }

    public interface IUsable
    {
        public void Use(GameCharacter gameCharacter);

        public bool CheckConditionToUse(GameCharacter gameCharacter);
    }
}