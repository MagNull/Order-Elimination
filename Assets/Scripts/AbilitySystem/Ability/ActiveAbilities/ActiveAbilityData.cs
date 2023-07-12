using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class ActiveAbilityData : IActiveAbilityData
    {
        [field: SerializeField]
        public ActiveAbilityBuilder BasedBuilder { get; set; }
        public ActiveAbilityView View { get; set; }
        public AbilityGameRepresentation GameRepresentation { get; set; }
        public AbilityRules Rules { get; set; }
        public IAbilityTargetingSystem TargetingSystem { get; set; }
        public ActiveAbilityExecution Execution { get; set; }
    }

    public interface IActiveAbilityData
    {
        public ActiveAbilityBuilder BasedBuilder { get; }
        public ActiveAbilityView View { get; }
        public AbilityGameRepresentation GameRepresentation { get; }
        public AbilityRules Rules { get; }
        public IAbilityTargetingSystem TargetingSystem { get; }
        public ActiveAbilityExecution Execution { get; }
    }
}
