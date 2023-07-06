using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace OrderElimination.AbilitySystem
{
    public class HealActionProcessor : IActionProcessor
    {
        [ShowInInspector, DisplayAsString, PropertyOrder(-1)]
        private string _healFormula
            => $"=(HealSize {_healOperation.AsString()} {_healValue.DisplayedFormula})";

        [ShowInInspector, OdinSerialize]
        private MathOperation _healOperation = MathOperation.Multiply;

        [ShowInInspector, OdinSerialize]
        private IContextValueGetter _healValue = new ConstValueGetter() { Value = 1 };

        public TAction ProcessAction<TAction>(TAction originalAction, ActionContext performContext)
            where TAction : BattleAction<TAction>
        {
            if (originalAction is not HealAction healAction)
                return originalAction;
            healAction.HealSize = ChangeValueGetter(healAction.HealSize, _healOperation, _healValue);
            return originalAction;
        }

        private MathValueGetter ChangeValueGetter(
            IContextValueGetter initial, MathOperation operation, IContextValueGetter value)
        {
            var newFormula = new MathValueGetter
            {
                Left = initial,
                Operation = operation,
                Right = value
            };
            return newFormula;
        }
    }
}
