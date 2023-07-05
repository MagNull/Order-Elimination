using System;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public static class AbilityFactory
    {
        public static IActiveAbilityData CreateActiveAbility(ActiveAbilityBuilder builderData)
        {
            var view = new ActiveAbilityView(
                builderData.CellGroupsHighlightColors,
                builderData.Name,
                builderData.Icon,
                builderData.Description,
                builderData.HideInCharacterDiscription,
                builderData.ShowCrosshairWhenTargeting,
                builderData.ShowTrajectoryWhenTargeting);
            var gameRepresentation = new AbilityGameRepresentation
            {
                CooldownTime = builderData.CooldownTime
            };
            var rules = new AbilityRules(builderData.AvailabilityConditions, builderData.TargetCellConditions, builderData.UsageCost);
            IAbilityTargetingSystem targetingSystem;
            if (builderData.TargetingSystem == TargetingSystemType.NoTarget)
            {
                var casterPattern = (CasterRelativePattern)builderData.DistributionPattern;
                targetingSystem = new NoTargetTargetingSystem(casterPattern);
            }
            else if (builderData.TargetingSystem == TargetingSystemType.SingleTarget)
            {
                targetingSystem = new SingleTargetTargetingSystem(
                    builderData.DistributionPattern,
                    builderData.TargetCellConditions);
            }
            else if (builderData.TargetingSystem == TargetingSystemType.MultiTarget)
            {
                targetingSystem = new MultiTargetTargetingSystem(
                    builderData.DistributionPattern,
                    builderData.TargetCellConditions,
                    builderData.NecessaryTargets,
                    builderData.OptionalTargets);
            }
            else
            {
                Logging.LogException(new NotImplementedException());
                throw new NotImplementedException();
            }
            var execution = new ActiveAbilityExecution(builderData.AbilityInstructions.ToArray());

            var abilityData = new ActiveAbilityData
            {
                BasedBuilder = builderData,
                View = view,
                GameRepresentation = gameRepresentation,
                Rules = rules,
                TargetingSystem = targetingSystem,
                Execution = execution
            };
            return abilityData;
        }

        public static IPassiveAbilityData CreatePassiveAbility(PassiveAbilityBuilder builderData)
        {
            var view = new PassiveAbilityView(
                builderData.Name,
                builderData.Icon,
                builderData.Description,
                builderData.HideInCharacterDiscription);
            var gameRepresentation = new AbilityGameRepresentation
            {
                CooldownTime = builderData.CooldownTime
            };
            var execution = new PassiveAbilityExecution(builderData.TriggerInstructions.ToArray());

            var abilityData = new PassiveAbilityData
            {
                View = view,
                GameRepresentation = gameRepresentation,
                Execution = execution,
                BasedBuilder = builderData
            };
            return abilityData;
        }
    }
}
