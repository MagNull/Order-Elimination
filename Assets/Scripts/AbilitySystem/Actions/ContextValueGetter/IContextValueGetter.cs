using OrderElimination.Infrastructure;
using Sirenix.Serialization;
using System;

namespace OrderElimination.AbilitySystem
{
    public interface IContextValueGetter : ICloneable<IContextValueGetter>
    {
        //TODO: add identification, when value can be calculated and what info it requires
        // (e.g. target/battlecontext/caster/nothing)

        public const string EmptyValueReplacement = "_";

        public string DisplayedFormula { get; }

        public float GetValue(ValueCalculationContext context);
    }
}
