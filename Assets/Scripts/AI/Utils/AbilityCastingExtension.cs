using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using UnityEngine;

namespace AI.Utils
{
    public static class AbilityCastingExtension
    {
        public static async UniTask<bool> CastSingleTarget(this ActiveAbilityRunner abilityRunner,
            IBattleContext battleContext,
            AbilitySystemActor caster, Vector2Int targetPos)
        {
            if (!abilityRunner.AbilityData.TargetingSystem.IsTargeting)
            {
                if (!abilityRunner.InitiateCast(battleContext, caster))
                    return false;
            }
            
            var targeting = (SingleTargetTargetingSystem)abilityRunner.AbilityData.TargetingSystem;
            targeting.ConfirmationUnlocked += _ => { targeting.ConfirmTargeting(); };

            var casterView = battleContext.EntitiesBank.GetViewByEntity(caster);
            Debug.Log($"AI {casterView.Name} casted {abilityRunner.AbilityData.View.Name}");
            var completed = false;
            abilityRunner.AbilityExecutionCompleted += _ => completed = true;
            
            targeting.Select(targetPos);
            await UniTask.WaitUntil(() => completed);

            return true;
        }
    }
}