using OrderElimination;
using RoguelikeMap;

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
    }
}