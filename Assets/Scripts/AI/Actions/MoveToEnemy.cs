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
    public abstract class MoveToEnemy : IBehaviorTreeTask
    {
        public async UniTask<bool> Run(IBattleContext battleContext, AbilitySystemActor caster)
        {
            var targets = GetTargets(battleContext, caster);

            foreach (var target in targets)
            {
                if (await TryExecuteTo(battleContext, caster, target))
                    return true;
            }

            return false;
        }

        protected abstract AbilitySystemActor[] GetTargets(IBattleContext battleContext, AbilitySystemActor caster);

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
    
    public class MoveToNearestEnemy : MoveToEnemy
    {
        protected override AbilitySystemActor[] GetTargets(IBattleContext battleContext, AbilitySystemActor caster)
        {
            return battleContext.EntitiesBank.GetEnemiesByDistance(battleContext, caster);
        }
    }
    
    public class MoveToMostValuableEnemy : MoveToEnemy
    {
        protected override AbilitySystemActor[] GetTargets(IBattleContext battleContext, AbilitySystemActor caster) =>
            battleContext.EntitiesBank.GetEnemiesByValue(battleContext, caster);
    }
}