using System.Linq;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace AI.Conditions
{
    public class AbilityAvailabilityCheck : SequentialTask
    {
        [SerializeField]
        private ActiveAbilityBuilder[] _abilitiesToCheck;
        [FormerlySerializedAs("_available")]
        [SerializeField]
        private bool _availabilityToCheck;

        protected override async UniTask<bool> Run(Blackboard blackboard)
        {
            var caster = blackboard.Get<AbilitySystemActor>("caster");
            var abilities = caster.ActiveAbilities;
            foreach (var ability in abilities)
            {
                if (!_abilitiesToCheck.Contains(ability.AbilityData.BasedBuilder) || ability.Cooldown <= 0)
                    continue;
                
                if(_availabilityToCheck)
                    return false;
            }

            return true;
        }
    }
}