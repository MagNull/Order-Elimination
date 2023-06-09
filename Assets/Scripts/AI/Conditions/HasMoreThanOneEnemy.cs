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
        
        public UniTask<bool> Run(Blackboard blackboard)
        {
            var caster = blackboard.Get<AbilitySystemActor>("caster");
            var enemies = blackboard.Get<AbilitySystemActor[]>("enemies");
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