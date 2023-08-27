using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace OrderElimination.AbilitySystem
{
    public class DamageActionProcessor : IActionProcessor
    {
        [FoldoutGroup("Damage Filter", order: -2)]
        [ShowInInspector, OdinSerialize]
        private EnumMask<DamageType> _allowedDamageTypes = EnumMask<DamageType>.Full;

        [ShowInInspector, DisplayAsString, PropertyOrder(-1)]
        [FoldoutGroup("DamageChanger", order: 0)]
        [BoxGroup("DamageChanger/DamageSize")]
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
        [BoxGroup("DamageChanger/DamageSize")]
        [ShowInInspector, OdinSerialize]
        private MathOperation _damageOperation = MathOperation.Multiply;

        [FoldoutGroup("DamageChanger")]
        [BoxGroup("DamageChanger/DamageSize")]
        [ShowInInspector, OdinSerialize]
        private IContextValueGetter _damageValue = new ConstValueGetter() { Value = 1 };

        [FoldoutGroup("DamageChanger")]
        [BoxGroup("DamageChanger/DamageProperties")]
        [ShowInInspector, OdinSerialize]
        private bool _changeDamagePriority;

        [FoldoutGroup("DamageChanger")]
        [BoxGroup("DamageChanger/DamageProperties")]
        [ShowIf(nameof(_changeDamagePriority))]
        [ShowInInspector, OdinSerialize]
        private LifeStatPriority _damagePriority;

        [FoldoutGroup("DamageChanger")]
        [BoxGroup("DamageChanger/DamageProperties")]
        [ShowInInspector, OdinSerialize]
        private bool _changeDamageType;

        [FoldoutGroup("DamageChanger")]
        [BoxGroup("DamageChanger/DamageProperties")]
        [ShowIf(nameof(_changeDamageType))]
        [ShowInInspector, OdinSerialize]
        private DamageType _damageType;

        [FoldoutGroup("DamageChanger")]
        [BoxGroup("DamageChanger/DamageProperties")]
        [ShowInInspector, OdinSerialize]
        private float _armorMultiplier = 1;

        [FoldoutGroup("DamageChanger")]
        [BoxGroup("DamageChanger/DamageProperties")]
        [ShowInInspector, OdinSerialize]
        private float _healthMultiplier = 1;

        [FoldoutGroup("DamageChanger")]
        [BoxGroup("DamageChanger/DamageProperties")]
        [ShowInInspector, OdinSerialize]
        private bool _ignoreEvasion;

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
            var newDamage = ChangeValueGetter(damageAction.DamageSize, _damageOperation, _damageValue);
            damageAction.DamageSize = newDamage;
            if (_changeDamagePriority)
                damageAction.DamagePriority = _damagePriority;
            if (_changeDamageType)
                damageAction.DamageType = _damageType;
            damageAction.ArmorMultiplier *= _armorMultiplier;
            damageAction.HealthMultiplier *= _healthMultiplier;
            if (_ignoreEvasion)
                damageAction.IgnoreEvasion = true;
            damageAction.Accuracy = ChangeValueGetter(damageAction.Accuracy, _accuracyOperation, _accuracyValue);
            return originalAction;
        }

        private IContextValueGetter ChangeValueGetter(
            IContextValueGetter initial, MathOperation operation, IContextValueGetter newValue)
        {
            var newFormula = new MathValueGetter();
            newFormula.Left = initial;
            newFormula.Operation = operation;
            newFormula.Right = newValue;
            return newFormula;
        }
    }
}
