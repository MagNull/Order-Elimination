using System.Collections.Generic;
using System.Linq;
using AI.Utils;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AI.Conditions
{
    public class FindTarget : SequentialTask
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


        protected override async UniTask<bool> Run(Blackboard blackboard)
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

            targets = targets
                .Where(e =>
                    !e.IsDisposedFromBattle &&
                    context.BattleMap.GetGameDistanceBetween(e.Position, caster.Position) <= _radius)
                .Where(enemy => _targetConditions.All(co => co.Check(enemy)));
            var targetsCount = targets.Count();
            Debug.Log("Targets count " + targetsCount);

            if (!targets.Any())
                return false;
            blackboard.Register("targets", targets);

            switch (_relation)
            {
                case Relation.Equal:
                    if (targetsCount != _count)
                        return false;
                    break;
                case Relation.Greater:
                    if (!(targetsCount > _count))
                        return false;
                    break;
                case Relation.Less:
                    if (!(targetsCount < _count))
                        return false;
                    break;
            }
            Debug.Log("Targets success");

            return true;
        }
    }
}