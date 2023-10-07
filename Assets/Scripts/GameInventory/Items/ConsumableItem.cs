using System;
using OrderElimination.AbilitySystem;
using UnityEngine;

namespace GameInventory.Items
{
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
}