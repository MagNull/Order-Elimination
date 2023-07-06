using System;
using System.Collections.Generic;
using System.Linq;
using AI.Utils;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI.Actions
{
    public class KeepDistanceFromMelee : BehaviorTreeTask
    {
        [SerializeField]
        private int _distance;

        protected override async UniTask<bool> Run(Blackboard blackboard)
        {
            var context = blackboard.Get<IBattleContext>("context");
            var caster = blackboard.Get<AbilitySystemActor>("caster");
            
            var notOptimalCells = new List<Vector2Int>();
            var enemies = blackboard.Get<IEnumerable<AbilitySystemActor>>("targets");

            foreach (var enemy in enemies)
            {
                var notOptimalFromEnemy = AIUtilities.GetCellsFromTarget(_distance, enemy.Position);
                notOptimalCells.AddRange(notOptimalFromEnemy);
            }

            var movementAbility = AbilityAIPresentation.GetMoveAbility(caster);
            movementAbility.InitiateCast(context, caster);
            if (movementAbility.AbilityData.TargetingSystem
                is not IRequireSelectionTargetingSystem manualTargeting)
                throw new NotSupportedException();
            var optimalCells = manualTargeting.PeekAvailableCells(context, caster)
                .Except(notOptimalCells);
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