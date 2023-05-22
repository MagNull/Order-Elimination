using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class PassiveAbilityExecution
    {
        public ITriggerInstruction[] TriggerInstructions;

        public void Activate(IBattleContext battleContext, AbilitySystemActor caster)
        {
            foreach (var instruction in TriggerInstructions)
            {
                instruction.Activate(battleContext, caster);
            }
        }
    }
}
