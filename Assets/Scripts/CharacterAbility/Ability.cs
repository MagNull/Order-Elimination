using OrderElimination;

namespace CharacterAbility
{
    public abstract class Ability
    {
        protected readonly IAbilityCaster _caster;
        protected readonly BattleObjectSide _filter;

        protected Ability(IAbilityCaster caster, BattleObjectSide filter)
        {
            _caster = caster;
            _filter = filter;
        }
        public abstract void Use(IBattleObject target, IReadOnlyBattleStats stats, BattleMap battleMap);
    }
}