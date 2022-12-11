using OrderElimination;

namespace CharacterAbility.AbilityEffects
{
    public class ActiveAbility : Ability
    {
        private readonly Ability _nextAbility;
        private readonly bool _selfCast;

        public ActiveAbility(IBattleObject caster, Ability nextAbility, bool selfCast,
            BattleObjectSide filter)
            : base(caster, nextAbility, filter, 100)
        {
            _nextAbility = nextAbility;
            _selfCast = selfCast;
        }

        protected override void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            _nextAbility.Use(_selfCast ? _caster : target, stats);
        }
    }
}