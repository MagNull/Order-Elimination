using System;
using OrderElimination;
using OrderElimination.AbilitySystem;

namespace GameInventory.Items
{
    [Serializable]
    public class ConsumableItem : Item
    {
        private int _consumesLeft;

        public ConsumableItem(ItemData itemData) : this(itemData, itemData.UseTimes) { }

        public ConsumableItem(ItemData itemData, int consumesLeft) : base(itemData)
        {
            Logging.Log($"{itemData.UseAbility?.Name}");
            if (itemData.UseAbility == null)
                throw new ArgumentException(
                    $"Item {itemData.View.Name} has no {nameof(itemData.UseAbility)} assigned.");
            if (consumesLeft < 0)
                throw new ArgumentOutOfRangeException();
            _consumesLeft = consumesLeft;
        }

        public int ConsumesLeft
        {
            get => _consumesLeft;
            protected set
            {
                _consumesLeft = value;
                if (_consumesLeft <= 0)
                    UseTimesOver?.Invoke(this);
            }
        }

        public event Action<ConsumableItem> UseTimesOver;

        public override void OnTook(AbilitySystemActor abilitySystemActor)
        {
            var abilityBuilder = Data.UseAbility;
            var ability = new ActiveAbilityRunner(AbilityFactory.CreateActiveAbility(abilityBuilder),
                AbilityProvider.Equipment);
            abilitySystemActor.GrantActiveAbility(ability);
            UseTimesOver += _ => abilitySystemActor.RemoveActiveAbility(ability);
            ability.AbilityExecutionStarted += _ => ConsumesLeft--;
        }
    }
}