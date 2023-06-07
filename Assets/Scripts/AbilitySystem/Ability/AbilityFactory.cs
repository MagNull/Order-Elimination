using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public static class AbilityFactory
    {
        public static IActiveAbilityData CreateActiveAbility(ActiveAbilityBuilder builderData)
        {
            var abilityData = new ActiveAbilityData();

            var view = new AbilityView(
                builderData.CellGroupsHighlightColors, 
                builderData.Name, 
                builderData.Icon, 
                builderData.Description);
            var gameRepresentation = new AbilityGameRepresentation();
            gameRepresentation.CooldownTime = builderData.CooldownTime;
            var rules = new AbilityRules(builderData.AvailabilityConditions, builderData.TargetCellConditions, builderData.UsageCost);
            IAbilityTargetingSystem targetingSystem;
            if (builderData.TargetingSystem == TargetingSystemType.NoTarget)
            {
                var casterPattern = (CasterRelativePattern) builderData.DistributionPattern;
                targetingSystem = new NoTargetTargetingSystem(casterPattern);
            }
            else if (builderData.TargetingSystem == TargetingSystemType.SingleTarget)
            {
                targetingSystem = new SingleTargetTargetingSystem(builderData.DistributionPattern);
            }
            else if (builderData.TargetingSystem == TargetingSystemType.MultiTarget)
            {
                targetingSystem = new MultiTargetTargetingSystem(
                    builderData.DistributionPattern, 
                    builderData.NecessaryTargets, 
                    builderData.OptionalTargets);
            }
            else 
                throw new NotImplementedException();
            var execution = new ActiveAbilityExecution(builderData.AbilityInstructions.ToArray());

            abilityData.View = view;
            abilityData.GameRepresentation = gameRepresentation;
            abilityData.Rules = rules;
            abilityData.TargetingSystem = targetingSystem;
            abilityData.Execution = execution;

            return abilityData;
        }

        public static IPassiveAbilityData CreatePassiveAbility(PassiveAbilityBuilder builderData)
        {
            var abilityData = new PassiveAbilityData();

            var view = new AbilityView(
                builderData.CellGroupsHighlightColors,
                builderData.Name,
                builderData.Icon,
                builderData.Description);
            var gameRepresentation = new AbilityGameRepresentation();
            gameRepresentation.CooldownTime = builderData.CooldownTime;
            var execution = new PassiveAbilityExecution(builderData.TriggerInstructions.ToArray());

            abilityData.View = view;
            abilityData.GameRepresentation = gameRepresentation;
            abilityData.Execution = execution;

            return abilityData;
        }
    }
}
