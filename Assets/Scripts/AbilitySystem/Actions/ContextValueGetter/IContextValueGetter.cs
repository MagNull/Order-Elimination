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
            if (this is MathValueGetter mathValue)
            {
                //Means one or both parts cant be precalculated. So must be end string
                var left = mathValue.Left.CanBePrecalculatedWith(context)
                    ? mathValue.Left.GetValue(context).ToString()
                    : mathValue.Left.GetSimplifiedFormula(context);
                var right = mathValue.Right.CanBePrecalculatedWith(context)
                    ? mathValue.Right.GetValue(context).ToString()
                    : mathValue.Right.GetSimplifiedFormula(context);
                var operation = mathValue.Operation.AsString();
                return $"({left} {operation} {right})";
                //Works bad with (1 * (2 * (3 * ?)))-type formulas
            }
            if (this is RandomValueGetter randValueGetter)
            {
                //Means one or both parts cant be precalculated. So must be end string
                var start = randValueGetter.RangeStart.CanBePrecalculatedWith(context)
                    ? randValueGetter.RangeStart.GetValue(context).ToString()
                    : randValueGetter.RangeStart.GetSimplifiedFormula(context);
                var end = randValueGetter.RangeEnd.CanBePrecalculatedWith(context)
                    ? randValueGetter.RangeEnd.GetValue(context).ToString()
                    : randValueGetter.RangeEnd.GetSimplifiedFormula(context);
                return $"[{start} – {end}]";
                //Works bad with (1 * (2 * (3 * ?)))-type formulas
            }
            return DisplayedFormula;
        }

        public virtual bool ContainsChildrenOfType<TValueGetter>()
            where TValueGetter : IContextValueGetter
        {
            if (this is MathValueGetter mathValueGetter)
            {
                return mathValueGetter.Left is TValueGetter
                    || mathValueGetter.Right is TValueGetter
                    || mathValueGetter.Left.ContainsChildrenOfType<TValueGetter>()
                    || mathValueGetter.Right.ContainsChildrenOfType<TValueGetter>();
            }
            if (this is RandomValueGetter randomValueGetter)
            {
                return randomValueGetter.RangeStart is TValueGetter
                    || randomValueGetter.RangeEnd is TValueGetter
                    || randomValueGetter.RangeStart.ContainsChildrenOfType<TValueGetter>()
                    || randomValueGetter.RangeEnd.ContainsChildrenOfType<TValueGetter>();
            }
            return false;
        }
    }
}
