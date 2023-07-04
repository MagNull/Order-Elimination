using System.Collections.Generic;
using System.Linq;
using AI.Actions;
using AI.Utils;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace AI.Conditions
{
    public class HasTargetsInRadius : BehaviorTreeTask
    {
        [Title("Target Conditions")]
        [SerializeField]
        private TargetSort _sortBy;
        [SerializeReference]
        private ITargetCondition[] _targetConditions;
        [SerializeField]
        private BattleRelationship _relationship;
        [SerializeField]
        private int _radius;

        [Title("Count")]
        [SerializeField]
        private int _count;
        [SerializeField]
        private Relation _relation;


        public override async UniTask<bool> Run(Blackboard blackboard)
        {
            var context = blackboard.Get<IBattleContext>("context");
            var caster = blackboard.Get<AbilitySystemActor>("caster");
            IEnumerable<AbilitySystemActor> targets;
            switch (_sortBy)
            {
                case TargetSort.Distance:
                    targets = context.EntitiesBank.GetTargetsByDistance(context, caster, _relationship);
                    break;
                case TargetSort.Value:
                    targets = context.EntitiesBank.GetTargetsByValue(context, caster, _relationship);
                    break;
                default:
                    targets = new AbilitySystemActor[] { };
                    break;
            }

            targets = targets.Where(e =>
                context.BattleMap.GetGameDistanceBetween(e.Position, caster.Position) <= _radius)
                .Where(enemy => _targetConditions.All(co => co.Check(enemy)));

            if (!targets.Any())
                return false;

            switch (_relation)
            {
                case Relation.Equal:
                    if (!(targets.Count() == _count))
                        return false;
                    break;
                case Relation.Greater:
                    if (!(targets.Count() > _count))
                        return false;
                    break;
                case Relation.Less:
                    if (!(targets.Count() < _count))
                        return false;
                    break;
            }
            
            blackboard.Register("targets", targets);
            return true;
        }
    }
}