using System.Collections.Generic;
using System.Linq;
using OrderElimination.AbilitySystem;

namespace AI.Utils
{
    public static class AbilityAIPresentation
    {
        public static IEnumerable<(ActiveAbilityRunner ability, AbilityImpact impact)> GetAvailableDamageAbilities(IBattleContext battleContext, 
            AbilitySystemActor caster, AbilitySystemActor target)
        {
            return caster.ActiveAbilities
                .Where(ability => ability.IsCastAvailable(battleContext, caster))
                .Select(ability => (ability, new AbilityImpact(ability.AbilityData, battleContext, caster, target.Position)))
                .Where(impact => impact.Item2.Damage > 0)
                .Where(evaluatedAbility =>
                    evaluatedAbility.ability.AbilityData.Rules.GetAvailableCellPositions(battleContext, caster)
                        .Contains(target.Position))
                .OrderBy(evaluatedAbility => evaluatedAbility.Item2.Damage);
        }
        
        public static IEnumerable<(ActiveAbilityRunner ability, AbilityImpact impact)> GetAvailableHealAbilities(IBattleContext battleContext, 
            AbilitySystemActor caster, AbilitySystemActor target)
        {
            return caster.ActiveAbilities
                .Where(ability => ability.IsCastAvailable(battleContext, caster))
                .Select(ability => (ability, new AbilityImpact(ability.AbilityData, battleContext, caster, target.Position)))
                .Where(impact => impact.Item2.Heal > 0)
                .Where(evaluatedAbility =>
                    evaluatedAbility.ability.AbilityData.Rules.GetAvailableCellPositions(battleContext, caster)
                        .Contains(target.Position))
                .OrderBy(evaluatedAbility => evaluatedAbility.Item2.Heal);
        }

        public static IEnumerable<(ActiveAbilityRunner ability, AbilityImpact impact)> GetDamageAbilities(IBattleContext battleContext,
            AbilitySystemActor caster, AbilitySystemActor target)
        {
            return caster.ActiveAbilities
                .Where(ability => ability.IsCastAvailable(battleContext, caster))
                .Select(ability => (ability,
                    new AbilityImpact(ability.AbilityData, battleContext, caster, target.Position)))
                .Where(impact => impact.Item2.Damage > 0)
                .OrderBy(evaluatedAbility => evaluatedAbility.Item2.Damage);
        }

        public static ActiveAbilityRunner GetMoveAbility(AbilitySystemActor caster) => caster.ActiveAbilities
            .First(a => a.AbilityData.View.Name == "Перемещение");
    }
}