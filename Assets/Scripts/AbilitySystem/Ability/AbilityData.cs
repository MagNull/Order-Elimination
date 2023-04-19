using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace OrderElimination.AbilitySystem
{
    [CreateAssetMenu(fileName = "AbilityData", menuName = "AbilitySystem/AbilityData")]
    public class AbilityData : SerializedScriptableObject
    {
        [ShowInInspector, OdinSerialize]
        public AbilityView View { get; set; }
        [ShowInInspector, OdinSerialize]
        public AbilityGameRepresentation GameRepresentation { get; set; }
        [ShowInInspector, OdinSerialize]
        public AbilityRules Rules { get; set; }
        [ShowInInspector, OdinSerialize]
        public IAbilityTargetingSystem TargetingSystem { get; set; }
        [ShowInInspector, OdinSerialize]
        public AbilityExecution Execution { get; set; }
    }
}
