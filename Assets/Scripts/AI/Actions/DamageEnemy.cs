using System.Collections.Generic;
using System.Linq;
using AI.Utils;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;

namespace AI.Actions
{
    public class DamageEnemy : IBehaviorTreeTask
    {
        public async UniTask<bool> Run(Blackboard blackboard)
        {
            var targets = blackboard.Get<AbilitySystemActor[]>("enemies");
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
            var availableDamageAbilities = GetAvailableDamageAbilities(battleContext, caster, target);

            if (!availableDamageAbilities.Any())
                return false;

            var maxDamageAbility = availableDamageAbilities.First();

            switch (maxDamageAbility.ability.AbilityData.TargetingSystem)
            {
                case SingleTargetTargetingSystem:
                {
                    await maxDamageAbility.ability.CastSingleTarget(battleContext, caster, target.Position);
                    break;
                }
            }

            return true;
        }

        private IEnumerable<(ActiveAbilityRunner ability, AbilityImpact)> GetAvailableDamageAbilities(
            IBattleContext battleContext, AbilitySystemActor caster, AbilitySystemActor target)
        {
            return caster.ActiveAbilities
                .Where(ability => ability.IsCastAvailable(battleContext, caster))
                .Select(ability => (ability, new AbilityImpact(ability.AbilityData, battleContext, caster, target)))
                .Where(impact => impact.Item2.Damage > 0)
                .Where(evaluatedAbility =>
                    evaluatedAbility.ability.AbilityData.Rules.GetAvailableCellPositions(battleContext, caster)
                        .Contains(target.Position))
                .OrderByDescending(evaluatedAbility => evaluatedAbility.Item2.Damage);
        }
    }
}