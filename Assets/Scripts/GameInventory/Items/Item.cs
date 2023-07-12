using System;
using OrderElimination.AbilitySystem;
using OrderElimination.MacroGame;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameInventory.Items
{
    [Serializable]
    public abstract class Item
    {
        [ShowInInspector, SerializeField]
        private ItemView _itemView;

        [SerializeField, HideInInspector]
        private ItemType _itemType;

        [SerializeField, HideInInspector]
        private int _itemId;

        [SerializeField, HideInInspector]
        private ItemRarity _itemRarity;

        public ItemView View => _itemView;
        public ItemType Type => _itemType;
        public ItemRarity Rarity => _itemRarity;
        public int Id => _itemId;

        protected Item(ItemData itemData)
        {
            _itemView = itemData.View;
            _itemType = itemData.Type;
            _itemId = itemData.Id;
            _itemRarity = itemData.Rarity;
        }

        public abstract void OnTook(AbilitySystemActor abilitySystemActor);
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