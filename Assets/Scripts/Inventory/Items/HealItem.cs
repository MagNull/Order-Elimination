using Inventory;
using OrderElimination.MacroGame;
using UnityEngine;

namespace Inventory_Items
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
            //TODO: Implement healing
            Debug.Log("Heal " + gameCharacter.CharacterData.Name + " " + _healAmount + " health");
            UseTimes--;
        }
    }
}