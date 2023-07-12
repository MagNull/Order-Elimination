using System;
using OrderElimination.MacroGame;
using UnityEngine;

namespace GameInventory.Items
{
    public class HealItem : ConsumableItem, IUsable
    {
        private readonly int _healAmount;
        
        public HealItem(HealItemData itemData) : base(itemData)
        {
            _healAmount = itemData.HealAmount;
        }

        public void Use(GameCharacter gameCharacter)
        {
            if (!CheckConditionToUse(gameCharacter))
                throw new ArgumentException("Character is already full health");
            
            gameCharacter.CurrentHealth += _healAmount;
            UseTimes--;
        }

        public bool CheckConditionToUse(GameCharacter gameCharacter)
        {
            return gameCharacter.CurrentHealth < gameCharacter.CharacterStats.MaxHealth;
        }
    }
}