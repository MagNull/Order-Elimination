using System;
using System.Collections.Generic;
using System.Linq;
using AI.Utils;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI.Actions
{
    public enum Purpose
    {
        Damage,
        Heal
    }
    
    public class MoveToTarget : BehaviorTreeTask
    {
        [SerializeField]
        private Purpose _purpose; 
        protected override async UniTask<bool> Run(Blackboard blackboard)
        {
            var targets = blackboard.Get<IEnumerable<AbilitySystemActor>>("targets");
            if (!targets.Any())
                return false;
            
            var context = blackboard.Get<IBattleContext>("context");
            var caster = blackboard.Get<AbilitySystemActor>("caster");

            foreach (var target in targets)
            {
                if (await TryExecuteTo(context, caster, target))
                    return true;
            }

            return false;
        }
        
        private async UniTask<bool> TryExecuteTo(
            IBattleContext battleContext, 
            AbilitySystemActor caster,
            AbilitySystemActor target)
        {
            var movementAbility = AbilityAIPresentation.GetMoveAbility(caster);
            if (movementAbility == null)
                return false;
            if (movementAbility.AbilityData.TargetingSystem
                is not IRequireSelectionTargetingSystem manualTargeting)
                throw new NotSupportedException();
            var targetAbilities = _purpose switch
            {
                Purpose.Damage => AbilityAIPresentation.GetDamageAbilities(battleContext, caster, target),
                Purpose.Heal => AbilityAIPresentation.GetAvailableHealAbilities(battleContext, caster, target),
                _ => throw new ArgumentOutOfRangeException()
            };
            foreach (var damageAbility in targetAbilities)
            {
                var cellsFromTarget = GetCellsForCastingAbility(
                    battleContext, damageAbility.AbilityRunner.AbilityData, caster);
                var intersect = cellsFromTarget
                    .Where(c => manualTargeting.CanSelectPeek(battleContext, caster, c));
                if (!intersect.Any())
                    continue;
                
                var random = Random.Range(0, intersect.Count());
                var result = await movementAbility.CastSingleTarget(battleContext, caster, intersect.ElementAt(random));
                manualTargeting.CancelTargeting();
                return result;
            }

            manualTargeting.CancelTargeting();

            return false;
        }

        private Vector2Int[] GetCellsForCastingAbility(
            IBattleContext battleContext, IActiveAbilityData abilityData, AbilitySystemActor caster)
        {
            if (abilityData.TargetingSystem is not IRequireSelectionTargetingSystem manualTargeting)
                throw new NotSupportedException();
            return manualTargeting.PeekAvailableCells(battleContext, caster);
        }
    }
}