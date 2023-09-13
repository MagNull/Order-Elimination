using OrderElimination.Infrastructure;
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
                builderData.HideInCharacterDescription,
                builderData.ShowCrosshairWhenTargeting,
                builderData.ShowTrajectoryWhenTargeting);
            var rules = new AbilityRules(builderData.AvailabilityConditions, builderData.UsageCost);
            IAbilityTargetingSystem targetingSystem;
            if (builderData.TargetingSystem == TargetingSystemType.NoTarget)
            {
                targetingSystem = new NoTargetTargetingSystem(builderData.CellGroupsDistributor);
            }
            else if (builderData.TargetingSystem == TargetingSystemType.SingleTarget)
            {
                targetingSystem = new SingleTargetTargetingSystem(
                    builderData.CellGroupsDistributor,
                    builderData.TargetCellConditions);
            }
            else if (builderData.TargetingSystem == TargetingSystemType.MultiTarget)
            {
                targetingSystem = new MultiTargetTargetingSystem(
                    builderData.CellGroupsDistributor,
                    builderData.TargetCellConditions,
                    builderData.NecessaryTargets,
                    builderData.OptionalTargets);
            }
            else
            {
                throw new NotImplementedException();
            }
            var instructions = builderData.AbilityInstructions.DeepClone();
            var execution = new ActiveAbilityExecution(instructions);

            var gameRepresentation = AbilityGameRepresentation.FromActiveAbility(
                rules, builderData.CooldownTime, targetingSystem, execution);
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
                builderData.HideInCharacterDescription);
            var execution = new PassiveAbilityExecution(
                builderData.ActionsOnActivation ?? new IBattleAction[0], 
                builderData.TriggerInstructions ?? new ITriggerInstruction[0]);
            var gameRepresentation = AbilityGameRepresentation.FromPassiveAbility(
                builderData.CooldownTime, execution);
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
