using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class PassiveAbilityRunner
    {
        public PassiveAbilityRunner(PassiveAbilityData abilityData)
        {
            AbilityData = abilityData;
        }

        public PassiveAbilityData AbilityData { get; private set; }

        public bool IsActive { get; private set; } = false;

        private IBattleTrigger[] _activationTriggers;

        //public event Action<ActiveAbilityRunner> AbilityCasted;
        //public event Action<ActiveAbilityRunner> AbilityCastCompleted;

        public void Activate(IBattleContext battleContext, AbilitySystemActor caster)
        {
            if (IsActive) throw new InvalidOperationException("Passive Ability Runner has already been activated.");
            _activationTriggers = AbilityData.Execution.Activate(battleContext, caster);
            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive) throw new InvalidOperationException("Passive Ability Runner is not active.");
            AbilityData.Execution.Dectivate(_activationTriggers);
            IsActive = false;
        }
    }
}
