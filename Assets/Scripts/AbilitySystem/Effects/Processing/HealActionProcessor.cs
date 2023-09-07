using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace OrderElimination.AbilitySystem
{
    public class HealActionProcessor : IActionProcessor
    {
        [ShowInInspector, DisplayAsString, PropertyOrder(-1)]
        private string _healFormula
        {
            get
            {
                if (_healValue != null)
                    return $"=(HealSize {_healOperation.AsString()} {_healValue.DisplayedFormula})";
                return "No value operand.";
            }
        }

        [ShowInInspector, OdinSerialize]
        private BinaryMathOperation _healOperation = BinaryMathOperation.Multiply;

        [ShowInInspector, OdinSerialize]
        private IContextValueGetter _healValue = new ConstValueGetter(1);

        public TAction ProcessAction<TAction>(TAction originalAction, ActionContext performContext)
            where TAction : BattleAction<TAction>
        {
            if (originalAction is not HealAction healAction)
                return originalAction;
            healAction.HealSize = ChangeValueGetter(healAction.HealSize, _healOperation, _healValue);
            return originalAction;
        }

        private IContextValueGetter ChangeValueGetter(
            IContextValueGetter initial, BinaryMathOperation operation, IContextValueGetter value)
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
