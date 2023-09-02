using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class RoundValueGetter : IContextValueGetter
    {
        public string DisplayedFormula => "Round";

        public IContextValueGetter Clone()
        {
            var clone = new RoundValueGetter();
            return clone;
        }

        public float GetValue(ValueCalculationContext context)
        {
            return context.BattleContext.CurrentRound;
        }
    }
}
