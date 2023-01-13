using Cysharp.Threading.Tasks;
using OrderElimination;

namespace CharacterAbility
{
    public class StunAbility : Ability
    {
        private readonly Ability _nextEffect;

        public StunAbility(IBattleObject caster, Ability nextEffect, float probability, BattleObjectSide filter) :
            base(caster, nextEffect, filter, probability)
        {
            _nextEffect = nextEffect;
        }

        
        protected override async UniTask ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            if (target is not IActor targetActor)
            {
                if(_nextEffect == null)
                    return;
                await _nextEffect.Use(target, stats);
                return;
            }
            targetActor.ClearActions();
            if(_nextEffect == null)
                return;
            await _nextEffect.Use(target, stats);
        }
    }
}