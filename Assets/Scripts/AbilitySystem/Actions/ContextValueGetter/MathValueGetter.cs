using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    [Serializable]
    public struct MathValueGetter : IContextValueGetter
    {
        [BoxGroup("Operation", ShowLabel = false)]
        [OdinSerialize]
        public IContextValueGetter Left { get; set; }

        [BoxGroup("Operation", ShowLabel = false)]
        [OdinSerialize]
        public BinaryMathOperation Operation { get; set; }

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

        public bool CanBePrecalculatedWith(ValueCalculationContext context)
        {
            return Left.CanBePrecalculatedWith(context) && Right.CanBePrecalculatedWith(context);
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
            var result = MathExtensions.PerformBinaryOperation(leftValue, rightValue, Operation);
            return result;
        }
    }
}
