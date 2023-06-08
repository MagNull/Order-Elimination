using System.Linq;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using UnityEngine;

namespace AI.Conditions
{
    public class HasMoreThanOneEnemy : IBehaviorTreeTask
    {
        [SerializeField]
        private int _radius;
        
        public UniTask<bool> Run(IBattleContext battleContext, AbilitySystemActor caster)
        {
            var enemies = battleContext.EntitiesBank.GetEntities(BattleSide.Allies)
                .Union(battleContext.EntitiesBank.GetEntities(BattleSide.Player));
            var count = 0;
            foreach (var enemy in enemies)
            {
                if (Vector2Int.Distance(enemy.Position, caster.Position) <= _radius)
                    count++;
            }

            return UniTask.FromResult(count > 1);
        }
    }
}