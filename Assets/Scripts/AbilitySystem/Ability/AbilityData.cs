using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class AbilityData : ScriptableObject
    {
        public AbilityView View { get; private set; }
        public AbilityGameRepresentation GameRepresentation { get; private set; }
        public AbilityRules Rules { get; private set; }
        public IAbilityTargetingSystem TargetingSystem { get; private set; }
        public AbilityExecution Execution { get; private set; }

        
    }
}
