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
        private BinaryMathOperation _damageOperation = BinaryMathOperation.Multiply;

        [FoldoutGroup("DamageChanger")]
        [BoxGroup("DamageChanger/DamageSize")]
        [ShowInInspector, OdinSerialize]
        private IContextValueGetter _damageValue = new ConstValueGetter() { Value = 1 };

        [FoldoutGroup("DamageChanger")]
        [BoxGroup("DamageChanger/DamageProperties")]
        [HorizontalGroup("DamageChanger/DamageProperties/Priority")]
        [ToggleLeft]
        [ShowInInspector, OdinSerialize]
        private bool _changeDamagePriority;

        [FoldoutGroup("DamageChanger")]
        [BoxGroup("DamageChanger/DamageProperties")]
        [HorizontalGroup("DamageChanger/DamageProperties/Priority")]
        [HideLabel]
        [EnableIf(nameof(_changeDamagePriority))]
        [ShowInInspector, OdinSerialize]
        private LifeStatPriority _damagePriority;

        [FoldoutGroup("DamageChanger")]
        [BoxGroup("DamageChanger/DamageProperties")]
        [HorizontalGroup("DamageChanger/DamageProperties/Type")]
        [ToggleLeft]
        [ShowInInspector, OdinSerialize]
        private bool _changeDamageType;

        [FoldoutGroup("DamageChanger")]
        [BoxGroup("DamageChanger/DamageProperties")]
        [HorizontalGroup("DamageChanger/DamageProperties/Type")]
        [HideLabel]
        [EnableIf(nameof(_changeDamageType))]
        [ShowInInspector, OdinSerialize]
        private DamageType _damageType;

        [FoldoutGroup("DamageChanger")]
        [BoxGroup("DamageChanger/DamageProperties")]
        [HorizontalGroup("DamageChanger/DamageProperties/ArmorMult")]
        [ToggleLeft]
        [ShowInInspector, OdinSerialize]
        private bool _changeArmorMultiplier;

        [FoldoutGroup("DamageChanger")]
        [BoxGroup("DamageChanger/DamageProperties")]
        [HorizontalGroup("DamageChanger/DamageProperties/ArmorMult")]
        [HideLabel]
        [EnableIf(nameof(_changeArmorMultiplier))]
        [ShowInInspector, OdinSerialize]
        private float _armorMultiplier = 1;

        [FoldoutGroup("DamageChanger")]
        [BoxGroup("DamageChanger/DamageProperties")]
        [HorizontalGroup("DamageChanger/DamageProperties/HealthMult")]
        [ToggleLeft]
        [ShowInInspector, OdinSerialize]
        private bool _changeHealthMultiplier;

        [FoldoutGroup("DamageChanger")]
        [BoxGroup("DamageChanger/DamageProperties")]
        [HorizontalGroup("DamageChanger/DamageProperties/HealthMult")]
        [HideLabel]
        [EnableIf(nameof(_changeHealthMultiplier))]
        [ShowInInspector, OdinSerialize]
        private float _healthMultiplier = 1;

        [FoldoutGroup("DamageChanger")]
        [BoxGroup("DamageChanger/DamageProperties")]
        [ShowInInspector, OdinSerialize]
        private bool _ignoreObstacles;

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
        private BinaryMathOperation _accuracyOperation = BinaryMathOperation.Multiply;

        [FoldoutGroup("AccuracyChanger")]
        [ShowInInspector, OdinSerialize]
        private IContextValueGetter _accuracyValue = new ConstValueGetter() { Value = 1 };

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

        public EnumMask<DamageType> AllowedDamageTypes => _allowedDamageTypes;
        public bool IsChangingDamage => !IsReturnsToSameValue(DamageOperation, DamageOperand);
        public BinaryMathOperation DamageOperation => _damageOperation;
        public IContextValueGetter DamageOperand => _damageValue;
        public bool IsChangingAccuracy => !IsReturnsToSameValue(AccuracyOperation, AccuracyOperand);
        public BinaryMathOperation AccuracyOperation => _accuracyOperation;
        public IContextValueGetter AccuracyOperand => _accuracyValue;
        //DamageChange
        //AccuracyChange
        public bool ChangeDamagePriority => _changeDamagePriority;
        public LifeStatPriority OverriddenDamagePriority => _damagePriority;
        public bool ChangeDamageType => _changeDamageType;
        public DamageType OverriddenDamageType => _damageType;
        public float ArmorMultiplier => _armorMultiplier;
        public float HealthMultiplier => _healthMultiplier;
        public bool IgnoreObstacles => _ignoreObstacles;
        public bool IgnoreEvasion => _ignoreEvasion;
        #endregion

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
            if (_changeArmorMultiplier)
                damageAction.ArmorMultiplier *= _armorMultiplier;
            if (_changeHealthMultiplier)
                damageAction.HealthMultiplier *= _healthMultiplier;
            if (_ignoreObstacles)
                damageAction.ObjectsBetweenAffectAccuracy = true;
            if (_ignoreEvasion)
                damageAction.IgnoreEvasion = true;
            damageAction.Accuracy = ChangeValueGetter(damageAction.Accuracy, _accuracyOperation, _accuracyValue);
            return originalAction;
        }

        private IContextValueGetter ChangeValueGetter(
            IContextValueGetter initial, BinaryMathOperation operation, IContextValueGetter newValue)
        {
            var newFormula = new MathValueGetter();
            newFormula.Left = initial;
            newFormula.Operation = operation;
            newFormula.Right = newValue;
            return newFormula;
        }
    }
}
