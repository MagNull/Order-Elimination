using System;
using OrderElimination.MacroGame;
using UnityEngine;

namespace GameInventory.Items
{
    public class HealItem : ConsumableItem, IUsable
    {
        [SerializeField]
        private int _healAmount;
        
        public HealItem(HealItemData itemData) : base(itemData)
        {
            _healAmount = itemData.HealAmount;
        }

        public void Use(GameCharacter gameCharacter)
        {
            if (!CheckConditionToUse(gameCharacter))
                throw new ArgumentException("Character is already full health");
            
            Debug.Log(gameCharacter.CurrentHealth);
            gameCharacter.CurrentHealth += _healAmount;
            Debug.Log(gameCharacter.CurrentHealth);

            UseTimes--;
        }

        public bool CheckConditionToUse(GameCharacter gameCharacter)
        {
            return gameCharacter.CurrentHealth < gameCharacter.CharacterStats.MaxHealth;
        }
    }
}