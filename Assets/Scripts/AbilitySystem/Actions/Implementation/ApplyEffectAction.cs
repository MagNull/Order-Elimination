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
        public IEffect EffectSample { get; set; }
        public float ApplyChance { get; set; }

        protected override bool Perform(ActionExecutionContext useContext)
        {
            if (RandomExtensions.RollChance(ApplyChance))
            {
                return useContext.ActionTarget.ApplyEffect(EffectSample, useContext.ActionMaker);
            }
            return false;
        }
    }
}
