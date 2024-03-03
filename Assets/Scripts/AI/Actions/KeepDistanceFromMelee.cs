using System;
using System.Collections.Generic;
using System.Linq;
using AI.Utils;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI.Actions
{
    public class KeepDistanceFromMelee : SequentialTask
    {
        [SerializeField]
        private int _distance;

        protected override async UniTask<bool> Run(Blackboard blackboard)
        {
            var context = blackboard.Get<IBattleContext>("context");
            var caster = blackboard.Get<AbilitySystemActor>("caster");
            
            var enemies = blackboard.Get<IEnumerable<AbilitySystemActor>>("targets");
            var notOptimalCells = enemies
                .SelectMany(e => AIUtilities.GetCellsFromTarget(_distance, e.Position))
                .ToHashSet();

            var movementAbility = AbilityAIPresentation.GetMoveAbility(caster);
            if (movementAbility == null)
                return false;
            movementAbility.InitiateCast(context, caster);
            if (movementAbility.AbilityData.TargetingSystem
                is not IRequireSelectionTargetingSystem manualTargeting)
                throw new NotSupportedException();
            var optimalCells = manualTargeting.PeekAvailableCells(context, caster)
                .Where(c => notOptimalCells.Contains(c));
            if (!optimalCells.Any())
            {
                movementAbility.AbilityData.TargetingSystem.CancelTargeting();
                return false;
            }
            
            var result = await movementAbility.CastSingleTarget(context, caster,
                optimalCells.ElementAt(Random.Range(0, optimalCells.Count())));
            
            return result;
        }
    }
}