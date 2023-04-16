using OrderElimination;
using RoguelikeMap;

namespace StartSessionMenu.ChooseCharacter.CharacterCard
{
    public class CharacterCardWithHealthBar : CharacterCard
    {
        private HealthBar _healthBar;

        public override void InitializeCard(Character character)
        {
            _healthBar = GetComponentInChildren<HealthBar>();
            _healthBar?.SetMaxHealth(character.GetBattleStats().UnmodifiedHealth);
            base.InitializeCard(character);
        }
    }
}