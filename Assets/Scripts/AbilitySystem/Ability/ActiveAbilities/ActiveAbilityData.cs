using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace OrderElimination.AbilitySystem
{
    public class ActiveAbilityData : IActiveAbilityData
    {
        public AbilityView View { get; set; }
        public AbilityGameRepresentation GameRepresentation { get; set; }
        public AbilityRules Rules { get; set; }
        public IAbilityTargetingSystem TargetingSystem { get; set; } //For Active Abilities
        public ActiveAbilityExecution Execution { get; set; }
    }

    public interface IActiveAbilityData
    {
        public AbilityView View { get; }
        public AbilityGameRepresentation GameRepresentation { get; }
        public AbilityRules Rules { get; }
        public IAbilityTargetingSystem TargetingSystem { get; }
        public ActiveAbilityExecution Execution { get; }
    }
}
