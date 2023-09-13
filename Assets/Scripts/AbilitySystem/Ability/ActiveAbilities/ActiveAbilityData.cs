using OrderElimination.AbilitySystem.GameRepresentation;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class ActiveAbilityData : IActiveAbilityData
    {
        public ActiveAbilityBuilder BasedBuilder { get; set; }
        public ActiveAbilityView View { get; set; }
        public IAbilityGameRepresentation GameRepresentation { get; set; }
        public AbilityRules Rules { get; set; }
        public IAbilityTargetingSystem TargetingSystem { get; set; }
        public ActiveAbilityExecution Execution { get; set; }
    }

    public interface IActiveAbilityData
    {
        public ActiveAbilityBuilder BasedBuilder { get; }
        public ActiveAbilityView View { get; }
        public IAbilityGameRepresentation GameRepresentation { get; }
        public AbilityRules Rules { get; }
        public IAbilityTargetingSystem TargetingSystem { get; }
        public ActiveAbilityExecution Execution { get; }
    }
}
