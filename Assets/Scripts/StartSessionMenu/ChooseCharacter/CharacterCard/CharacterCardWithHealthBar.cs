using GameInventory;
using OrderElimination.MacroGame;
using RoguelikeMap;
using UnityEngine;

namespace StartSessionMenu.ChooseCharacter.CharacterCard
{
    public class CharacterCardWithHealthBar : CharacterCard
    {
        [SerializeField]
        private InventoryPresenter _inventoryPresenter;
        
        private HealthBar _healthBar;

        public override void InitializeCard(GameCharacter character, bool isSelected)
        {
            _healthBar = GetComponentInChildren<HealthBar>();
            _healthBar.SetGameCharacter(character);
            _inventoryPresenter.InitInventoryModel(character.Inventory);
            base.InitializeCard(character, isSelected);
        }
    }
}