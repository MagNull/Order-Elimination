using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class ApplyEffectAction : BattleAction<ApplyEffectAction>
    {
        public IEffect Effect { get; set; }
        public float ApplyChance { get; set; }

        protected override bool Perform(ActionExecutionContext useContext)
        {
            if (RandomExtensions.TryChance(ApplyChance))
            {
                return useContext.ActionTarget.ApplyEffect(Effect);
            }
            return false;
        }
    }
}
