using System;

namespace OrderElimination.AbilitySystem
{
    public class RoundValueGetter : IContextValueGetter
    {
        public string DisplayedFormula => "Round";

        public bool CanBePrecalculatedWith(ValueCalculationContext context)
        {
            return context.BattleContext != null;
        }

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
