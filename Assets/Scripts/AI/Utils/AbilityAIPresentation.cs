﻿using System.Collections.Generic;
using System.Linq;
using OrderElimination.AbilitySystem;

namespace AI.Utils
{
    public static class AbilityAIPresentation
    {
        public static IEnumerable<(ActiveAbilityRunner ability, AbilityImpact impact)> GetAvailableDamageAbilities(IBattleContext battleContext, 
            AbilitySystemActor caster, AbilitySystemActor target)
        {
            //TODO Doesn't support NoTarget abilities
            return caster.ActiveAbilities
                .Where(ability => ability.IsCastAvailable(battleContext, caster))
                .Select(ability => (ability, new AbilityImpact(ability.AbilityData, battleContext, caster, target.Position)))
                .Where(e => e.Item2.CurrentDamage > 0)
                .Where(e => 
                    e.ability.AbilityData.TargetingSystem is IRequireSelectionTargetingSystem manualTargeting
                    && manualTargeting.PeekAvailableCells(battleContext, caster).Contains(target.Position))
                .OrderBy(evaluatedAbility => evaluatedAbility.Item2.CurrentDamage);
        }
        
        public static IEnumerable<(ActiveAbilityRunner ability, AbilityImpact impact)> GetAvailableHealAbilities(IBattleContext battleContext, 
            AbilitySystemActor caster, AbilitySystemActor target)
        {
            //TODO Doesn't support NoTarget abilities
            return caster.ActiveAbilities
                .Where(ability => ability.IsCastAvailable(battleContext, caster))
                .Select(ability => (ability, new AbilityImpact(ability.AbilityData, battleContext, caster, target.Position)))
                .Where(impact => impact.Item2.CurrentHeal > 0)
                .Where(e =>
                    e.ability.AbilityData.TargetingSystem is IRequireSelectionTargetingSystem manualTargeting
                    && manualTargeting.PeekAvailableCells(battleContext, caster).Contains(target.Position))
                .OrderBy(evaluatedAbility => evaluatedAbility.Item2.RawHeal);
        }

        public static IEnumerable<(ActiveAbilityRunner ability, AbilityImpact impact)> GetDamageAbilities(IBattleContext battleContext,
            AbilitySystemActor caster, AbilitySystemActor target)
        {
            var test = caster.ActiveAbilities
                .Where(ability => ability.IsCastAvailable(battleContext, caster))
                .Select(ability => (ability,
                    new AbilityImpact(ability.AbilityData, battleContext, caster, target.Position)));
            return caster.ActiveAbilities
                .Where(ability => ability.IsCastAvailable(battleContext, caster))
                .Select(ability => (ability,
                    new AbilityImpact(ability.AbilityData, battleContext, caster, target.Position)))
                .Where(impact => impact.Item2.RawDamage > 0)
                .OrderBy(evaluatedAbility => evaluatedAbility.Item2.CurrentDamage);;
        }

        public static ActiveAbilityRunner GetMoveAbility(AbilitySystemActor caster) => caster.ActiveAbilities
            .FirstOrDefault(a => a.AbilityData.View.Name == "Перемещение");
    }
}