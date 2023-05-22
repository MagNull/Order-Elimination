using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class PassiveAbilityExecution
    {
        public ITriggerAbilityInstruction[] TriggerInstructions;

        public PassiveAbilityExecution(ITriggerAbilityInstruction[] triggerInstructions)
        {
            TriggerInstructions = triggerInstructions;
        }

        public IBattleTrigger[] Activate(IBattleContext battleContext, AbilitySystemActor caster)
        {
            return TriggerInstructions.Select(i => i.Activate(battleContext, caster)).ToArray();
        }

        public void Dectivate(IBattleTrigger[] activationTriggers)
        {
            foreach (var trigger in activationTriggers)
            {
                trigger.Deactivate();
            }
        }
    }
}
