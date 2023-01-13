using Cysharp.Threading.Tasks;
using OrderElimination;

namespace CharacterAbility.AbilityEffects
{
    public class ActiveAbility : Ability
    {
        private readonly Ability _nextEffect;
        private readonly bool _selfCast;

        public ActiveAbility(IBattleObject caster, Ability nextEffect, bool selfCast,
            BattleObjectSide filter)
            : base(caster, nextEffect, filter)
        {
            _nextEffect = nextEffect;
            _selfCast = selfCast;
        }

        protected override async UniTask ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            if(_nextEffect == null)
                return;
            await _nextEffect.Use(_selfCast ? _caster : target, stats);
        }
    }
}