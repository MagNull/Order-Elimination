using OrderElimination.AbilitySystem;
using OrderElimination.AbilitySystem.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class TimerTrigger : IContextTriggerSetup
    {
        public IBattleTrigger GetTrigger(IBattleContext battleContext)
        {
            var instance = new ITriggerSetup.BattleTrigger(this);
        }
    }
}
