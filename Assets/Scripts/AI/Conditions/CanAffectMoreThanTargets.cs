using System.Collections.Generic;
using System.Linq;
using AI.Utils;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using UnityEngine;

namespace AI.Conditions
{
    public class CanAffectMoreThanTargets : SequentialTask
    {
        [SerializeField]
        private ActiveAbilityBuilder _abilityBuilder;

        [SerializeField]
        private BattleRelationship _relationship;

        [SerializeField]
        private int _count;

        protected async override UniTask<bool> Run(Blackboard blackboard)
        {
            var targets = blackboard.Get<IEnumerable<AbilitySystemActor>>("targets");
            var context = blackboard.Get<IBattleContext>("context");
            var caster = blackboard.Get<AbilitySystemActor>("caster");

            var abilities = caster.ActiveAbilities;
            var ability = abilities.FirstOrDefault(ab => ab.AbilityData.BasedBuilder == _abilityBuilder);
            if (ability == null)
                return false;

            var newTargets = targets.Where(target => Check(context, caster, ability, target));

            if(!newTargets.Any())
                return false;
            blackboard.Register("targets", newTargets);
            return true;
        }

        private bool Check(IBattleContext context, AbilitySystemActor caster, ActiveAbilityRunner ability,
            AbilitySystemActor target)
        {
            var abilityImpact = new AbilityImpact(ability.AbilityData, context, caster, target.Position);
            switch (_relationship)
            {
                case BattleRelationship.Ally:
                    if (abilityImpact.AffectedAlies > _count)
                        return true;
                    break;
                case BattleRelationship.Enemy:
                    if (abilityImpact.AffectedEnemies > _count)
                        return true;
                    break;
            }

            return false;
        }
    }
}