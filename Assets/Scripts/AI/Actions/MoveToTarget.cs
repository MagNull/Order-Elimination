using System;
using System.Collections.Generic;
using System.Linq;
using AI.Utils;
using Cysharp.Threading.Tasks;
using OrderElimination;
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
    
    public class MoveToTarget : SequentialTask
    {
        [SerializeField]
        private Purpose _purpose; 
        protected override async UniTask<bool> Run(Blackboard blackboard)
        {
            var targets = blackboard.Get<IEnumerable<AbilitySystemActor>>("targets");
            Logging.Log("Targets da");
            if (!targets.Any())
                return false;
            
            var context = blackboard.Get<IBattleContext>("context");
            var caster = blackboard.Get<AbilitySystemActor>("caster");
            
            Logging.Log("Start Move");
            foreach (var target in targets)
            {
                if (await TryExecuteTo(context, caster, target))
                    return true;
            }
            Logging.Log("Fail Move");

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
            foreach (var targetAbility in targetAbilities)
            {
                var targeting = targetAbility.AbilityRunner.AbilityData.GameRepresentation.TargetingSystem;

                //Works only for square-distance abilities
                var distanceFromTargetPattern = new DistanceFromPointPattern(
                    0, targeting.MaxSquareRange, true);//target range, not actual hit range!

                var cellsFromTarget = distanceFromTargetPattern.GetAbsolutePositions(target.Position);

                var intersect = cellsFromTarget
                    .Where(c => manualTargeting.CanSelectPeek(battleContext, caster, c));//Where can go
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