namespace CharacterAbility
{
    public abstract class Ability
    {
        protected IAbilityCaster _caster;

        protected Ability(IAbilityCaster caster)
        {
            _caster = caster;
        }
        public abstract void Use(IBattleObject target, BattleMap battleMap);
    }
}