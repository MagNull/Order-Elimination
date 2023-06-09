using AI.Utils;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using UnityEngine;

namespace AI.Actions
{
    public enum TargetSort
    {
        Distance,
        Value
    }

    public class FindEnemies : IBehaviorTreeTask
    {
        [SerializeField]
        private TargetSort _targetSort;

        public async UniTask<bool> Run(Blackboard blackboard)
        {
            var context = blackboard.Get<IBattleContext>("context");
            var caster = blackboard.Get<AbilitySystemActor>("caster");
            AbilitySystemActor[] enemies;
            switch (_targetSort)
            {
                case TargetSort.Distance:
                    enemies = context.EntitiesBank.GetEnemiesByDistance(context, caster);
                    break;
                case TargetSort.Value:
                    enemies = context.EntitiesBank.GetEnemiesByValue(context, caster);
                    break;
                default:
                    enemies = new AbilitySystemActor[] { };
                    break;
            }

            if (enemies.Length == 0)
                return false;

            blackboard.Register("enemies", context.EntitiesBank.GetEnemiesByDistance(context, caster));
            return true;
        }
    }
}