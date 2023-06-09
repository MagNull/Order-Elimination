using System.Linq;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using UnityEngine;

namespace AI.Conditions
{
    public class HasEnemyInRadius : IBehaviorTreeTask
    {
        [SerializeField]
        private int _radius;

        public UniTask<bool> Run(Blackboard blackboard)
        {
            var caster = blackboard.Get<AbilitySystemActor>("caster");
            var enemies = blackboard.Get<AbilitySystemActor[]>("enemies");
            foreach (var enemy in enemies)
            {
                if (Vector2Int.Distance(enemy.Position, caster.Position) <= _radius)
                    return UniTask.FromResult(true);
            }

            return UniTask.FromResult(false);
        }
    }
}