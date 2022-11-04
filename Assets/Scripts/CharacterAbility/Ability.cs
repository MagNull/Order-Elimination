namespace CharacterAbility
{
    public abstract class Ability
    {
        protected BattleCharacter _caster;

        protected Ability(BattleCharacter caster)
        {
            _caster = caster;
        }
        public abstract void Use(IBattleObject target, BattleMap battleMap);
    }
}