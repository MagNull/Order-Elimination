using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace OrderElimination.AbilitySystem
{
    [Serializable]
    public class BinaryMathValueGetter : IContextValueGetter
    {
        [BoxGroup("Operation", ShowLabel = false)]
        [ShowInInspector, OdinSerialize]
        public IContextValueGetter Left { get; set; } = new ConstValueGetter(0);

        [BoxGroup("Operation", ShowLabel = false)]
        [ShowInInspector, OdinSerialize]
        public BinaryMathOperation Operation { get; set; }

        [BoxGroup("Operation", ShowLabel = false)]
        [ShowInInspector, OdinSerialize]
        public IContextValueGetter Right { get; set; } = new ConstValueGetter(0);

        public string DisplayedFormula 
            => $"({Left.DisplayedFormula} {Operation.AsString()} {Right.DisplayedFormula})";

        public IContextValueGetter Clone()
        {
            var clone = new BinaryMathValueGetter();
            clone.Left = Left.Clone();
            clone.Operation = Operation;
            clone.Right = Right.Clone();
            return clone;
        }

        public float GetValue(ValueCalculationContext context)
        {
            var left = Left.GetValue(context);
            var right = Right.GetValue(context);
            var result = MathExtensions.PerformBinaryOperation(left, right, Operation);
            return result;
        }
    }
}
