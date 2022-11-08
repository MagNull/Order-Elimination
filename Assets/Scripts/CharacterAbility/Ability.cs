using OrderElimination;

namespace CharacterAbility
{
    public abstract class Ability
    {
        protected IAbilityCaster _caster;

        protected Ability(IAbilityCaster caster)
        {
            _caster = caster;
        }
        public abstract void Use(IBattleObject target, IReadOnlyBattleStats stats, BattleMap battleMap);
    }
}