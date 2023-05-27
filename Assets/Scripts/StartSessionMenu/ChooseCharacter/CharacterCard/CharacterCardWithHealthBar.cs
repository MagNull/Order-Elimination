using OrderElimination;
using RoguelikeMap;
using UnityEngine;

namespace StartSessionMenu.ChooseCharacter.CharacterCard
{
    public class CharacterCardWithHealthBar : CharacterCard
    {
        private HealthBar _healthBar;

        public override void InitializeCard(Character character, bool isSelected)
        {
            _healthBar = GetComponentInChildren<HealthBar>();
            _healthBar.SetMaxHealth((int)character.GetBattleStats().UnmodifiedHealth);
            base.InitializeCard(character, isSelected);
        }

        public void SetImage(Sprite sprite)
        {
            _cardImage.sprite = sprite;
        }
    }
}