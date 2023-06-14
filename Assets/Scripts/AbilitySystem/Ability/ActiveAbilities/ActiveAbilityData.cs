using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using System;

namespace OrderElimination.AbilitySystem
{
    public class ActiveAbilityData : IActiveAbilityData
    {
        public AbilityView View { get; set; }
        public AbilityGameRepresentation GameRepresentation { get; set; }
        public AbilityRules Rules { get; set; }
        public IAbilityTargetingSystem TargetingSystem { get; set; } //For Active Abilities
        public ActiveAbilityExecution Execution { get; set; }
        public ActiveAbilityBuilder BasedBuilder { get; }

        public ActiveAbilityData(ActiveAbilityBuilder builderData)
        {
            BasedBuilder = builderData;
            var view = new AbilityView(
                builderData.CellGroupsHighlightColors,
                builderData.Name,
                builderData.Icon,
                builderData.Description,
                builderData.HideInCharacterDiscription);
            var gameRepresentation = new AbilityGameRepresentation();
            gameRepresentation.CooldownTime = builderData.CooldownTime;
            var rules = new AbilityRules(builderData.AvailabilityConditions, builderData.TargetCellConditions, builderData.UsageCost);
            IAbilityTargetingSystem targetingSystem;
            if (builderData.TargetingSystem == TargetingSystemType.NoTarget)
            {
                var casterPattern = (CasterRelativePattern)builderData.DistributionPattern;
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

            View = view;
            GameRepresentation = gameRepresentation;
            Rules = rules;
            TargetingSystem = targetingSystem;
            Execution = execution;
        }
    }

    public interface IActiveAbilityData
    {
        public AbilityView View { get; }
        //AbilityPreview ? (Range, Damage, etc.)
        public AbilityGameRepresentation GameRepresentation { get; }
        public AbilityRules Rules { get; }
        public IAbilityTargetingSystem TargetingSystem { get; }
        public ActiveAbilityExecution Execution { get; }
        public ActiveAbilityBuilder BasedBuilder { get; }
    }
}
