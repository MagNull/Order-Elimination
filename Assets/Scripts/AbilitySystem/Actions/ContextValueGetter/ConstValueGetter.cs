using Sirenix.Serialization;
using System;

namespace OrderElimination.AbilitySystem
{
    [Serializable]
    public struct ConstValueGetter : IContextValueGetter
    {
        [OdinSerialize]
        public float Value { get; set; }

        public string DisplayedFormula => Value.ToString();

        public IContextValueGetter Clone()
        {
            var clone = new ConstValueGetter();
            clone.Value = Value;
            return clone;
        }

        public float GetValue(ValueCalculationContext context) => Value;

        public bool CanBePrecalculatedWith(ValueCalculationContext context) => true;

        public ConstValueGetter(float value) => Value = value;
    }
}
