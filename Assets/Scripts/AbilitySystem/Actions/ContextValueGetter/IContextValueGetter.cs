using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;


namespace OrderElimination.AbilitySystem
{
    [PropertyTooltip("@$value." + nameof(DisplayedFormula))]
    public interface IContextValueGetter : ICloneable<IContextValueGetter>
    {
        //TODO: add identification, when value can be calculated and what info it requires
        // (e.g. target/battlecontext/caster/nothing)

        public const string EmptyValueReplacement = "_";

        public string DisplayedFormula { get; }

        public float GetValue(ValueCalculationContext context);
    }
}
