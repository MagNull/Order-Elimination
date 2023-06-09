using System;
using System.Linq;
using AI.Utils;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI.Conditions
{
    public class MoveToEnemy : IBehaviorTreeTask
    {
        public async UniTask<bool> Run(Blackboard blackboard)
        {
            var targets = blackboard.Get<AbilitySystemActor[]>("enemies");
            if (targets.Length == 0)
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
        
        private async UniTask<bool> TryExecuteTo(IBattleContext battleContext, AbilitySystemActor caster,
            AbilitySystemActor target)
        {
            var movementAbility = AbilityAIPresentation.GetMoveAbility(caster);
            var damageAbilities = AbilityAIPresentation.GetDamageAbilities(battleContext, caster, target);

            foreach (var damageAbility in damageAbilities)
            {
                var cellsFromTarget = GetCellsForCastingAbility(damageAbility.AbilityData, target);
                var intersect = movementAbility.AbilityData.Rules.GetAvailableCellPositions(battleContext, caster)
                    .Intersect(cellsFromTarget);
                if (!intersect.Any())
                    continue;
                var random = Random.Range(0, intersect.Count());
                await movementAbility.CastSingleTarget(battleContext, caster, intersect.ElementAt(random));
                return true;
            }

            movementAbility.AbilityData.TargetingSystem.CancelTargeting();

            return false;
        }

        private Vector2Int[] GetCellsForCastingAbility(IActiveAbilityData abilityData, AbilitySystemActor target)
        {
            var cellConditions = abilityData.Rules.CellConditions;
            var patternCondition = (InPatternCondition)cellConditions.FirstOrDefault(c => c is InPatternCondition);
            if (patternCondition == null ||
                patternCondition.Pattern is not DistanceFromPointPattern distanceFromPointPattern)
                return Array.Empty<Vector2Int>();

            var abilityDistance = distanceFromPointPattern.MaxDistanceFromOrigin;

            return AIUtilities.GetCellsFromTarget(Mathf.FloorToInt(abilityDistance), target.Position);
        }
    }
}