using OrderElimination.Infrastructure;

namespace OrderElimination.AbilitySystem
{
    public interface IContextValueGetter : ICloneable<IContextValueGetter>
    {
        public const string EmptyValueReplacement = "_";

        public string DisplayedFormula { get; }

        float GetValue(ActionContext useContext);
    }
}
