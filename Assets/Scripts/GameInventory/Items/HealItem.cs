using System;
using OrderElimination.MacroGame;
using UnityEngine;

namespace GameInventory.Items
{
    public class HealItem : ConsumableItem, IUsable
    {
        public HealItem(HealItemData itemData) : base(itemData) { }

        public HealItem(HealItemData itemData, int consumesLeft) : base(itemData, consumesLeft) { }

        public void Use(GameCharacter gameCharacter)
        {
            if (!CheckConditionToUse(gameCharacter))
                throw new ArgumentException("Character is already full health");
            
            Debug.Log(gameCharacter.CurrentHealth);
            gameCharacter.CurrentHealth += ((HealItemData)Data).HealAmount;
            Debug.Log(gameCharacter.CurrentHealth);

            ConsumesLeft--;
        }

        public bool CheckConditionToUse(GameCharacter gameCharacter)
        {
            return gameCharacter.CurrentHealth < gameCharacter.CharacterStats.MaxHealth;
        }
    }
}