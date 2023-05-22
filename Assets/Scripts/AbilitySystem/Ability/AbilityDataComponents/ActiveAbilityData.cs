using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace OrderElimination.AbilitySystem
{
    public class ActiveAbilityData
    {
        public AbilityView View { get; set; }
        public AbilityGameRepresentation GameRepresentation { get; set; }
        public AbilityRules Rules { get; set; }
        public IAbilityTargetingSystem TargetingSystem { get; set; } //For Active Abilities
        public ActiveAbilityExecution Execution { get; set; }
    }
}
