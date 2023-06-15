using System.Collections.Generic;
using System.Linq;
using AI.Utils;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using UnityEngine;

namespace AI.Actions
{
    public class DamageTarget : BehaviorTreeTask
    {
        public override async UniTask<bool> Run(Blackboard blackboard)
        {
            var targets = blackboard.Get<IEnumerable<AbilitySystemActor>>("targets");
            var context = blackboard.Get<IBattleContext>("context");
            var caster = blackboard.Get<AbilitySystemActor>("caster");

            foreach (var target in targets)
            {
                if (await TryExecuteTo(context, caster, target))
                    return true;
            }

            return false;
        }

        private async UniTask<bool> TryExecuteTo(IBattleContext battleContext, AbilitySystemActor caster,
            AbilitySystemActor target)
        {
            var availableDamageAbilities =
                AbilityAIPresentation.GetAvailableDamageAbilities(battleContext, caster, target);

            if (!availableDamageAbilities.Any())
                return false;

            //Find first ability that kill target or deal maximum damage
            var bestUseAbility = availableDamageAbilities.FirstOrDefault(ab => ab.Item2.CurrentDamage >=
                                                                               target.LifeStats.Health).ability ??
                                 availableDamageAbilities.Last().ability;

            switch (bestUseAbility.AbilityData.TargetingSystem)
            {
                case SingleTargetTargetingSystem:
                {
                    await bestUseAbility.CastSingleTarget(battleContext, caster, target.Position);
                    break;
                }
            }

            return true;
        }
    }
}