using System.Collections.Generic;
using System.Linq;
using AI.Utils;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;

namespace AI.Actions
{
    public abstract class DamageEnemy : IBehaviorTreeTask
    {
        public async UniTask<bool> Run(IBattleContext battleContext, AbilitySystemActor caster)
        {
            var targets = GetTargets(battleContext, caster);

            foreach (var target in targets)
            {
                if (await TryExecuteTo(battleContext, caster, target))
                    return true;
            }

            return false;
        }
        
        protected abstract AbilitySystemActor[] GetTargets(IBattleContext battleContext, AbilitySystemActor caster);
        
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

            return false;
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
    
    public class DamageNearest : DamageEnemy
    {
        protected override AbilitySystemActor[] GetTargets(IBattleContext battleContext, AbilitySystemActor caster) =>
            battleContext.EntitiesBank.GetEnemiesByDistance(battleContext, caster);
    }
    
    public class DamageMostValuable : DamageEnemy
    {
        protected override AbilitySystemActor[] GetTargets(IBattleContext battleContext, AbilitySystemActor caster) =>
            battleContext.EntitiesBank.GetEnemiesByValue();
    }
}