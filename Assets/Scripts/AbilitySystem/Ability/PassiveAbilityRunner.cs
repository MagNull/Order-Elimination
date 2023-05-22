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

        public bool Activate(IBattleContext battleContext, AbilitySystemActor caster)
        {
            AbilityData.Execution.Activate(battleContext, caster);
            return true;
        }
    }
}
