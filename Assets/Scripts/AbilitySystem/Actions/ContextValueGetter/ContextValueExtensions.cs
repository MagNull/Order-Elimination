namespace OrderElimination.AbilitySystem
{
    public static class ContextValueExtensions
    {
        public static string GetInPercentOrSimplify(this IContextValueGetter value, ValueCalculationContext context)
        {
            if (value.CanBePrecalculatedWith(context))
                return $"{value.GetValue(context) * 100}%";
            return value.GetSimplifiedFormula(context);
        }
    }
}
