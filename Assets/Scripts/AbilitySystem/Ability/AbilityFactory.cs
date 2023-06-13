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
            var abilityData = new ActiveAbilityData(builderData);
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
            abilityData.BasedBuilder = builderData;

            return abilityData;
        }
    }
}
