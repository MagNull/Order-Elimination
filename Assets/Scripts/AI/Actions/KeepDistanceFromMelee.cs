using System;
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

        public async UniTask<bool> Run(IBattleContext battleContext, AbilitySystemActor caster)
        {
            Vector2Int[] notOptimalCells = Array.Empty<Vector2Int>();
            var enemies = battleContext.EntitiesBank.GetEntities(BattleSide.Allies)
                .Union(battleContext.EntitiesBank.GetEntities(BattleSide.Player));

            foreach (var enemy in enemies)
            {
                var notOptimalFromEnemy = AIUtilities.GetCellsFromTarget(_distance, enemy.Position);
                notOptimalCells.AddRange(notOptimalFromEnemy);
            }

            var movementAbility = AbilityAIPresentation.GetMoveAbility(caster);
            movementAbility.InitiateCast(battleContext, caster);

            var optimalCells = movementAbility.AbilityData.Rules
                .GetAvailableCellPositions(battleContext, caster)
                .Except(notOptimalCells);
            if (!optimalCells.Any())
                return false;
            
            await movementAbility.CastSingleTarget(battleContext, caster,
                optimalCells.ElementAt(Random.Range(0, optimalCells.Count())));
            
            return true;
        }
    }
}