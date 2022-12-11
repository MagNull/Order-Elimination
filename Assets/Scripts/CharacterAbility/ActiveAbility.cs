using OrderElimination;

namespace CharacterAbility.AbilityEffects
{
    public class ActiveAbility : Ability
    {
        private readonly Ability _effects;
        private readonly bool _selfCast;

        public ActiveAbility(IBattleObject caster, Ability effects, bool selfCast,
            BattleObjectSide filter)
            : base(caster, effects, filter, 100)
        {
            _effects = effects;
            _selfCast = selfCast;
        }

        protected override void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            _effects?.Use(_selfCast ? _caster : target, stats);
        }
    }
}