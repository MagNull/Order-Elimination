using System.Collections.Generic;
using System.Linq;
using AI.Utils;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using UnityEngine;
using UnityEngine.Serialization;

namespace AI.Actions
{
    public enum TargetSort
    {
        Distance,
        Value
    }
    

    public class HasTargetsInRadius : IBehaviorTreeTask
    {
        [SerializeField]
        private TargetSort _sortBy;

        [SerializeField]
        private PassiveAbilityBuilder[] _needPassiveEffects;

        [SerializeField]
        private BattleRelationship _relationship;

        [SerializeField]
        private int _radius;

        public async UniTask<bool> Run(Blackboard blackboard)
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
                .Where(CheckPassiveEffect);

            if (!enemies.Any())
                return false;

            blackboard.Register("targets", enemies);
            return true;
        }
        
        private bool CheckPassiveEffect(AbilitySystemActor target)
        {
            if (!_needPassiveEffects.Any())
                return true;
            var targetPassives = target.PassiveAbilities.Select(ab => ab.AbilityData.BasedBuilder);
            return _needPassiveEffects.All(ef => targetPassives.Contains(ef));
        }
    }
}