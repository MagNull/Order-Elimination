using System.Collections.Generic;
using System.Linq;
using AI.Actions;
using AI.Utils;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using UnityEngine;
using UnityEngine.Serialization;

namespace AI.Conditions
{
    public class HasTargetsInRadius : BehaviorTreeTask
    {
        [SerializeField]
        private TargetSort _sortBy;

        [SerializeReference]
        private ITargetCondition[] _targetConditions;

        [SerializeField]
        private BattleRelationship _relationship;

        [SerializeField]
        private int _radius;

        public override async UniTask<bool> Run(Blackboard blackboard)
        {
            var context = blackboard.Get<IBattleContext>("context");
            var caster = blackboard.Get<AbilitySystemActor>("caster");
            IEnumerable<AbilitySystemActor> enemies;
            switch (_sortBy)
            {
                case TargetSort.Distance:
                    enemies = context.EntitiesBank.GetTargetsByDistance(context, caster, _relationship);
                    break;
                case TargetSort.Value:
                    enemies = context.EntitiesBank.GetTargetsByValue(context, caster, _relationship);
                    break;
                default:
                    enemies = new AbilitySystemActor[] { };
                    break;
            }

            enemies = enemies.Where(e =>
                context.BattleMap.GetGameDistanceBetween(e.Position, caster.Position) <= _radius)
                .Where(enemy => _targetConditions.All(co => co.Check(enemy)));

            if (!enemies.Any())
                return false;

            blackboard.Register("targets", enemies);
            return true;
        }
    }
}