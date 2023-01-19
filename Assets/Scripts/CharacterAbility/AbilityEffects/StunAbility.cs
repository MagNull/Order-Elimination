using Cysharp.Threading.Tasks;
using OrderElimination;

namespace CharacterAbility
{
    public class StunAbility : Ability
    {

        public StunAbility(IBattleObject caster, bool isMain, Ability nextEffect, float probability, BattleObjectSide filter) :
            base(caster, isMain, nextEffect, filter, probability)
        {
        }

        
        protected override async UniTask ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            if (target is not IActor targetActor)
            {
                await UseNext(target, stats);
                return;
            }
            targetActor.ClearActions();
            await UseNext(target, stats);
        }
    }
}