using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace OrderElimination.AbilitySystem
{
    public class DamageActionProcessor : IActionProcessor
    {
        [FoldoutGroup("Action Filter", order: -2)]
        [ShowInInspector, OdinSerialize]
        private EnumMask<DamageType> _allowedDamageTypes = EnumMask<DamageType>.Full;

        [ShowInInspector, DisplayAsString, PropertyOrder(-1)]
        [FoldoutGroup("DamageChanger", order: 0)]
        private string _damageFormula
        {
            get
            {
                if (_damageValue != null)
                    return $"=(DamageSize {_damageOperation.AsString()} {_damageValue.DisplayedFormula})";
                return "No value operand.";
            }
        }

        [FoldoutGroup("DamageChanger")]
        [ShowInInspector, OdinSerialize]
        private MathOperation _damageOperation = MathOperation.Multiply;

        [FoldoutGroup("DamageChanger")]
        [ShowInInspector, OdinSerialize]
        private IContextValueGetter _damageValue = new ConstValueGetter() { Value = 1 };

        [ShowInInspector, DisplayAsString, PropertyOrder(-1)]
        [FoldoutGroup("AccuracyChanger", order: 1)]
        private string _accuracyFormula
        {
            get
            {
                if (_accuracyValue != null)
                    return $"=(Accuracy {_accuracyOperation.AsString()} {_accuracyValue.DisplayedFormula})";
                return "No value operand.";
            }
        }

        [FoldoutGroup("AccuracyChanger")]
        [ShowInInspector, OdinSerialize]
        private MathOperation _accuracyOperation = MathOperation.Multiply;

        [FoldoutGroup("AccuracyChanger")]
        [ShowInInspector, OdinSerialize]
        private IContextValueGetter _accuracyValue = new ConstValueGetter() { Value = 1 };

        public TAction ProcessAction<TAction>(TAction originalAction, ActionContext performContext)
            where TAction : BattleAction<TAction>
        {
            if (originalAction is not InflictDamageAction damageAction)
                return originalAction;
            if (_allowedDamageTypes != null && !_allowedDamageTypes[damageAction.DamageType])
                return originalAction;
            damageAction.DamageSize = ChangeValueGetter(damageAction.DamageSize, _damageOperation, _damageValue);
            damageAction.Accuracy = ChangeValueGetter(damageAction.Accuracy, _accuracyOperation, _accuracyValue);
            return originalAction;
        }

        private MathValueGetter ChangeValueGetter(
            IContextValueGetter initial, MathOperation operation, IContextValueGetter value)
        {
            var newFormula = new MathValueGetter();
            newFormula.Left = initial;
            newFormula.Operation = operation;
            newFormula.Right = value;
            return newFormula;
        }
    }
}
