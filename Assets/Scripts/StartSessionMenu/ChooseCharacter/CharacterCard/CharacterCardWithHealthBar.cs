using OrderElimination;
using RoguelikeMap;
using UnityEngine;

namespace StartSessionMenu.ChooseCharacter.CharacterCard
{
    public class CharacterCardWithHealthBar : CharacterCard
    {
        private HealthBar _healthBar;

        public override void InitializeCard(Character character, Transform defaultParent)
        {
            _healthBar = GetComponentInChildren<HealthBar>();
            _healthBar?.SetMaxHealth(character.GetBattleStats().UnmodifiedHealth);
            base.InitializeCard(character, defaultParent);
        }
    }
}