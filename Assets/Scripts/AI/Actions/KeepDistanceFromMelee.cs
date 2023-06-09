﻿using System;
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
    public class KeepDistanceFromMelee : IBehaviorTreeTask
    {
        [SerializeField]
        private int _distance;

        public async UniTask<bool> Run(Blackboard blackboard)
        {
            var context = blackboard.Get<IBattleContext>("context");
            var caster = blackboard.Get<AbilitySystemActor>("caster");
            
            Vector2Int[] notOptimalCells = Array.Empty<Vector2Int>();
            var enemies = blackboard.Get<AbilitySystemActor[]>("enemies");

            foreach (var enemy in enemies)
            {
                var notOptimalFromEnemy = AIUtilities.GetCellsFromTarget(_distance, enemy.Position);
                notOptimalCells.AddRange(notOptimalFromEnemy);
            }

            var movementAbility = AbilityAIPresentation.GetMoveAbility(caster);
            movementAbility.InitiateCast(context, caster);

            var optimalCells = movementAbility.AbilityData.Rules
                .GetAvailableCellPositions(context, caster)
                .Except(notOptimalCells);
            if (!optimalCells.Any())
            {
                movementAbility.AbilityData.TargetingSystem.CancelTargeting();
                return false;
            }
            
            await movementAbility.CastSingleTarget(context, caster,
                optimalCells.ElementAt(Random.Range(0, optimalCells.Count())));
            
            return true;
        }
    }
}