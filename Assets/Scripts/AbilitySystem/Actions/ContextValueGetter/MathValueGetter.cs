using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace OrderElimination.AbilitySystem
{
    [Serializable]
    [Obsolete("Use " + nameof(BinaryMathValueGetter) + " instead.")]
    public struct MathValueGetter : IContextValueGetter
    {
        [BoxGroup("Operation", ShowLabel = false)]
        [OdinSerialize]
        public IContextValueGetter Left { get; set; }

        [BoxGroup("Operation", ShowLabel = false)]
        [OdinSerialize]
        public MathOperation Operation { get; set; }

        [BoxGroup("Operation", ShowLabel = false)]
        [OdinSerialize]
        public IContextValueGetter Right { get; set; }

        public string DisplayedFormula
        {
            get
            {
                var leftValue = Left != null 
                    ? Left.DisplayedFormula 
                    : IContextValueGetter.EmptyValueReplacement;
                var rightValue = Right != null 
                    ? Right.DisplayedFormula 
                    : IContextValueGetter.EmptyValueReplacement;
                var operation = Operation.AsString();
                return $"({leftValue} {operation} {rightValue})";
            }
        }

        public IContextValueGetter Clone()
        {
            var clone = new MathValueGetter();
            clone.Left = Left.Clone();
            clone.Right = Right.Clone();
            clone.Operation = Operation;
            return clone;
        }

        public float GetValue(ValueCalculationContext context)
        {
            var leftValue = Left != null ? Left.GetValue(context) : 0;
            var rightValue = Right != null ? Right.GetValue(context) : 0;
            var result = Operation switch
            {
                MathOperation.Add => leftValue + rightValue,
                MathOperation.Subtract => leftValue - rightValue,
                MathOperation.Multiply => leftValue * rightValue,
                MathOperation.Divide => leftValue / rightValue,
                _ => throw new NotImplementedException(),
            };
            return result;
        }
    }
}
