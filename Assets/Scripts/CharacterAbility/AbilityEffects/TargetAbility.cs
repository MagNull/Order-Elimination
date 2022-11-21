using OrderElimination;

namespace CharacterAbility.AbilityEffects
{
    public class TargetAbility : Ability
    {
        private readonly Ability _nextAbility;
        private readonly bool _selfCast;

        public TargetAbility(IBattleObject caster, Ability nextAbility, bool selfCast,
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