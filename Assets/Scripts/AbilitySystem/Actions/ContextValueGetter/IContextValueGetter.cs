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

        public bool CanBePrecalculatedWith(ValueCalculationContext context);

        public float GetValue(ValueCalculationContext context);

        public virtual string GetSimplifiedFormula(ValueCalculationContext context)
        {
            if (CanBePrecalculatedWith(context))
                return GetValue(context).ToString();
            if (this is MathValueGetter mathValueGetter)
            {
                //Means one or both parts cant be precalculated. So must be end string
                var left = mathValueGetter.Left.CanBePrecalculatedWith(context)
                    ? mathValueGetter.Left.GetValue(context).ToString()
                    : mathValueGetter.Left.GetSimplifiedFormula(context);
                var right = mathValueGetter.Left.CanBePrecalculatedWith(context)
                    ? mathValueGetter.Left.GetValue(context).ToString()
                    : mathValueGetter.Left.GetSimplifiedFormula(context);
                var operation = mathValueGetter.Operation.AsString();
                return $"({left} {operation} {right})";
                //Works bad with (1 * (2 * (3 * ?)))-type formulas
            }
            return DisplayedFormula;
        }
    }
}
