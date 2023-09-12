using System.Collections.Generic;
using System.Linq;
using OrderElimination.AbilitySystem;

namespace AI.Utils
{
    public static class AbilityAIPresentation
    {
        public static IEnumerable<EvaluatedAbilityRunner> GetAvailableDamageAbilities(
            IBattleContext battleContext, AbilitySystemActor caster, AbilitySystemActor target)
        {
            //TODO Doesn't support NoTarget abilities
            return caster.ActiveAbilities
                .Where(ability => ability.IsCastAvailable(battleContext, caster))
                .Select(ability => new EvaluatedAbilityRunner(ability, new AbilityImpact(ability.AbilityData, battleContext, caster, target.Position)))
                .Where(e => e.AbilityImpact.CurrentDamage > 0)
                .Where(e => 
                    e.AbilityRunner.AbilityData.TargetingSystem is IRequireSelectionTargetingSystem manualTargeting
                    && manualTargeting.CanSelectPeek(battleContext, caster, target.Position))
                .OrderBy(evaluatedAbility => evaluatedAbility.AbilityImpact.CurrentDamage);
        }
        
        public static IEnumerable<EvaluatedAbilityRunner> GetAvailableHealAbilities(
            IBattleContext battleContext, AbilitySystemActor caster, AbilitySystemActor target)
        {
            //TODO Doesn't support NoTarget abilities
            return caster.ActiveAbilities
                .Where(ability => ability.IsCastAvailable(battleContext, caster))
                .Select(ability => new EvaluatedAbilityRunner(ability, new AbilityImpact(ability.AbilityData, battleContext, caster, target.Position)))
                .Where(impact => impact.AbilityImpact.CurrentHeal > 0)
                .Where(e =>
                    e.AbilityRunner.AbilityData.TargetingSystem is IRequireSelectionTargetingSystem manualTargeting
                    && manualTargeting.CanSelectPeek(battleContext, caster, target.Position))
                .OrderBy(evaluatedAbility => evaluatedAbility.AbilityImpact.RawHeal);
        }

        public static IEnumerable<EvaluatedAbilityRunner> GetDamageAbilities(
            IBattleContext battleContext, AbilitySystemActor caster, AbilitySystemActor target)
        {
            //var test = caster.ActiveAbilities
            //    .Where(ability => ability.IsCastAvailable(battleContext, caster))
            //    .Select(ability => new EvaluatedAbilityRunner(ability, new AbilityImpact(ability.AbilityData, battleContext, caster, target.Position)));
            return caster.ActiveAbilities
                .Where(ability => ability.IsCastAvailable(battleContext, caster))
                .Select(ability => new EvaluatedAbilityRunner(ability, new AbilityImpact(ability.AbilityData, battleContext, caster, target.Position)))
                .Where(impact => impact.AbilityImpact.RawDamage > 0)
                .OrderBy(evaluatedAbility => evaluatedAbility.AbilityImpact.CurrentDamage);;
        }

        public static ActiveAbilityRunner GetMoveAbility(AbilitySystemActor caster) => caster.ActiveAbilities
            .FirstOrDefault(a => a.AbilityData.View.Name == "Перемещение");
    }
}