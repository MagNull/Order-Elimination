using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using UnityEngine;

namespace AI.Utils
{
    public static class AbilityCastingExtension
    {
        public static async UniTask CastSingleTarget(this ActiveAbilityRunner abilityRunner,
            IBattleContext battleContext,
            AbilitySystemActor caster, Vector2Int targetPos)
        {
            if(!abilityRunner.AbilityData.TargetingSystem.IsTargeting)
                abilityRunner.InitiateCast(battleContext, caster);
            
            var targeting = (SingleTargetTargetingSystem)abilityRunner.AbilityData.TargetingSystem;
            targeting.ConfirmationUnlocked += _ => { targeting.ConfirmTargeting(); };

            var completed = false;
            abilityRunner.AbilityExecutionCompleted += _ => completed = true;
            
            targeting.Select(targetPos);
            await UniTask.WaitUntil(() => completed);
        }
    }
}