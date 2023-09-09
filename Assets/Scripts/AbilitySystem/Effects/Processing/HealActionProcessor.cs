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

        #region Public Properties
        private bool IsReturnsToSameValue(BinaryMathOperation operation, IContextValueGetter operand)
        {
            if (!operand.CanBePrecalculatedWith(ValueCalculationContext.Empty))
                return false;//hard to answer
            var value = operand.GetValue(ValueCalculationContext.Empty);
            return value == 1
                && (operation == BinaryMathOperation.Multiply || operation == BinaryMathOperation.Divide)
                || value == 0
                && (operation == BinaryMathOperation.Add || operation == BinaryMathOperation.Subtract);
        }

        public bool IsChangingHeal => !IsReturnsToSameValue(_healOperation, _healValue);
        public BinaryMathOperation HealOperation => _healOperation;
        public IContextValueGetter HealOperand => _healValue;
        #endregion

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
