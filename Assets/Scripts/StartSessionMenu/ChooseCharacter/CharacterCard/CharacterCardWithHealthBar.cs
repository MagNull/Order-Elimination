using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.MacroGame;
using RoguelikeMap;
using UnityEngine;

namespace StartSessionMenu.ChooseCharacter.CharacterCard
{
    public class CharacterCardWithHealthBar : CharacterCard
    {
        private HealthBar _healthBar;

        public override void InitializeCard(GameCharacter character, bool isSelected)
        {
            _healthBar = GetComponentInChildren<HealthBar>();
            _healthBar.SetMaxHealth(Mathf.RoundToInt(character.CharacterStats.MaxHealth));
            base.InitializeCard(character, isSelected);
        }
    }
}